using System;
using System.ComponentModel.DataAnnotations;
using System.Reactive;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LastfmDiscordRPC2.Enums;
using LastfmDiscordRPC2.IO;
using LastfmDiscordRPC2.IO.Schema;
using LastfmDiscordRPC2.Logging;
using LastfmDiscordRPC2.Models.API;
using LastfmDiscordRPC2.Models.Responses;
using LastfmDiscordRPC2.Utilities;
using LastfmDiscordRPC2.ViewModels.Logging;
using ReactiveUI;

namespace LastfmDiscordRPC2.ViewModels.Panes;

public sealed class SettingsViewModel : ReactiveObject, IPaneViewModel
{
    private const string AppIDRegExp = @"^\d{17,21}$";

    public ReactiveCommand<bool, Unit> LaunchOnStartup { get; }
    public ReactiveCommand<bool, Unit> LastfmLogin { get; }
    public ReactiveCommand<Unit, Unit> SaveAppID { get; }
    public bool StartUpVisible { get; set; }
    public string PaneName { get; }
    public SettingsConsoleViewModel? LoggingControlViewModel { get; }

    private bool _saveEnabled;
    private bool _startUpChecked;
    private bool _isInProgress;
    private string _loginMessage;
    private string _appID;
    private readonly LastfmAPIClient _apiClient;
    private readonly AbstractConfigFileIO<SaveData> _saveDataFileIO;
    private readonly IRPCLogger _logger;

    public bool SaveEnabled
    {
        get => _saveEnabled;
        set => this.RaiseAndSetIfChanged(ref _saveEnabled, value);
    }

    public bool StartUpChecked
    {
        get => _startUpChecked;
        set => this.RaiseAndSetIfChanged(ref _startUpChecked, value);
    }

    public bool IsInProgress
    {
        get => _isInProgress;
        set => this.RaiseAndSetIfChanged(ref _isInProgress, value);
    }

    public string LoginMessage
    {
        get => _loginMessage;
        set
        {
            this.RaiseAndSetIfChanged(ref _loginMessage, value);
            this.RaisePropertyChanged(nameof(IsLoggedIn));
        }
    }

    public bool IsLoggedIn
    {
        get => _saveDataFileIO.ConfigData.UserAccount.SessionKey == "" || _saveDataFileIO.ConfigData.UserAccount.Username == "";
    }

    [RegularExpression($"{AppIDRegExp}", ErrorMessage = "Please enter a valid Discord App ID.")]
    public string AppID
    {
        get => _appID;
        set
        {
            this.RaiseAndSetIfChanged(ref _appID, value);
            SaveEnabled = Regex.IsMatch(value, AppIDRegExp);
        }
    }

    public SettingsViewModel(
        LastfmAPIClient apiClient,
        ILoggingControlViewModel loggingControlViewModel,
        AbstractConfigFileIO<SaveData> saveDataFileIO,
        IRPCLogger logger)
    {
        _apiClient = apiClient;
        _saveDataFileIO = saveDataFileIO;
        _logger = logger;
        LoggingControlViewModel = loggingControlViewModel as SettingsConsoleViewModel;

        LaunchOnStartup = ReactiveCommand.Create<bool>(SetLaunchOnStartup);
        LastfmLogin = ReactiveCommand.CreateFromTask<bool>(SetLastfmLogin);
        SaveAppID = ReactiveCommand.Create(SaveDiscordAppID);

        PaneName = "Settings";

        if (OperatingSystem.CurrentOS == OSEnum.Windows)
        {
            StartUpVisible = true;
            StartUpChecked = WinRegistry.CheckRegistryExists();
        }

        AppID = _saveDataFileIO.ConfigData.AppID;
        LoginMessage = IsLoggedIn ? "Not logged in" : $"Logged in as {_saveDataFileIO.ConfigData.UserAccount.Username}";
    }

    private void SaveDiscordAppID()
    {
        SaveData data = new SaveData(_saveDataFileIO.ConfigData)
        {
            AppID = AppID
        };
        _saveDataFileIO.SaveConfigData(data);
    }

    private static void SetLaunchOnStartup(bool startUpValue)
    {
        WinRegistry.SetRegistry(startUpValue);
    }

    private async Task SetLastfmLogin(bool logIn)
    {
        IsInProgress = true;

        if (logIn)
        {
            await LogUserIn();
        }
        else
        {
            LogUserOut();
        }

        IsInProgress = false;
    }

    private void LogUserOut()
    {
        SaveData data = new SaveData(_saveDataFileIO.ConfigData)
        {
            UserAccount = new SaveData.Account()
        };
        _saveDataFileIO.SaveConfigData(data);

        LoginMessage = "Not logged in";
    }

    private async Task LogUserIn()
    {
        try
        {
            TokenResponse token = await _apiClient.GetToken();
            Utilities.Utilities.OpenWebpage($"https://www.last.fm/api/auth/?api_key={Utilities.Utilities.LastfmAPIKey}&token={token.Token}");
            SessionResponse? sessionResponse = await _apiClient.GetSession(token.Token);

            SaveData data = new SaveData(_saveDataFileIO.ConfigData)
            {
                UserAccount = new SaveData.Account
                {
                    SessionKey = sessionResponse.LfmSession.SessionKey,
                    Username = sessionResponse.LfmSession.Username
                }
            };
            _saveDataFileIO.SaveConfigData(data);

            LoginMessage = $"Logged in as {_saveDataFileIO.ConfigData.UserAccount.Username}";
        }
        catch (Exception e)
        {
            LoginMessage = "";
            _logger.Error(e.Message);
        }
    }
}
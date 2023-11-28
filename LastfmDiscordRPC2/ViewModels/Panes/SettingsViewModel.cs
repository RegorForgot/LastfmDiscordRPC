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
using LastfmDiscordRPC2.ViewModels.Controls;
using ReactiveUI;

namespace LastfmDiscordRPC2.ViewModels.Panes;

public sealed class SettingsViewModel : AbstractPaneViewModel
{
    private const string AppIDRegExp = @"^\d{17,21}$";
    private const string NotLoggedIn = "Not logged in";

    public ReactiveCommand<bool, Unit> LaunchOnStartup { get; }
    public ReactiveCommand<bool, Unit> LastfmLogin { get; }
    public ReactiveCommand<Unit, Unit> SaveAppID { get; }
    public bool StartUpVisible { get; set; }
    public override string Name { get => "Settings"; }
    public AbstractLoggingControlViewModel LoggingControlViewModel { get; }

    private bool _saveEnabled;
    private bool _startUpChecked;
    private bool _isInProgress;
    private string _loginMessage;
    private string _appID;
    private readonly LastfmAPIClient _apiClient;
    private readonly AbstractConfigFileIO<SaveData> _saveDataFileIO;
    private readonly AbstractLoggingService _loggingService;

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

    private bool IsLoggedIn
    {
        get => _saveDataFileIO.ConfigData.UserAccount.SessionKey == "" || _saveDataFileIO.ConfigData.UserAccount.Username == "";
    }
    
    private string LoggedIn
    {
        get => $"Logged in as {_saveDataFileIO.ConfigData.UserAccount.Username}";
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
        AbstractLoggingControlViewModel loggingControlViewModel,
        AbstractConfigFileIO<SaveData> saveDataFileIO,
        AbstractLoggingService loggingService)
    {
        _apiClient = apiClient;
        _saveDataFileIO = saveDataFileIO;
        _loggingService = loggingService;
        LoggingControlViewModel = loggingControlViewModel;

        LaunchOnStartup = ReactiveCommand.Create<bool>(SetLaunchOnStartup);
        LastfmLogin = ReactiveCommand.CreateFromTask<bool>(SetLastfmLogin);
        SaveAppID = ReactiveCommand.Create(SaveDiscordAppID);

        if (OperatingSystem.CurrentOS == OSEnum.Windows)
        {
            StartUpVisible = true;
            StartUpChecked = WinRegistry.CheckRegistryExists();
        }

        AppID = _saveDataFileIO.ConfigData.AppID;
        LoginMessage = IsLoggedIn ? NotLoggedIn : LoggedIn;
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

        LoginMessage = NotLoggedIn;
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

            LoginMessage = LoggedIn;
        }
        catch (Exception e)
        {
            LoginMessage = NotLoggedIn;
            _loggingService.Error(e.Message);
        }
    }
}
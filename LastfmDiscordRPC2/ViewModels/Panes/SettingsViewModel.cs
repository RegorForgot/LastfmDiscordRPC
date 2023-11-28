using System;
using System.ComponentModel.DataAnnotations;
using System.Reactive;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LastfmDiscordRPC2.Logging;
using LastfmDiscordRPC2.Models;
using LastfmDiscordRPC2.Models.API;
using LastfmDiscordRPC2.Models.Responses;
using ReactiveUI;
using static LastfmDiscordRPC2.Models.Utilities.SaveAppData;

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
        get => SavedData.UserAccount.SessionKey == "" || SavedData.UserAccount.Username == "";
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

    public SettingsViewModel(LastfmAPIClient apiClient, ILoggingControlViewModel loggingControlViewModel, IRPCLogger logger)
    {
        _apiClient = apiClient;
        _logger = logger;
        LoggingControlViewModel = loggingControlViewModel as SettingsConsoleViewModel;
        
        LaunchOnStartup = ReactiveCommand.Create<bool>(SetLaunchOnStartup);
        LastfmLogin = ReactiveCommand.CreateFromTask<bool>(SetLastfmLogin);
        SaveAppID = ReactiveCommand.Create(SaveDiscordAppID);
        
        PaneName = "Settings";
        
        if (Utilities.OS == OSPlatform.Windows)
        {
            StartUpVisible = true;
            StartUpChecked = Utilities.CheckRegistryExists();
        }

        AppID = SavedData.AppID;
        LoginMessage = IsLoggedIn ? "Not logged in" : $"Logged in as {SavedData.UserAccount.Username}";
    }

    private void SaveDiscordAppID()
    {
        ApplicationData data = new ApplicationData(SavedData)
        {
            AppID = AppID
        };
        SaveData(data);
    }

    private void SetLaunchOnStartup(bool startUpValue)
    {
        Utilities.SetRegistry(startUpValue);
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
        ApplicationData data = new ApplicationData(SavedData)
        {
            UserAccount = new ApplicationData.Account()
        };
        SaveData(data);

        LoginMessage = "Not logged in";
    }

    private async Task LogUserIn()
    {
        try
        {
            TokenResponse token = await _apiClient.GetToken();
            Utilities.OpenWebpage(@$"https://www.last.fm/api/auth/?api_key={Utilities.APIKey}&token={token.Token}");
            
            SessionResponse? sessionResponse = await _apiClient.GetSession(token.Token);
            
            ApplicationData data = new ApplicationData(SavedData)
            {
                UserAccount = new ApplicationData.Account
                {
                    SessionKey = sessionResponse.LfmSession.SessionKey,
                    Username = sessionResponse.LfmSession.Username
                }
            };
            SaveData(data);

            LoginMessage = $"Logged in as {SavedData.UserAccount.Username}";
        }
        catch (Exception e)
        {
            LoginMessage = e.Message;
        }
    }
}
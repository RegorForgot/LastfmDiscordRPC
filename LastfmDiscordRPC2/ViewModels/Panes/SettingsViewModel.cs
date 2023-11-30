using System;
using System.ComponentModel.DataAnnotations;
using System.Reactive;
using System.Threading.Tasks;
using LastfmDiscordRPC2.Enums;
using LastfmDiscordRPC2.IO;
using LastfmDiscordRPC2.Logging;
using LastfmDiscordRPC2.Models;
using LastfmDiscordRPC2.Models.API;
using LastfmDiscordRPC2.Models.Responses;
using LastfmDiscordRPC2.Utilities;
using LastfmDiscordRPC2.ViewModels.Controls;
using ReactiveUI;
using static System.Text.RegularExpressions.Regex;

namespace LastfmDiscordRPC2.ViewModels.Panes;

public sealed class SettingsViewModel : AbstractPaneViewModel
{
    private const string AppIDRegExp = @"^\d{17,21}$";
    private const string NotLoggedIn = "Not logged in";

    private bool _startUpChecked;
    private string _loginMessage;
    private string _appID;
    private bool _appIDError;

    [RegularExpression($"{AppIDRegExp}", ErrorMessage = "Please enter a valid Discord App ID.")]
    public string AppID
    {
        get => _appID;
        set
        {
            this.RaiseAndSetIfChanged(ref _appID, value);
            AppIDError = IsMatch(value, AppIDRegExp);
        }
    }
    
    public bool StartUpChecked
    {
        get => _startUpChecked;
        set => this.RaiseAndSetIfChanged(ref _startUpChecked, value);
    }

    public string LoginMessage
    {
        get => _loginMessage;
        set => this.RaiseAndSetIfChanged(ref _loginMessage, value);
    }
    
    public bool AppIDError
    {
        get => _appIDError;
        set
        {
            this.RaiseAndSetIfChanged(ref _appIDError, value);
            this.RaisePropertyChanged(nameof(SaveEnabled));
        }
    }

    public ReactiveCommand<bool, Unit> LaunchOnStartup { get; }
    public ReactiveCommand<bool, Unit> LastfmLogin { get; }
    public ReactiveCommand<Unit, Unit> SaveAppID { get; }
    
    public bool StartUpVisible { get; set; }
    public LoggingControlViewModel LoggingControlViewModel { get; }
            
    private readonly LastfmAPIService _lastfmService;
    private readonly SaveCfgIOService _saveCfgService;
    private readonly LoggingService _loggingService;

    public override string Name => "Settings";
    private string LoggedIn => $"Logged in as {_saveCfgService.SaveCfg.UserAccount.Username}";
    public bool SaveEnabled => AppIDError && !State.IsRichPresenceActivated;

    public SettingsViewModel(
        LastfmAPIService lastfmService,
        LoggingControlViewModel loggingControlViewModel,
        SaveCfgIOService saveCfgService,
        LoggingService loggingService,
        CurrentState state) : base (state)
    {
        _lastfmService = lastfmService;
        _saveCfgService = saveCfgService;
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

        AppID = _saveCfgService.SaveCfg.UserRPCCfg.AppID;
        LoginMessage = State.IsLoggedIn ? LoggedIn : NotLoggedIn;
    }

    private void SaveDiscordAppID()
    {
        _saveCfgService.SaveCfg.UserRPCCfg.AppID = AppID;
        _saveCfgService.SaveConfigData();
    }

    private static void SetLaunchOnStartup(bool startUpValue)
    {
        WinRegistry.SetRegistry(startUpValue);
    }

    private async Task SetLastfmLogin(bool logIn)
    {
        State.IsLoginInProgress = true;

        if (logIn)
        {
            await LogUserIn();
        }
        else
        {
            LogUserOut();
        }

        State.IsLoginInProgress = false;
    }

    private void LogUserOut()
    {
        _saveCfgService.SaveCfg.UserAccount = new SaveCfg.Account();
        _saveCfgService.SaveConfigData();
        
        LoginMessage = NotLoggedIn;
    }

    private async Task LogUserIn()
    {
        try
        {
            TokenResponse token = await _lastfmService.GetToken();
            Utilities.Utilities.OpenWebpage($"https://www.last.fm/api/auth/?api_key={Utilities.Utilities.LastfmAPIKey}&token={token.Token}");
            SessionResponse? sessionResponse = await _lastfmService.GetSession(token.Token);

            _saveCfgService.SaveCfg.UserAccount = new SaveCfg.Account
            {
                SessionKey = sessionResponse.LfmSession.SessionKey,
                Username = sessionResponse.LfmSession.Username
            };
            
            _saveCfgService.SaveConfigData();

            LoginMessage = LoggedIn;
        }
        catch (Exception e)
        {
            LoginMessage = NotLoggedIn;
            _loggingService.Error(e.Message);
        }
    }
}
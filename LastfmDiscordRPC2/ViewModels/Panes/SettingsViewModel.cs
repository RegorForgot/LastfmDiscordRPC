using System;
using System.Reactive;
using System.Threading.Tasks;
using LastfmDiscordRPC2.DataTypes;
using LastfmDiscordRPC2.IO;
using LastfmDiscordRPC2.Logging;
using LastfmDiscordRPC2.Models.API;
using LastfmDiscordRPC2.Models.Responses;
using LastfmDiscordRPC2.Utilities;
using LastfmDiscordRPC2.ViewModels.Setter;
using ReactiveUI;
using static System.Text.RegularExpressions.Regex;
using static LastfmDiscordRPC2.DataTypes.SaveVars;

namespace LastfmDiscordRPC2.ViewModels.Panes;

public sealed class SettingsViewModel : AbstractPaneViewModel, ISettableViewModel
{
    private const string AppIDRegExp = @"^\d{17,21}$";
    private const string NotLoggedIn = "Not logged in";

    private bool _isStartUpChecked;
    private bool _isLoginInProgress;
    private bool _expiryMode;
    private bool _closeToTray;
    private bool _isAppIDError;
    private bool _isExpiryTimeError;
    
    private string _loginMessage;
    private string _appID;
    
    private TimeSpan _presenceExpiryTime;

    public string AppID
    {
        get => _appID;
        set
        {
            this.RaiseAndSetIfChanged(ref _appID, value);
            IsAppIDError = !IsMatch(value ?? Empty, AppIDRegExp);
        }
    }

    public bool CloseToTray
    {
        get => _closeToTray;
        set => this.RaiseAndSetIfChanged(ref _closeToTray, value);
    }
    
    public bool IsStartUpChecked
    {
        get => _isStartUpChecked;
        set => this.RaiseAndSetIfChanged(ref _isStartUpChecked, value);
    }
    
    public bool ExpiryMode
    {
        get => _expiryMode;
        set => this.RaiseAndSetIfChanged(ref _expiryMode, value);
    }

    public string LoginMessage
    {
        get => _loginMessage;
        set => this.RaiseAndSetIfChanged(ref _loginMessage, value);
    }

    public bool IsLoginInProgress
    {
        get => _isLoginInProgress;
        set
        {
            this.RaiseAndSetIfChanged(ref _isLoginInProgress, value);
            Context.UpdateLogin();
        }
    }
    
    public TimeSpan PresenceExpiryTime
    {
        get => _presenceExpiryTime;
        set
        {
            this.RaiseAndSetIfChanged(ref _presenceExpiryTime, value);
            IsExpiryTimeError = value == TimeSpan.Zero;
        }
    }
    
    public bool IsExpiryTimeError
    {
        get => _isExpiryTimeError;
        private set => this.RaiseAndSetIfChanged(ref _isExpiryTimeError, value);
    }

    public bool IsAppIDError
    {
        get => _isAppIDError;
        private set => this.RaiseAndSetIfChanged(ref _isAppIDError, value);
    }

    public ReactiveCommand<bool, Unit> LaunchOnStartupCmd { get; }
    public ReactiveCommand<bool, Unit> LastfmLoginCmd { get; }
    public ReactiveCommand<bool, Unit> SetExpiryModeCmd { get; }
    public ReactiveCommand<bool, Unit> SetCloseToTrayCmd { get; }
    public ReactiveCommand<Unit, Unit> SaveAppIDCmd { get; }
    public ReactiveCommand<Unit, Unit> ResetAppIDCmd { get; }
    public ReactiveCommand<Unit, Unit> ResetExpiryTimeCmd { get; }
    public ReactiveCommand<Unit, Unit> SaveExpiryTimeCmd { get; }
    
    public bool StartUpVisible { get; set; }

    private readonly LastfmAPIService _lastfmService;
    private readonly SaveCfgIOService _saveCfgService;
    private readonly LoggingService _loggingService;

    public override string Name => "Settings";
    private string LoggedIn => $"Logged in as {_saveCfgService.SaveCfg.UserAccount.Username}";

    public SettingsViewModel(
        LastfmAPIService lastfmService,
        SaveCfgIOService saveCfgService,
        LoggingService loggingService,
        UIContext context) : base(context)
    {
        _lastfmService = lastfmService;
        _saveCfgService = saveCfgService;
        _loggingService = loggingService;

        LaunchOnStartupCmd = ReactiveCommand.Create<bool>(SetLaunchOnStartup);
        LastfmLoginCmd = ReactiveCommand.CreateFromTask<bool>(SetLastfmLogin);
        SetExpiryModeCmd = ReactiveCommand.Create<bool>(SetExpiryMode);
        SetCloseToTrayCmd = ReactiveCommand.Create<bool>(SetCloseToTray);
        SaveAppIDCmd = ReactiveCommand.Create(SaveDiscordAppID);
        ResetAppIDCmd = ReactiveCommand.Create(ResetDiscordAppID);
        ResetExpiryTimeCmd = ReactiveCommand.Create(ResetExpiryTime);
        SaveExpiryTimeCmd = ReactiveCommand.Create(SaveExpiryTime);

        if (RuntimeLocator.CurrentOS == OSRuntimes.Windows)
        {
            StartUpVisible = true;
            IsStartUpChecked = WinRegistry.CheckRegistryExists();
        }

        SetProperties();
        LoginMessage = Context.IsLoggedIn ? LoggedIn : NotLoggedIn;
    }

    private void SaveExpiryTime()
    {
        _saveCfgService.SaveCfg.UserRPCCfg.ExpiryTime = PresenceExpiryTime;
        _saveCfgService.SaveConfigData();
    }

    private void SaveDiscordAppID()
    {
        _saveCfgService.SaveCfg.UserRPCCfg.AppID = AppID;
        _saveCfgService.SaveConfigData();
    }

    private void ResetDiscordAppID()
    {
        AppID = DefaultAppID;
        SaveDiscordAppID();
    }
    
    private void ResetExpiryTime()
    {
        PresenceExpiryTime = DefaultExpiryTime;
        SaveExpiryTime();
    }

    private void SetCloseToTray(bool closeToTray)
    {
        CloseToTray = closeToTray;
        _saveCfgService.SaveCfg.CloseToTray = closeToTray;
        _saveCfgService.SaveConfigData();
    }
    
    private static void SetLaunchOnStartup(bool startUpValue)
    {
        WinRegistry.SetRegistry(startUpValue);
    }

    private async Task SetLastfmLogin(bool logIn)
    {
        IsLoginInProgress = true;

        if (logIn)
        {
            await LogUserIn();
        }
        else
        {
            LogUserOut();
        }

        IsLoginInProgress = false;
    }
    
    private void SetExpiryMode(bool expiryMode)
    {
        ExpiryMode = expiryMode;
        _saveCfgService.SaveCfg.UserRPCCfg.ExpiryMode = expiryMode;
        _saveCfgService.SaveConfigData();
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
            TokenResponse token = await _lastfmService.GetToken(); ;
            SessionResponse sessionResponse = await _lastfmService.GetSession(token.Token);

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

    public void SetProperties()
    {
        AppID = _saveCfgService.SaveCfg.UserRPCCfg.AppID;
        ExpiryMode = _saveCfgService.SaveCfg.UserRPCCfg.ExpiryMode;
        PresenceExpiryTime = _saveCfgService.SaveCfg.UserRPCCfg.ExpiryTime;
        CloseToTray = _saveCfgService.SaveCfg.CloseToTray;
    }
}
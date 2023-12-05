using System;
using System.ComponentModel.DataAnnotations;
using System.Reactive;
using System.Threading.Tasks;
using LastfmDiscordRPC2.DataTypes;
using LastfmDiscordRPC2.IO;
using LastfmDiscordRPC2.Logging;
using LastfmDiscordRPC2.Models.API;
using LastfmDiscordRPC2.Models.Responses;
using LastfmDiscordRPC2.Utilities;
using LastfmDiscordRPC2.ViewModels.Controls;
using ReactiveUI;
using static System.Text.RegularExpressions.Regex;
using static LastfmDiscordRPC2.Utilities.URIOpen;

namespace LastfmDiscordRPC2.ViewModels.Panes;

public sealed class SettingsViewModel : AbstractPaneViewModel, IUpdatableViewModel
{
    private const string AppIDRegExp = @"^\d{17,21}$";
    private const string NotLoggedIn = "Not logged in";

    private bool _isStartUpChecked;
    private bool _isLoginInProgress;
    private bool _isAppIDError;
    private bool _isExpiryTimeError;
    private string _loginMessage;
    private string _appID;
    private TimeSpan _presenceExpiryTime;

    [RegularExpression($"{AppIDRegExp}", ErrorMessage = "Please enter a valid Discord App ID.")]
    public string AppID
    {
        get => _appID;
        set
        {
            if (value is null)
            {
                return;
            }

            this.RaiseAndSetIfChanged(ref _appID, value);
            IsAppIDError = IsMatch(value, AppIDRegExp);
        }
    }

    public bool IsStartUpChecked
    {
        get => _isStartUpChecked;
        set => this.RaiseAndSetIfChanged(ref _isStartUpChecked, value);
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
            Context.UpdateProperties();
        }
    }
    
    public TimeSpan PresenceExpiryTime
    {
        get => _presenceExpiryTime;
        set
        {
            this.RaiseAndSetIfChanged(ref _presenceExpiryTime, value);
            IsExpiryTimeError = value != TimeSpan.Zero;
        }
    }
    
    public bool IsExpiryTimeError
    {
        get => _isExpiryTimeError;
        private set
        {
            this.RaiseAndSetIfChanged(ref _isExpiryTimeError, value);
            this.RaisePropertyChanged(nameof(CanSaveExpiryTime));
        }
    }

    public bool IsAppIDError
    {
        get => _isAppIDError;
        private set
        {
            this.RaiseAndSetIfChanged(ref _isAppIDError, value);
            this.RaisePropertyChanged(nameof(CanSaveAppID));
        }
    }

    public ReactiveCommand<bool, Unit> LaunchOnStartupCmd { get; }
    public ReactiveCommand<bool, Unit> LastfmLoginCmd { get; }
    public ReactiveCommand<Unit, Unit> SaveAppIDCmd { get; }
    public ReactiveCommand<Unit, Unit> ResetAppIDCmd { get; }
    public ReactiveCommand<Unit, Unit> SaveExpiryTimeCmd { get; }
    public PreviewSettingControlViewModel PreviewSettingControlViewModel { get; }
    
    public bool StartUpVisible { get; set; }

    private readonly LastfmAPIService _lastfmService;
    private readonly SaveCfgIOService _saveCfgService;
    private readonly LoggingService _loggingService;

    public override string Name => "Settings";
    private string LoggedIn => $"Logged in as {_saveCfgService.SaveCfg.UserAccount.Username}";

    public bool CanLogOut => !Context.IsRichPresenceActivated && !IsLoginInProgress;
    public bool CanSaveAppID => IsAppIDError && !Context.HasRichPresenceActivated;
    public bool CanSaveExpiryTime => IsExpiryTimeError && !Context.IsRichPresenceActivated;
    public bool CanReset => !Context.HasRichPresenceActivated;


    public SettingsViewModel(
        LastfmAPIService lastfmService,
        PreviewSettingControlViewModel previewSettingControlViewModel,
        SaveCfgIOService saveCfgService,
        LoggingService loggingService,
        UIContext context) : base(context)
    {
        PreviewSettingControlViewModel = previewSettingControlViewModel;
        _lastfmService = lastfmService;
        _saveCfgService = saveCfgService;
        _loggingService = loggingService;

        LaunchOnStartupCmd = ReactiveCommand.Create<bool>(SetLaunchOnStartup);
        LastfmLoginCmd = ReactiveCommand.CreateFromTask<bool>(SetLastfmLogin);
        SaveAppIDCmd = ReactiveCommand.Create(SaveDiscordAppID);
        ResetAppIDCmd = ReactiveCommand.Create(ResetDiscordAppID);
        SaveExpiryTimeCmd = ReactiveCommand.Create(SaveExpiryTime);

        if (OperatingSystem.CurrentOS == OSEnum.Windows)
        {
            StartUpVisible = true;
            IsStartUpChecked = WinRegistry.CheckRegistryExists();
        }

        SetProperties();
        LoginMessage = Context.IsLoggedIn ? LoggedIn : NotLoggedIn;
    }
    
    private void SaveExpiryTime()
    {
        _saveCfgService.SaveCfg.UserRPCCfg.SleepTime = PresenceExpiryTime;
        _saveCfgService.SaveConfigData();
    }

    private void SaveDiscordAppID()
    {
        _saveCfgService.SaveCfg.UserRPCCfg.AppID = AppID;
        _saveCfgService.SaveConfigData();
    }

    private void ResetDiscordAppID()
    {
        AppID = SaveCfg.DefaultAppID;
        SaveDiscordAppID();
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

    public void UpdateProperties()
    {
        SetProperties();
        PreviewSettingControlViewModel.SetProperties();
        this.RaisePropertyChanged(nameof(CanLogOut));
        this.RaisePropertyChanged(nameof(CanSaveAppID));
        this.RaisePropertyChanged(nameof(CanReset));
        this.RaisePropertyChanged(nameof(CanSaveExpiryTime));
    }

    private void SetProperties()
    {
        AppID = _saveCfgService.SaveCfg.UserRPCCfg.AppID;
        PresenceExpiryTime = _saveCfgService.SaveCfg.UserRPCCfg.SleepTime;
    }
}
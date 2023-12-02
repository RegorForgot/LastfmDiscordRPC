using System;
using LastfmDiscordRPC2.IO;
using ReactiveUI;

namespace LastfmDiscordRPC2.ViewModels;

public class UIContext : ReactiveObject
{
    private readonly SaveCfgIOService _saveCfg;
    public Lazy<IViewModelUpdater> ViewModelUpdater { get; set; }

    public UIContext(SaveCfgIOService saveCfg, Lazy<IViewModelUpdater> viewModelUpdater)
    {
        _saveCfg = saveCfg;
        ViewModelUpdater = viewModelUpdater;
    }

    private bool _isLoginInProgress;
    private bool _isRichPresenceActivated;
    private bool _isAppIDError;

    public bool IsLoginInProgress
    {
        get => _isLoginInProgress;
        set
        {
            this.RaiseAndSetIfChanged(ref _isLoginInProgress, value);
            this.RaisePropertyChanged(nameof(IsLoggedIn));
            this.RaisePropertyChanged(nameof(CanLogOut));
        }
    }

    public bool IsRichPresenceActivated
    {
        get => _isRichPresenceActivated;
        set
        {
            this.RaiseAndSetIfChanged(ref _isRichPresenceActivated, value);
            this.RaisePropertyChanged(nameof(CanLogOut));
            this.RaisePropertyChanged(nameof(CanSave));
        }
    }

    public bool IsAppIDError
    {
        get => _isAppIDError;
        set
        {
            this.RaiseAndSetIfChanged(ref _isAppIDError, value);
            this.RaisePropertyChanged(nameof(CanSave));
        }
    }

    public bool IsLoggedIn => _saveCfg.SaveCfg.UserAccount.IsValid();
    public bool CanLogOut => !IsRichPresenceActivated && !IsLoginInProgress;
    public bool CanSave => !IsRichPresenceActivated && IsAppIDError;
}
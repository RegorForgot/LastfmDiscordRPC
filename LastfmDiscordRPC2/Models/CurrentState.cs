using LastfmDiscordRPC2.IO;
using ReactiveUI;

namespace LastfmDiscordRPC2.Models;

public class CurrentState : ReactiveObject
{
    private readonly SaveCfgIOService _saveCfg;


    public CurrentState(SaveCfgIOService saveCfg)
    {
        _saveCfg = saveCfg;
    }
    
    private bool _isLoginInProgress;
    private bool _isRichPresenceActivated;

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
        }
    }

    public bool IsLoggedIn => _saveCfg.SaveCfg.UserAccount.IsValid();
    public bool CanLogOut => !IsRichPresenceActivated && !IsLoginInProgress;
}
using System;
using LastfmDiscordRPC2.IO;
using LastfmDiscordRPC2.ViewModels.Update;
using ReactiveUI;

namespace LastfmDiscordRPC2.ViewModels;

public class UIContext : ReactiveObject
{
    private readonly SaveCfgIOService _saveCfg;
    private readonly Lazy<IViewModelSetter> _viewModelUpdater; 

    public UIContext(SaveCfgIOService saveCfg, Lazy<IViewModelSetter> viewModelUpdater)
    {
        _saveCfg = saveCfg;
        _viewModelUpdater = viewModelUpdater;
    }

    private bool _isRichPresenceActivated;
    private bool _hasRichPresenceActivated;

    public bool IsRichPresenceActivated
    {
        get => _isRichPresenceActivated;
        set
        {
            _viewModelUpdater.Value.SetAllViewModels();
            this.RaiseAndSetIfChanged(ref _isRichPresenceActivated, value);
            if (_isRichPresenceActivated)
            {
                HasRichPresenceActivated = true;
            }
        }
    }

    public bool HasRichPresenceActivated
    {
        get => _hasRichPresenceActivated;
        set => this.RaiseAndSetIfChanged(ref _hasRichPresenceActivated, value);
    }

    public bool IsLoggedIn => _saveCfg.SaveCfg.UserAccount.IsValid();

    public void UpdateLogin()
    {
        this.RaisePropertyChanged(nameof(IsLoggedIn));
    }
}
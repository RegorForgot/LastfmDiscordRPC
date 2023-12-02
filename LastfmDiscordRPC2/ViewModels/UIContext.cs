using System;
using System.Reflection;
using LastfmDiscordRPC2.IO;
using ReactiveUI;

namespace LastfmDiscordRPC2.ViewModels;

public class UIContext : ReactiveObject
{
    private readonly SaveCfgIOService _saveCfg;
    private Lazy<IViewModelUpdater> _viewModelUpdater; 

    public UIContext(SaveCfgIOService saveCfg, Lazy<IViewModelUpdater> viewModelUpdater)
    {
        _saveCfg = saveCfg;
        _viewModelUpdater = viewModelUpdater;
    }

    private bool _isRichPresenceActivated;

    public bool IsRichPresenceActivated
    {
        get => _isRichPresenceActivated;
        set
        {
            this.RaiseAndSetIfChanged(ref _isRichPresenceActivated, value);
            _viewModelUpdater.Value.UpdateAllViewModels();
        }
    }

    public bool IsLoggedIn => _saveCfg.SaveCfg.UserAccount.IsValid();
    
    public void UpdateProperties() => this.RaisePropertyChanged(nameof(IsLoggedIn));
}
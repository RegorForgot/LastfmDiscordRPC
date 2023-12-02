using System;
using System.Reflection;
using LastfmDiscordRPC2.IO;
using ReactiveUI;

namespace LastfmDiscordRPC2.ViewModels;

public class UIContext : ReactiveObject
{
    private readonly SaveCfgIOService _saveCfg;
    private readonly Lazy<IViewModelUpdater> _viewModelUpdater; 

    public UIContext(SaveCfgIOService saveCfg, Lazy<IViewModelUpdater> viewModelUpdater)
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
            this.RaiseAndSetIfChanged(ref _isRichPresenceActivated, value);
            if (_isRichPresenceActivated)
            {
                HasRichPresenceActivated = true;
            }
            _viewModelUpdater.Value.UpdateAllViewModels();
        }
    }

    public bool HasRichPresenceActivated
    {
        get => _hasRichPresenceActivated;
        set => this.RaiseAndSetIfChanged(ref _hasRichPresenceActivated, value);
    }

    public bool IsLoggedIn => _saveCfg.SaveCfg.UserAccount.IsValid();
    
    public void UpdateProperties()
    {
        this.RaisePropertyChanged(nameof(IsLoggedIn));
        _viewModelUpdater.Value.UpdateAllViewModels();
    }
}
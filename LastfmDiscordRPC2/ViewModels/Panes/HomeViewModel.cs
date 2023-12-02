using System.Reactive;
using LastfmDiscordRPC2.Models.RPC;
using ReactiveUI;

namespace LastfmDiscordRPC2.ViewModels.Panes;

public sealed class HomeViewModel : AbstractPaneViewModel, IUpdatableViewModel
{
    private readonly IPresenceService _presenceService;
    public override string Name => "Home";
    public ReactiveCommand<bool, Unit> SetPresence { get; }
    
    public HomeViewModel(
        IPresenceService presenceService, 
        UIContext uiContext) : base(uiContext)
    {
        _presenceService = presenceService;
        SetPresence = ReactiveCommand.Create<bool>(StartPresenceCommand);
    }

    private void StartPresenceCommand(bool activated)
    {
        if (activated)
        {
            _presenceService.ClearPresence();
        }
        else
        {
            _presenceService.SetPresence();
        }
        
        UIContext.IsRichPresenceActivated = !activated;
    }
    
    public void UpdateProperties()
    {
        throw new System.NotImplementedException();
    }
}
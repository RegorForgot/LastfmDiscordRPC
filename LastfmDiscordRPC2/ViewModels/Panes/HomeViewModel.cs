using System.Reactive;
using LastfmDiscordRPC2.Models.RPC;
using ReactiveUI;

namespace LastfmDiscordRPC2.ViewModels.Panes;

public sealed class HomeViewModel : AbstractPaneViewModel
{
    private readonly IPresenceService _presenceService;
    public override string Name { get => "Home"; }
    public ReactiveCommand<Unit, Unit> StartPresence { get; }
    
    public HomeViewModel(IPresenceService presenceService)
    {
        _presenceService = presenceService;
        StartPresence = ReactiveCommand.Create(StartPresenceCommand);
    }

    public void StartPresenceCommand()
    {
        _presenceService.SetPresence();
    }
}
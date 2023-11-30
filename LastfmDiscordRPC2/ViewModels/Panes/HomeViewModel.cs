using System.Reactive;
using LastfmDiscordRPC2.Models;
using LastfmDiscordRPC2.Models.RPC;
using ReactiveUI;

namespace LastfmDiscordRPC2.ViewModels.Panes;

public sealed class HomeViewModel : AbstractPaneViewModel
{
    private readonly IPresenceService _presenceService;
    public override string Name => "Home";
    public ReactiveCommand<Unit, Unit> StartPresence { get; }
    
    public HomeViewModel(IPresenceService presenceService, CurrentState state) : base(state)
    {
        _presenceService = presenceService;
        StartPresence = ReactiveCommand.Create(StartPresenceCommand);
    }

    private void StartPresenceCommand()
    {
        _presenceService.SetPresence();
        State.IsRichPresenceActivated = true;
    }
}
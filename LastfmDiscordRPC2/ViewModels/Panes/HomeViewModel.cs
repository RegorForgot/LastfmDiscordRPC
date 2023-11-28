using System.Reactive;
using LastfmDiscordRPC2.Models.RPC;
using ReactiveUI;

namespace LastfmDiscordRPC2.ViewModels.Panes;

public sealed class HomeViewModel : AbstractPaneViewModel
{
    private readonly PresenceSetter _presenceSetter;
    public override string Name { get => "Home"; }
    public ReactiveCommand<Unit, Unit> StartPresence { get; }
    
    public HomeViewModel(PresenceSetter presenceSetter)
    {
        _presenceSetter = presenceSetter;
        StartPresence = ReactiveCommand.Create(StartPresenceCommand);
    }

    public void StartPresenceCommand()
    {
        _presenceSetter.UpdatePresence();
    }
}
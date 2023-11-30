using LastfmDiscordRPC2.Models;

namespace LastfmDiscordRPC2.ViewModels.Panes;

public abstract class AbstractPaneViewModel : AbstractViewModel
{
    public CurrentState State { get; }

    protected AbstractPaneViewModel(CurrentState state)
    {
        State = state;
    }
}
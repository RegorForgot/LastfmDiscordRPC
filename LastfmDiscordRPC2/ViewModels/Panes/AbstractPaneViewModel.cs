namespace LastfmDiscordRPC2.ViewModels.Panes;

public abstract class AbstractPaneViewModel : AbstractViewModel
{
    public UIContext Context { get; }

    protected AbstractPaneViewModel(UIContext context)
    {
        Context = context;
    }
}
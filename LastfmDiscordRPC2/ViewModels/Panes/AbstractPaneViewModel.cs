namespace LastfmDiscordRPC2.ViewModels.Panes;

public abstract class AbstractPaneViewModel : AbstractViewModel
{
    public UIContext UIContext { get; }

    protected AbstractPaneViewModel(UIContext uiContext)
    {
        UIContext = uiContext;
    }
}
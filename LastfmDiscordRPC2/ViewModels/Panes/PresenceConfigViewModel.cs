using LastfmDiscordRPC2.ViewModels.Controls;

namespace LastfmDiscordRPC2.ViewModels.Panes;

public class PresenceConfigViewModel : AbstractPaneViewModel
{
    public override string Name => "PresenceConfig";
    public PreviewConfigControlViewModel PreviewConfigControlViewModel { get; }

    public PresenceConfigViewModel(UIContext context,
        PreviewConfigControlViewModel previewConfigControlViewModel) : base(context)
    {
        PreviewConfigControlViewModel = previewConfigControlViewModel;
    }
}
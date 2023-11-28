using ReactiveUI;

namespace LastfmDiscordRPC2.ViewModels.Panes;

public sealed class HomeViewModel : ReactiveObject, IPaneViewModel
{
    public string PaneName { get => "Home"; }
}
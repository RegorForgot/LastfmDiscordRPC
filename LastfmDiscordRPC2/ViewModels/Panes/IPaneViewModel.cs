using ReactiveUI;

namespace LastfmDiscordRPC2.ViewModels.Panes;

public interface IPaneViewModel : IReactiveObject
{
    public string PaneName { get; }
}
using System.Collections.ObjectModel;
using LastfmDiscordRPC2.ViewModels.Panes;
using ReactiveUI;

namespace LastfmDiscordRPC2.ViewModels;

public interface IWindowViewModel : IReactiveObject
{
    public ObservableCollection<IPaneViewModel> Children { get; }
}
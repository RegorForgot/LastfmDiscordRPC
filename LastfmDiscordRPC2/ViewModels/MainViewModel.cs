using System.Collections.Generic;
using System.Collections.ObjectModel;
using LastfmDiscordRPC2.ViewModels.Panes;
using ReactiveUI;

namespace LastfmDiscordRPC2.ViewModels;

public class MainViewModel : ReactiveObject
{
    public ObservableCollection<IPaneViewModel> Children { get; }
    
    public MainViewModel(IEnumerable<IPaneViewModel> children)
    {
        Children = new ObservableCollection<IPaneViewModel>(children);
    }
}
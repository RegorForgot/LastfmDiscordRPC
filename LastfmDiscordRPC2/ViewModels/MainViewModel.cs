using System.Collections.Generic;
using System.Collections.ObjectModel;
using LastfmDiscordRPC2.ViewModels.Panes;

namespace LastfmDiscordRPC2.ViewModels;

public class MainViewModel : AbstractViewModel
{
    public ObservableCollection<AbstractPaneViewModel> Children { get; }
    public override string Name { get; } 

    public MainViewModel(IEnumerable<AbstractPaneViewModel> children)
    {
        Children = new ObservableCollection<AbstractPaneViewModel>(children);
        Name = "Main";
    }
}
using System.Collections.Generic;
using System.Collections.ObjectModel;
using LastfmDiscordRPC2.ViewModels.Panes;
using LastfmDiscordRPC2.Views;
using ReactiveUI;

namespace LastfmDiscordRPC2.ViewModels;

public class MainViewModel : AbstractViewModel
{
    private readonly DialogWindow _window;

    private bool _isVisible = true;

    public bool IsVisible
    {
        get => _isVisible;
        set
        {
            this.RaiseAndSetIfChanged(ref _isVisible, value);
            _window.Hide();
        }
    }

    public ObservableCollection<AbstractPaneViewModel> Children { get; }
    public override string Name { get; } 

    public MainViewModel(IEnumerable<AbstractPaneViewModel> children, DialogWindow window)
    {
        _window = window;
        
        Children = new ObservableCollection<AbstractPaneViewModel>(children);
        Name = "Main";
    }
}
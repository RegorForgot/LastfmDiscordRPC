using System.Collections.ObjectModel;

namespace LastfmDiscordRPC2.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private ObservableCollection<ViewModelBase> _children;

    public ObservableCollection<ViewModelBase> Children => _children;

    public MainWindowViewModel()
    {
        _children = new ObservableCollection<ViewModelBase>
        {
            new HomeViewModel(),
            new SettingsViewModel(),
            new AboutViewModel()
        };
    }
}
using System.Collections.ObjectModel;
using LastfmDiscordRPC2.Models;

namespace LastfmDiscordRPC2.ViewModels;

public class MainViewModel : ViewModelBase
{
    public LastfmClient LastfmClient { get; }
    public ObservableCollection<ViewModelBase> Children { get; }

    public MainViewModel()
    {
        LastfmClient = new LastfmClient();
        Children = new ObservableCollection<ViewModelBase>
        {
            new HomeViewModel(this),
            new SettingsViewModel(this)
        };
    }
}
namespace LastfmDiscordRPC2.ViewModels;

public class HomeViewModel : ViewModelBase
{
    private readonly MainViewModel _mainViewModel;

    public HomeViewModel(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
    }
}
using System.Resources;
using System.Windows;
using LastfmDiscordRPC.Commands;
using LastfmDiscordRPC.Models;
using LastfmDiscordRPC.ViewModels;
using static System.Net.WebRequest;
using static LastfmDiscordRPC.Models.SaveAppData;

namespace LastfmDiscordRPC;

public partial class App : Application
{
    private DiscordClient _client;
    private ResourceManager _manager;
    private MainViewModel _mainViewModel;

    public App()
    {
        DefaultWebProxy = null;
    }
    
    protected override void OnStartup(StartupEventArgs e)
    {
        _manager = new ResourceManager(typeof(Resources.EmbeddedImages));
        _client = new DiscordClient(SavedData.AppKey);
        _mainViewModel = new MainViewModel(SavedData.Username, SavedData.APIKey, SavedData.AppKey, _client);
        MainWindow = new MainWindow(_manager)
        {
            DataContext = _mainViewModel
        };
        MainWindow.Show();
        
        base.OnStartup(e);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _client.Dispose();
        ((ActivateCommand) _mainViewModel.ActivateCommand).Dispose();
        base.OnExit(e);
    }
}
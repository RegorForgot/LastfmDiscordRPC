using System.Windows;
using LastfmDiscordRPC.Models;
using LastfmDiscordRPC.ViewModels;
using static System.Net.WebRequest;
using static LastfmDiscordRPC.Models.SaveAppData;

namespace LastfmDiscordRPC;

public partial class App : Application
{
    private DiscordClient _client;

    public App()
    {
        DefaultWebProxy = null;
    }
    
    protected override void OnStartup(StartupEventArgs e)
    {
        _client = new DiscordClient(SavedData.AppKey);
        MainWindow = new MainWindow
        {
            DataContext = new MainViewModel(SavedData.Username, SavedData.APIKey, SavedData.AppKey, _client)
        };
        MainWindow.Show();
        
        base.OnStartup(e);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _client.Dispose();
        base.OnExit(e);
    }
}
using System.Windows;
using LastfmDiscordRPC.MVVM.Models;
using LastfmDiscordRPC.MVVM.ViewModels;
using static System.Net.WebRequest;
using static LastfmDiscordRPC.MVVM.Models.SaveAppData;

namespace LastfmDiscordRPC;

public partial class App : Application
{
    private readonly DiscordClient _client;

    public App()
    {
        DefaultWebProxy = null;
        _client = new DiscordClient(SavedData.AppKey);
        InitializeComponent();
    }
    
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        MainWindow = new MainWindow
        {
            DataContext = new MainViewModel(SavedData.Username, SavedData.APIKey, SavedData.AppKey, _client)
        };

        MainWindow.Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _client.Dispose();
        base.OnExit(e);
    }
}
using System.Windows;
using LastfmDiscordRPC.MVVM.ViewModels;
using static System.Net.WebRequest;
using static LastfmDiscordRPC.MVVM.Models.SaveAppData;

namespace LastfmDiscordRPC;

public partial class App
{

    public App()
    {
        DefaultWebProxy = null;

    }
    
    protected override void OnStartup(StartupEventArgs e)
    {
        MainWindow = new MainWindow
        {
            DataContext = new MainViewModel(SavedData.Username, SavedData.APIKey)
        };
        MainWindow.Show();
    }
}
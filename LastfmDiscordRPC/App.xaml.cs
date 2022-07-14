using System.Windows;
using static System.Net.WebRequest;


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
            DataContext = new ViewModel(SaveAppData.SavedData.Username, SaveAppData.SavedData.APIKey)
        };
        MainWindow.Show();
    }
}
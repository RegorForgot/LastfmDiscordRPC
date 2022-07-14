using System.Windows;

namespace LastfmDiscordRPC;

public partial class App
{

    protected override void OnStartup(StartupEventArgs e)
    {
        MainWindow = new MainWindow
        {
            DataContext = new ViewModel(SaveAppData.SavedData.Username, SaveAppData.SavedData.APIKey)
        };
        MainWindow.Show();
    }
}
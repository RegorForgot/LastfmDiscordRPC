using System.Windows;

namespace LastfmDiscordRPC;

public partial class App : Application
{

    protected override void OnStartup(StartupEventArgs e)
    {
        this.MainWindow = new MainWindow()
        {
            DataContext = new AppViewModel(SaveAppData.SavedData.Username, 
                SaveAppData.SavedData.APIKey)
        };
        MainWindow.Show();
    }
}
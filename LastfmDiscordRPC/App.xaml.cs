using System.Windows;

namespace LastfmDiscordRPC;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        this.MainWindow = new AppWindow()
        {
            DataContext = new AppViewModel()
        };
        MainWindow.Show();
    }
}
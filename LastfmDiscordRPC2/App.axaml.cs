using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using LastfmDiscordRPC2.Models;
using LastfmDiscordRPC2.ViewModels;
using LastfmDiscordRPC2.Views;

namespace LastfmDiscordRPC2;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Line below is needed to remove Avalonia data validation.
            // Without this line you will get duplicate validations from both Avalonia and CT
            ExpressionObserver.DataValidators.RemoveAll(x => x is DataAnnotationsValidationPlugin);
            desktop.MainWindow = new MainWindowView
            {
                DataContext = new MainWindowViewModel()
            };
            Utilities.SaveAppData.SaveData(new AppData());
        }

        base.OnFrameworkInitializationCompleted();
    }
}
using Autofac;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
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
        using ILifetimeScope container = ContainerConfigurator.Configure().BeginLifetimeScope();
        IWindowViewModel windowViewModel = container.Resolve<IWindowViewModel>();
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow((MainViewModel) windowViewModel);
        }
        
        base.OnFrameworkInitializationCompleted();
    }
}
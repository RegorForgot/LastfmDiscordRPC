using System;
using Autofac;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using LastfmDiscordRPC2.ViewModels;
using LastfmDiscordRPC2.Views;

namespace LastfmDiscordRPC2;

public class App : Application
{
    private IClassicDesktopStyleApplicationLifetime? _desktop;
    private MainViewModel? _mainViewModel;
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        using ILifetimeScope container = ContainerConfigurator.Configure().BeginLifetimeScope();
        _mainViewModel = container.Resolve<MainViewModel>();
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            _desktop = desktop;
            _desktop.ShutdownMode = ShutdownMode.OnMainWindowClose;
            _desktop.MainWindow = new MainWindow(_mainViewModel);
        }
        
        base.OnFrameworkInitializationCompleted();
    }
    
    private void TrayIcon_OnClicked(object? sender, EventArgs e)
    {
        if (_desktop == null || _desktop.MainWindow is { IsVisible: true })
        {
            return;
        }
        _desktop.MainWindow?.Show();
    }
    
    private void MainItem_OnClick(object? sender, EventArgs e)
    {
        TrayIcon_OnClicked(sender, e);
    }
    
    private void ExitItem_OnClick(object? sender, EventArgs e)
    {
        _desktop?.Shutdown();
    }
}
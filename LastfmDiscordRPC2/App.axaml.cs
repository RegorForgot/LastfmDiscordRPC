using System;
using Autofac;
using Autofac.Core;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using LastfmDiscordRPC2.Models.API;
using LastfmDiscordRPC2.Models.RPC;
using LastfmDiscordRPC2.ViewModels;
using LastfmDiscordRPC2.Views;

namespace LastfmDiscordRPC2;

public class App : Application
{
    private IClassicDesktopStyleApplicationLifetime? _desktop;
    private ILifetimeScope? _lifetimeScope;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

    }

    public override void OnFrameworkInitializationCompleted()
    {
        _lifetimeScope = ContainerConfigurator.Configure().BeginLifetimeScope();
        MainViewModel mainViewModel = _lifetimeScope.Resolve<MainViewModel>();
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            _desktop = desktop;
            _desktop.ShutdownMode = ShutdownMode.OnMainWindowClose;
            _desktop.MainWindow = new MainWindow(mainViewModel);
            
            
            // Yes, I know this is not ideal.
            _lifetimeScope.Disposer.AddInstanceForDisposal(_lifetimeScope.Resolve<IDiscordClient>());
            _lifetimeScope.Disposer.AddInstanceForDisposal(_lifetimeScope.Resolve<ISignatureAPIService>());
            _lifetimeScope.Disposer.AddInstanceForDisposal(_lifetimeScope.Resolve<LastfmAPIService>());
            _lifetimeScope.Disposer.AddInstanceForDisposal(_lifetimeScope.Resolve<IPresenceService>());

            _desktop.Exit += (_, _) => _lifetimeScope.Dispose();
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

    private void OpenItem_OnClick(object? sender, EventArgs e)
    {
        TrayIcon_OnClicked(sender, e);
    }

    private void ExitItem_OnClick(object? sender, EventArgs e)
    {
        _desktop?.Shutdown();

    }
}
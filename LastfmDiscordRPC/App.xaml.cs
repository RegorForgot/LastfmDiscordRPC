using System;
using System.Windows;
using LastfmDiscordRPC.ViewModels;
using LastfmDiscordRPC.Views;
using static System.Net.WebRequest;
using static LastfmDiscordRPC.Models.SaveAppData;

namespace LastfmDiscordRPC;

public partial class App
{
    private readonly MainViewModel _mainViewModel;
    private readonly TrayIcon _trayIcon;

    public App()
    {
        DefaultWebProxy = null;
        _mainViewModel = new MainViewModel(SavedData);
        _trayIcon = new TrayIcon(this);
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        MainWindow = new MainWindow
        {
            DataContext = _mainViewModel
        };
        MainWindow.Show();

        base.OnStartup(e);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _trayIcon.Dispose();
        _mainViewModel.Dispose();
        base.OnExit(e);
    }
    
    public void OnTrayClick(object? sender, EventArgs e)
    {
        if (MainWindow!.Visibility != Visibility.Hidden) return;
        MainWindow.Show();
        MainWindow.WindowState = WindowState.Normal;
    }
}
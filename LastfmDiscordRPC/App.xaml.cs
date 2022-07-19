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

    /// <summary>
    /// Creates the view model and tray icon that will be used in the application, as well as setting
    /// DefaultWebProxy to null.
    /// </summary>
    public App()
    {
        DefaultWebProxy = null;
        _mainViewModel = new MainViewModel(SavedData);
        _trayIcon = new TrayIcon(this);
        AppDomain.CurrentDomain.UnhandledException += (sender, _) =>
        {
            _mainViewModel.Logger.Error("Unhandled Exception! {0}", ((Exception)sender).Message);
            MessageBox.Show("Please report this error to the developer!",
                "Unhandled exception!",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            Shutdown(1);
        };
    }

    /// <summary>
    /// Creates the main window and shows it.
    /// </summary>
    /// <param name="e"></param>
    protected override void OnStartup(StartupEventArgs e)
    {
        MainWindow = new MainWindow
        {
            DataContext = _mainViewModel
        };
        MainWindow.Show();

        base.OnStartup(e);
    }

    /// <summary>
    /// When the program is closed, disposes of the disposable objects in MainViewModel and the tray icon in the bottom right.
    /// </summary>
    /// <param name="e"></param>
    protected override void OnExit(ExitEventArgs e)
    {
        _trayIcon.Dispose();
        _mainViewModel.Dispose();
        base.OnExit(e);
    }
    
    /// <summary>
    /// When the tray icon is clicked, it simply shows the main window again.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnTrayClick(object? sender, EventArgs e)
    {
        if (MainWindow!.Visibility != Visibility.Hidden)
        {
            return;
        }
        MainWindow.Show();
        MainWindow.WindowState = WindowState.Normal;
    }
}
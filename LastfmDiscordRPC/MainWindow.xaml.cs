using System;
using System.ComponentModel;
using System.Drawing;
using System.Resources;
using System.Windows;
using System.Windows.Forms;

namespace LastfmDiscordRPC;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    private NotifyIcon? _trayIcon;

    public MainWindow()
    {
        InitializeComponent();
        
        ResourceManager resourceManager = new ResourceManager(typeof(Resources.TrayIcon));

        _trayIcon = new NotifyIcon();
        _trayIcon.Icon = (Icon)resourceManager.GetObject("Icon")!;
        _trayIcon.Text = @"Last.fm Rich Presence";
        _trayIcon.Visible = true;
        _trayIcon.Click += (sender, args) =>
        {
            Show();
            WindowState = WindowState.Normal;
        };
    }
    
    protected override void OnStateChanged(EventArgs e)
    {
        if (WindowState == WindowState.Minimized) Hide();
        base.OnStateChanged(e);
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        _trayIcon!.Visible = false;
        _trayIcon = null;
        base.OnClosing(e);
    }
}
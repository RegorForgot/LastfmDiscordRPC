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
    private NotifyIcon? _icon;
    public MainWindow()
    {
        InitializeComponent();
        ResourceManager resourceManager = new ResourceManager(typeof(Resources.TrayIcon));
            
        _icon = new NotifyIcon();
        _icon.Icon = (Icon) resourceManager.GetObject("Icon")!;
        _icon.Visible = true;
        _icon.Click += ClickTrayIcon;
    }

    protected override void OnStateChanged(EventArgs e)
    {
        if (WindowState == WindowState.Minimized) Hide();
        base.OnStateChanged(e);
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        _icon!.Click -= ClickTrayIcon;
        _icon.Visible = false;
        _icon = null;
        base.OnClosing(e);
    }
        
    private void ClickTrayIcon(object? sender, EventArgs e)
    {
        Show();
        WindowState = WindowState.Normal;
    }

    private void CloseButton_OnClick(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
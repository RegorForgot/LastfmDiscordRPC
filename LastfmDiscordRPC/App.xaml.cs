using System;
using System.Drawing;
using System.Resources;
using System.Windows;
using System.Windows.Forms;
using LastfmDiscordRPC.Commands;
using LastfmDiscordRPC.Models;
using LastfmDiscordRPC.ViewModels;
using static System.Net.WebRequest;
using static LastfmDiscordRPC.Models.SaveAppData;

namespace LastfmDiscordRPC;

public partial class App
{
    private readonly DiscordClient _client;
    private readonly MainViewModel _mainViewModel;
    private readonly ResourceManager _manager;
    private readonly NotifyIcon _trayIcon;

    public App()
    {
        DefaultWebProxy = null;
        _trayIcon = new NotifyIcon();
        _manager = new ResourceManager(typeof(Resources.EmbeddedImages));
        _client = new DiscordClient(SavedData.AppKey);
        _mainViewModel = new MainViewModel(SavedData.Username, SavedData.APIKey, SavedData.AppKey, _client);
        SetTrayIcon();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        MainWindow = new MainWindow(_manager)
        {
            DataContext = _mainViewModel
        };
        MainWindow.Show();

        base.OnStartup(e);
    }

    private void SetTrayIcon()
    {
        _trayIcon.Icon = (Icon)_manager.GetObject("AppIcon")!;
        _trayIcon.Text = @"Last.fm Rich Presence";
        _trayIcon.Visible = true;
        _trayIcon.Click += TrayIcon_OnClick;
        _trayIcon.ContextMenuStrip = GetContextMenuStrip();
    }

    private ContextMenuStrip GetContextMenuStrip()
    {
        ContextMenuStrip contextMenu = new ContextMenuStrip();
        ColorConverter converter = new ColorConverter();
        contextMenu.Items.Add(new ToolStripLabel("Last.fm Rich presence", ((Icon)_manager.GetObject("AppIcon")!).ToBitmap()));
        contextMenu.Items.Add(new ToolStripSeparator());
        contextMenu.Items.Add(new ToolStripMenuItem("Open App", null, OnTrayClick));
        contextMenu.Items.Add(new ToolStripMenuItem("Exit", null, (sender, args) => Shutdown(0)));
        contextMenu.Font = new Font("Calibri Light", 11);
        contextMenu.ForeColor = Color.White;
        contextMenu.BackColor = (Color)(converter.ConvertFromString("#263238") ?? Color.DarkSlateGray);
        contextMenu.ShowImageMargin = false;

        return contextMenu;
    }

    private void OnTrayClick(object? sender, EventArgs e)
    {
        if (MainWindow.Visibility != Visibility.Hidden) return;
        MainWindow.Show();
        MainWindow.WindowState = WindowState.Normal;
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _trayIcon.Dispose();
        _client.Dispose();
        ((SetPresenceCommand)_mainViewModel.SetPresenceCommand).Dispose();
        base.OnExit(e);
    }

    private void TrayIcon_OnClick(object? sender, EventArgs e)
    {
        if (((MouseEventArgs)e).Button != MouseButtons.Left) return;

        OnTrayClick(sender, e);
    }
}
using System;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;
using LastfmDiscordRPC.Resources;

namespace LastfmDiscordRPC.Views;

public class TrayIcon : IDisposable
{
    private readonly NotifyIcon _trayIcon;
    private readonly ResourceManager _manager;
    private readonly App _app;

    public TrayIcon(App app)
    {
        _app = app;
        _trayIcon = new NotifyIcon();
        _manager = new ResourceManager(typeof(EmbeddedImages));
        SetTrayIcon();
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
        contextMenu.Items.Add(new ToolStripMenuItem("Open App", null, _app.OnTrayClick));
        contextMenu.Items.Add(new ToolStripMenuItem("Exit", null, (_, _) => _app.Shutdown(0)));
        contextMenu.Font = new Font("Calibri Light", 11);
        contextMenu.ForeColor = Color.White;
        contextMenu.BackColor = (Color)(converter.ConvertFromString("#263238") ?? Color.DarkSlateGray);
        contextMenu.ShowImageMargin = false;

        return contextMenu;
    }
    
    private void TrayIcon_OnClick(object? sender, EventArgs e)
    {
        if (((MouseEventArgs)e).Button != MouseButtons.Left)
        {
            return;
        }
        _app.OnTrayClick(sender, e);
    }

    public void Dispose()
    {
        _trayIcon.Dispose();
        SuppressFinalize(this);
    }
}
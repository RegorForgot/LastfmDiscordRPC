using System;
using System.Resources;
using System.Windows;

namespace LastfmDiscordRPC;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    public MainWindow(ResourceManager manager)
    {
        InitializeComponent();
    }

    protected override void OnStateChanged(EventArgs e)
    {
        if (WindowState == WindowState.Minimized) Hide();
        base.OnStateChanged(e);
    }
}
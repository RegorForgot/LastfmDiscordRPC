using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using LastfmDiscordRPC2.IO;
using LastfmDiscordRPC2.Utilities;
using LastfmDiscordRPC2.ViewModels;
using LastfmDiscordRPC2.ViewModels.Panes;

namespace LastfmDiscordRPC2.Views;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel? dataContext)
    {
        InitializeComponent();
        
        TransparencyLevelHint = new[] { WindowTransparencyLevel.Mica, WindowTransparencyLevel.None };

        if (RuntimeLocator.IsWindows11)
        {
            Background = new SolidColorBrush(Colors.Transparent);
        }

        DataContext = dataContext;
        
        foreach (AbstractPaneViewModel viewModel in dataContext.Children)
        {
            UserControl? control = this.FindControl<UserControl>(viewModel.Name);
            if (control != null)
            {
                control.DataContext = viewModel;
            }
        }
        
        IsVisibleProperty.Changed.AddClassHandler<Window>((window, args) =>
        {
            if (window == this && window.PlatformImpl != null && (bool)args.NewValue)
            {
                WindowState = WindowState.Normal;
            }
        });
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        if (DataContext is not MainViewModel { IoService.SaveCfg.CloseToTray: true })
        {
            return;
        }
        e.Cancel = true;
        Hide();
    }
}
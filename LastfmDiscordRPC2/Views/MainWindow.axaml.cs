using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using LastfmDiscordRPC2.ViewModels;
using LastfmDiscordRPC2.ViewModels.Panes;

namespace LastfmDiscordRPC2.Views;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel? dataContext)
    {
        InitializeComponent();

        
        TransparencyLevelHint = new[] { WindowTransparencyLevel.Mica, WindowTransparencyLevel.None };

        if (OperatingSystem.IsWindows11)
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

        WindowStateProperty.Changed.AddClassHandler<Window>((window, args) =>
        {
            if (window == this && window.PlatformImpl != null && (WindowState)(args.NewValue ?? WindowState.Maximized) == WindowState.Minimized)
            {
                Hide();
            }
        });

        IsVisibleProperty.Changed.AddClassHandler<Window>((window, args) =>
        {
            if (window == this && window.PlatformImpl != null && (bool)args.NewValue)
            {
                WindowState = WindowState.Normal;
            }
        });
    }
}
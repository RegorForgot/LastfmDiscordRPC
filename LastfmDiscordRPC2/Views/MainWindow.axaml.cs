using Avalonia.Controls;
using Avalonia.Media;
using LastfmDiscordRPC2.DataTypes;
using LastfmDiscordRPC2.ViewModels;
using LastfmDiscordRPC2.ViewModels.Panes;

namespace LastfmDiscordRPC2.Views;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel dataContext)
    {
        InitializeComponent();
        
        DataContext = dataContext;
        TransparencyLevelHint = new[] { WindowTransparencyLevel.Mica, WindowTransparencyLevel.AcrylicBlur, WindowTransparencyLevel.None };

        Background = OperatingSystem.CurrentOS == OSEnum.Windows ? new SolidColorBrush(Colors.Transparent) : new SolidColorBrush(Colors.Black);
        
        foreach (AbstractPaneViewModel viewModel in dataContext.Children)
        {
            UserControl? control = this.FindControl<UserControl>(viewModel.Name);
            if (control != null)
            {
                control.DataContext = viewModel;
            }
        }
    }
}
using Avalonia.Controls;
using Avalonia.Media;
using LastfmDiscordRPC2.Enums;
using LastfmDiscordRPC2.ViewModels;
using LastfmDiscordRPC2.ViewModels.Panes;

namespace LastfmDiscordRPC2.Views;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel dataContext)
    {
        InitializeComponent();
        
        DataContext = dataContext;
        Background = OperatingSystem.CurrentOS == OSEnum.Windows ? new SolidColorBrush(Colors.Transparent) : new SolidColorBrush(Colors.Black);
        
        foreach (IPaneViewModel viewModel in dataContext.Children)
        {
            UserControl? control = this.FindControl<UserControl>(viewModel.PaneName);
            if (control != null)
            {
                control.DataContext = viewModel;
            }
        }
    }
}
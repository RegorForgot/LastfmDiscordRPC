using System.Runtime.InteropServices;
using Avalonia.Controls;
using Avalonia.Media;
using LastfmDiscordRPC2.Models;

namespace LastfmDiscordRPC2.Views;

public partial class MainWindowView : Window
{
    private static readonly Color WindowsBackground = Color.FromArgb(0xD0, 0x00, 0x00, 0x00);
    private static readonly Color UnixBackground = Color.FromRgb(0x00, 0x00, 0x00);
    
    public MainWindowView()
    {
        InitializeComponent();
        Background = Utilities.OS == OSPlatform.Windows ? 
            new SolidColorBrush(WindowsBackground) : new SolidColorBrush(UnixBackground);
    }
}
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace LastfmDiscordRPC2.Views;

public partial class Settings : UserControl
{
    public Settings()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
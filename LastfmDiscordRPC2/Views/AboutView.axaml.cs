using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace LastfmDiscordRPC2.Views;

public partial class AboutView : UserControl
{
    public AboutView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
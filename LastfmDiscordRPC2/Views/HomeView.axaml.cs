using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace LastfmDiscordRPC2.Views;

public partial class Home : UserControl
{
    public Home()
    {
        InitalizeComponent();
    }

    private void InitalizeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
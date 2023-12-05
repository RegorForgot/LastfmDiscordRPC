using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace LastfmDiscordRPC2.Views.Controls;

public partial class PreviewControlView : UserControl
{
    public PreviewControlView()
    {
        InitalizeComponent();
    }

    private void InitalizeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
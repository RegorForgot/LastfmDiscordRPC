using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace LastfmDiscordRPC2.Views.Controls;

public partial class PreviewConfigControlView : UserControl
{
    public PreviewConfigControlView()
    {
        InitalizeComponent();
    }

    private void InitalizeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
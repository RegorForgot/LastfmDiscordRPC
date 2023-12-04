using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace LastfmDiscordRPC2.Views.Controls;

public partial class PreviewSettingControlView : UserControl
{
    public PreviewSettingControlView()
    {
        InitalizeComponent();
    }

    private void InitalizeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
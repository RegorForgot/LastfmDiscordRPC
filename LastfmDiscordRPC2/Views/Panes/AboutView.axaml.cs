using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace LastfmDiscordRPC2.Views.Panes;

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
    
    private void RPCConsoleOnTextChanged(object? sender, TextChangedEventArgs e)
    {
        // Yuck....
        TextBox? rpcConsole = (TextBox)sender!;
        if (rpcConsole?.Text != null)
        {
            rpcConsole.CaretIndex = rpcConsole.Text.Length;
        }
    }
}
using System.Windows.Controls;

namespace LastfmDiscordRPC.Views;

public partial class Body
{
    public Body()
    {
        InitializeComponent();
    }

    private void OutputBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        OutputBox.ScrollToEnd();
    }
}
using System.Diagnostics;
using System.Windows.Navigation;

namespace LastfmDiscordRPC.Views;

public partial class Footer
{
    public Footer()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Opens a window in browser that opens the link to the Github repo.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SourceLink_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
    {
        string url = SourceLink.NavigateUri.AbsoluteUri;
        ProcessStartInfo sInfo = new ProcessStartInfo(url)
        {
            UseShellExecute = true
        };
        Process.Start(sInfo);
    }
}
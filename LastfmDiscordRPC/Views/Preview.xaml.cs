using System.Diagnostics;
using System.Windows.Navigation;

namespace LastfmDiscordRPC.Views;

public partial class Preview
{
    public Preview()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Opens a window in browser that opens the link to the album cover's image.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void AlbumCover_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
    {
        string url = AlbumCover.NavigateUri.AbsoluteUri;
        ProcessStartInfo sInfo = new ProcessStartInfo(url)
        {
            UseShellExecute = true
        };
        Process.Start(sInfo);
    }
}
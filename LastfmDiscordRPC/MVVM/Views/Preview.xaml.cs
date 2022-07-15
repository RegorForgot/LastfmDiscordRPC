using System.Diagnostics;
using System.Windows.Navigation;

namespace LastfmDiscordRPC.MVVM.Views;

public partial class Preview
{
    public Preview()
    {
        InitializeComponent();
    }

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
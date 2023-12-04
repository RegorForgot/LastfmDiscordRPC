using System.Diagnostics;

namespace LastfmDiscordRPC2.Utilities;

public static class Utilities
{
    public const string LastfmAPIKey = "79d35013754ac3b3225b73bba566afca";

    public static void OpenURI(string url)
    {
        Process.Start(
            new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            }
        );
    }
}
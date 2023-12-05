using System;
using System.Diagnostics;
using System.Reactive;
using ReactiveUI;

namespace LastfmDiscordRPC2.Utilities;

public static class URIOpen
{
    public static ReactiveCommand<string, Unit> OpenURICmd { get; } = ReactiveCommand.Create<string>(OpenURI);

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
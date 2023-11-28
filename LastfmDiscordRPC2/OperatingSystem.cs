using System.Runtime.InteropServices;
using LastfmDiscordRPC2.Enums;

namespace LastfmDiscordRPC2;

public static class OperatingSystem
{
    public static OSEnum CurrentOS { get; }
    
    static OperatingSystem()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            CurrentOS = OSEnum.OSX;
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            CurrentOS = OSEnum.Linux;
        }
        else
        {
            CurrentOS = OSEnum.Windows;
        }
    }
}
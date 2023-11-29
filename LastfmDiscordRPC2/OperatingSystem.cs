using System;
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
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            CurrentOS = OSEnum.Windows;
        }
        else
        {
            throw new Exception("Unsupported operating system... How did you even get here!?");
        }
    }
}
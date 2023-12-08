using System;
using System.Runtime.InteropServices;
using LastfmDiscordRPC2.DataTypes;

namespace LastfmDiscordRPC2.Utilities;

public static class RuntimeLocator
{
    public static OSRuntimes CurrentOS { get; }
    public static bool IsWindows11 { get; }

    static RuntimeLocator()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            CurrentOS = OSRuntimes.OSX;
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            CurrentOS = OSRuntimes.Linux;
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            CurrentOS = OSRuntimes.Windows;
            IsWindows11 = Environment.OSVersion.Version.Build >= 22000;
        }
        else
        {
            throw new Exception("Unsupported operating system... How did you even get here!?");
        }
    }
}
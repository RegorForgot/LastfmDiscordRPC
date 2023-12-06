using System;
using System.Runtime.InteropServices;

namespace LastfmDiscordRPC2;

public static class OperatingSystem
{
    public static DataTypes.OperatingSystem CurrentOS { get; }

    static OperatingSystem()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            CurrentOS = DataTypes.OperatingSystem.OSX;
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            CurrentOS = DataTypes.OperatingSystem.Linux;
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            CurrentOS = DataTypes.OperatingSystem.Windows;
        }
        else
        {
            throw new Exception("Unsupported operating system... How did you even get here!?");
        }
    }
}
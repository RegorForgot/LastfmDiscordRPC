using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Win32;

namespace LastfmDiscordRPC2.Utilities;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
public static class WinRegistry
{
    public static bool CheckRegistryExists()
    {
        RegistryKey? winStartup = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
        bool isRegistered = winStartup != null && winStartup.GetValueNames().Contains("LastfmDiscordRPC");
        winStartup?.Close();
        return isRegistered;
    }

    public static void SetRegistry(bool startUpValue)
    {
        RegistryKey? winStartup =
            Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
        if (startUpValue)
        {
            winStartup?.SetValue("LastfmDiscordRPC", '"' + AppContext.BaseDirectory + "LastfmDiscordRPC2.exe" + '"');
        }
        else
        {
            winStartup?.DeleteValue("LastfmDiscordRPC", false);
        }
    }
}
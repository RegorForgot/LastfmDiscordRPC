using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Win32;

namespace LastfmDiscordRPC2.Models;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
public static class Utilities
{
    public static bool CheckRegistryExists()
    {
        RegistryKey? winLogon = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
        bool exists = winLogon != null && winLogon.GetValueNames().Contains("LastfmDiscordRPC");
        winLogon?.Close();
        return exists;
    }

    public static void SetRegistry(bool startUpChecked)
    {
        RegistryKey? winLogon = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
        if (startUpChecked)
        {
            winLogon?.SetValue("LastfmDiscordRPC", '"' + AppContext.BaseDirectory + "LastfmDiscordRPC2.exe" + '"');
        }
        else
        {
            winLogon?.DeleteValue("LastfmDiscordRPC", false);
        }
    }
}
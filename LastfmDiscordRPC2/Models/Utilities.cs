using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using LastfmDiscordRPC2.IO.Schema;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace LastfmDiscordRPC2.Models;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
public static class Utilities
{
    public static readonly string DefaultAppID = "997756398664421446";
    public static readonly string LastfmAPIKey = "79d35013754ac3b3225b73bba566afca";

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

    public static void OpenWebpage(string url)
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
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace LastfmDiscordRPC2.Models;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
public static class Utilities
{
    public static readonly string DefaultAppID = "997756398664421446";
    public static readonly string APIKey = "79d35013754ac3b3225b73bba566afca";
    public static readonly OSPlatform OS;

    static Utilities()
    {
        OS = GetOperatingSystem();
    }

    private static OSPlatform GetOperatingSystem()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return OSPlatform.OSX;
        }
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return OSPlatform.Linux;
        }
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return OSPlatform.Windows;
        }

        throw new Exception("Cannot determine operating system!");
    }

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

    public static class SaveAppData
    {
        public static AppData SavedData { get; private set; }
        public static readonly string FolderPath;
        private static readonly string FilePath;
        private static readonly object Lock = new object();

        static SaveAppData()
        {
            FolderPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            FolderPath += OS.ToString() switch
            {
                "WINDOWS" => @"/AppData/Local/LastfmDiscordRPC",
                "LINUX" => @"/.LastfmDiscordRPC",
                "OSX" => @"/Library/Application Support/LastfmDiscordRPC",
                _ => FolderPath
            };

            FilePath = $@"{FolderPath}/config.json";
            ReadData();
        }

        private static void ReadData()
        {
            try
            {
                string json;
                lock (Lock) json = File.ReadAllText(FilePath);
                AppData appData = JsonConvert.DeserializeObject<AppData>(json) ?? throw new Exception();
                SavedData = appData;
            }
            catch (Exception)
            {
                SaveData(new AppData());
            }
        }

        public static void SaveData(AppData appData)
        {
            try
            {
                lock (Lock) File.WriteAllText(FilePath, JsonConvert.SerializeObject(appData));
            }
            catch (IOException)
            {
                if (Directory.Exists(FolderPath))
                {
                    throw;
                }

                Directory.CreateDirectory(FolderPath);
                SaveData(appData);
            }

            SavedData = appData;
        }
    }
}
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading;
using Avalonia;
using Avalonia.Platform;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace LastfmDiscordRPC2.Models;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
public static class Utilities
{
    public static readonly string DefaultAppID = "997756398664421446";
    public static readonly string APIKey = "79d35013754ac3b3225b73bba566afca";
    private static readonly OperatingSystemType OS;

    static Utilities()
    {
        OS = AvaloniaLocator.Current.GetService<IRuntimePlatform>()!
            .GetRuntimeInfo().OperatingSystem;
    }

    public static bool CheckRegistryExists()
    {
        RegistryKey? winStartup = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
        bool exists = winStartup != null && winStartup.GetValueNames().Contains("LastfmDiscordRPC");
        winStartup?.Close();
        return exists;
    }

    public static void SetRegistry(bool startUpChecked)
    {
        RegistryKey? winStartup =
            Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
        if (startUpChecked)
        {
            winStartup?.SetValue("LastfmDiscordRPC", '"' + AppContext.BaseDirectory + "LastfmDiscordRPC2.exe" + '"');
        }
        else
        {
            winStartup?.DeleteValue("LastfmDiscordRPC", false);
        }
    }

    public static class SaveAppData
    {
        public static AppData SavedData { get; }
        private static readonly string FolderPath;
        private static readonly string FilePath;
        private static readonly object Lock = new();

        static SaveAppData()
        {
            FolderPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            FolderPath += OS switch
            {
                OperatingSystemType.WinNT => @"/AppData/Local/LastfmDiscordRPC",
                OperatingSystemType.Linux => @"/.LastfmDiscordRPC",
                OperatingSystemType.OSX => @"/Library/Application Support/LastfmDiscordRPC",
                _ => FolderPath
            };

            FilePath = $@"{FolderPath}/config.json";

            SavedData = ReadData();
        }

        private static AppData ReadData()
        {
            try
            {
                string json;
                lock (Lock) json = File.ReadAllText(FilePath);
                AppData appData = JsonConvert.DeserializeObject<AppData>(json) ?? throw new Exception();
                return appData;
            }
            catch (Exception)
            {
                return SaveData(new AppData());
            }
        }
        
        public static void SaveData()
        {
            SaveData(SavedData);
        }

        public static AppData SaveData(AppData appData)
        {
            try
            {
                lock (Lock) File.WriteAllText(FilePath, JsonConvert.SerializeObject(appData));
            }
            catch (IOException)
            {
                if (CheckFolderExists())
                {
                    throw;
                }

                Directory.CreateDirectory(FolderPath);
                SaveData(appData);
            }

            return appData;
        }

        private static bool CheckFolderExists()
        {
            if (Directory.Exists(FolderPath))
            {
                return true;
            }

            Directory.CreateDirectory(FolderPath);
            return false;
        }
    }
}
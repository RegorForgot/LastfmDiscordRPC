using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace LastfmDiscordRPC2.Models;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
public static class Utilities
{
    public readonly static string DefaultAppID = "997756398664421446";
    public readonly static string APIKey = "79d35013754ac3b3225b73bba566afca";

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

    public static class SaveAppData
    {
        public static AppData SavedData { get; private set; }
        public readonly static string FolderPath;
        private readonly static string FilePath;
        private readonly static object Lock = new object();

        static SaveAppData()
        {
            if (OperatingSystem.IsWindows())
            {
                FolderPath =
                    $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\LastfmDiscordRPC";
                FilePath = $@"{FolderPath}\config.json";
            }

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
                return new AppData();
            }
        }

        public static void SaveData(AppData appData)
        {
            SavedData = appData;
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
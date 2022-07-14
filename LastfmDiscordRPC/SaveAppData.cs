using System;
using System.IO;
using System.Windows;
using LastfmDiscordRPC.JSONSchemas;
using Newtonsoft.Json;

namespace LastfmDiscordRPC;

public static class SaveAppData
{
    public static AppData SavedData { get; private set; }
    private readonly static string FilePath;

    static SaveAppData()
    {
        string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\LastfmDiscordRPC";
        FilePath = folderPath + "\\config.json";
        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
        if (!File.Exists(FilePath)) CreateDefaultFile();
        else 
        {
            try
            {
                SavedData = JsonConvert.DeserializeObject<AppData>(File.ReadAllText(FilePath))!;
            } catch (JsonException e)
            {
                string errorMessage = $"Error! {e.Message}\n Defaulting values.";
                MessageBox.Show(errorMessage, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                CreateDefaultFile();
            }
        }
    }

    private static void CreateDefaultFile()
    {
        SaveData("default", "05467a3191853eb8da38dfb38ed3c733");
    }

    public static void SaveData(string username, string apiKey)
    {
        AppData appData = new AppData()
        {
            Username = username, APIKey = apiKey
        };
        SavedData = appData;
        File.WriteAllText(FilePath, JsonConvert.SerializeObject(appData));
    }
}
using System;
using System.IO;
using Newtonsoft.Json;

namespace LastfmDiscordRPC.Models;

public static class SaveAppData
{
    public static AppData SavedData { get; private set; }
    public const string DefaultAPIKey = "05467a3191853eb8da38dfb38ed3c733";
    public const string DefaultAppID = "997756398664421446";
    public readonly static string FolderPath;
    private readonly static string FilePath;
    private readonly static object Lock = new object();

    static SaveAppData()
    {
        FolderPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\LastfmDiscordRPC";
        FilePath = $@"{FolderPath}\config.json";
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
        } catch (Exception)
        {
            return new AppData();
        }
    }

    private static void SaveData(AppData appData)
    {
        SaveData(appData.Username, appData.APIKey, appData.AppID);
    }

    public static void SaveData(string username, string apiKey, string appKey)
    {
        AppData appData = new AppData
        {
            Username = username, APIKey = apiKey, AppID = appKey
        };
        SavedData = appData;

        try
        {
            lock (Lock) File.WriteAllText(FilePath, JsonConvert.SerializeObject(appData));
        } catch (IOException)
        {
            if (CheckFolderExists()) throw;
            Directory.CreateDirectory(FolderPath);
            SaveData(appData);
        }
    }

    public static bool CheckFolderExists()
    {
        if (Directory.Exists(FolderPath)) return true;
        Directory.CreateDirectory(FolderPath);

        return false;
    }

    public class AppData
    {
        [JsonProperty] public string Username { get; set; } = Empty;
        [JsonProperty] public string APIKey { get; set; } = DefaultAPIKey;
        [JsonProperty] public string AppID { get; set; } = DefaultAppID;
    }
}
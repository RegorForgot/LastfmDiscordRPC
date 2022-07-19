using System;
using System.IO;
using Newtonsoft.Json;

namespace LastfmDiscordRPC.Models;

public static class SaveAppData
{
    public static AppData SavedData { get; private set; }
    public readonly static string DefaultAPIKey = "05467a3191853eb8da38dfb38ed3c733";
    public readonly static string DefaultAppID = "997756398664421446";
    public readonly static string FolderPath;
    private readonly static string FilePath;
    private readonly static object Lock = new object();

    static SaveAppData()
    {
        FolderPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\LastfmDiscordRPC";
        FilePath = $@"{FolderPath}\config.json";
        SavedData = ReadData();
    }

    /// <summary>
    /// Read the data from 
    /// </summary>
    /// <returns>AppData containing user or default data</returns>
    /// <exception cref="Exception">Any exception thrown - will simply return default</exception>
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

    /// <summary>
    /// Try to save the username, apiKey, and appKey that have been provided in the paramaters - set the SavedData static
    /// variable to those values.
    /// </summary>
    /// <param name="username"/>
    /// <param name="apiKey"></param>
    /// <param name="appKey"></param>
    /// <exception cref="IOException">If it cannot save even with the folder created</exception>
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
            if (CheckFolderExists())
            {
                throw;
            }
            Directory.CreateDirectory(FolderPath);
            SaveData(appData);
        }
    }

    /// <summary>
    /// Check if the save/log-file folder exists - if it does not, make it.
    /// </summary>
    /// <returns></returns>
    public static bool CheckFolderExists()
    {
        if (Directory.Exists(FolderPath))
        {
            return true;
        }
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
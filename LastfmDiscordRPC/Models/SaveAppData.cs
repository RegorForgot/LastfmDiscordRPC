using System;
using System.IO;
using System.Windows;
using Newtonsoft.Json;
using static System.String;

namespace LastfmDiscordRPC.Models;

public static class SaveAppData
{
    public static AppData SavedData { get; private set; }
    public const string DefaultAPIKey = "05467a3191853eb8da38dfb38ed3c733";
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
        AppData? appData = null;

        try
        {
            string json;
            lock (Lock) json = File.ReadAllText(FilePath);
            appData = JsonConvert.DeserializeObject<AppData>(json) ?? throw new NoDataException();
        } catch (JsonException e)
        {
            string errorMessage = $"Error deserializing! {e.Message}\n Defaulting values.";
            MessageBox.Show(errorMessage, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            SaveData();
        } catch (NoDataException e)
        {
            string errorMessage = $"No data found! {e.Message}\n Defaulting values.";
            MessageBox.Show(errorMessage, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            SaveData();
        } catch (IOException e)
        {
            string errorMessage = $"Error reading from file! {e.Message}\n Defaulting values.";
            MessageBox.Show(errorMessage, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
        } finally
        {
            appData ??= new AppData
            {
                Username = "", APIKey = "", AppKey = ""
            };
        }

        return appData;
    }

    public static void SaveData(string username = "", string apiKey = "", string appKey = "")
    {
        AppData appData = new AppData
        {
            Username = username, APIKey = apiKey, AppKey = appKey
        };
        SavedData = appData;

        try
        {
            lock(Lock) File.WriteAllText(FilePath, JsonConvert.SerializeObject(appData));
        } catch (IOException e)
        {
            string errorMessage = $"Error writing to file! {e.Message}\n Aborting write.";
            MessageBox.Show(errorMessage, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private class NoDataException : Exception
    { }
}

public class AppData
{
    [JsonProperty] public string Username { get; set; } = Empty;
    [JsonProperty] public string APIKey { get; set; } = Empty;
    [JsonProperty] public string AppKey { get; set; } = Empty;
}
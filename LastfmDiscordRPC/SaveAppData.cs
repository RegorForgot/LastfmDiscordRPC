using System;
using System.IO;
using System.Windows;
using LastfmDiscordRPC.Exceptions;
using Newtonsoft.Json;

namespace LastfmDiscordRPC;

public static class SaveAppData
{
    public static AppData SavedData { get; private set; }
    public const string DefaultAPIKey = "05467a3191853eb8da38dfb38ed3c733";
    private readonly static string FolderPath;
    private readonly static string FilePath;

    static SaveAppData()
    {
        FolderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\LastfmDiscordRPC";
        FilePath = FolderPath + "\\config.json";

        CreateFileIfNotExist();
        SavedData = ReadData();
    }

    private static void CreateFileIfNotExist()
    {
        if (!Directory.Exists(FolderPath)) Directory.CreateDirectory(FolderPath);
        if (!File.Exists(FilePath)) File.Create(FilePath).Close();
    }

    private static AppData ReadData()
    {
        AppData? appData = null;

        try
        {
            string json = File.ReadAllText(FilePath);
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
                Username = "default", APIKey = DefaultAPIKey
            };
        }

        return appData;
    }

    public static void SaveData(string username = "default", string apiKey = DefaultAPIKey)
    {
        CreateFileIfNotExist();
        
        AppData appData = new AppData
        {
            Username = username, APIKey = apiKey
        };
        SavedData = appData;

        try
        {
            File.WriteAllText(FilePath, JsonConvert.SerializeObject(appData));
        } catch (IOException e)
        {
            string errorMessage = $"Error writing to file! {e.Message}\n Aborting write.";
            MessageBox.Show(errorMessage, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
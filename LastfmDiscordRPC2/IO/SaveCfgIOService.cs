using System;
using System.IO;
using LastfmDiscordRPC2.Logging;
using Newtonsoft.Json;

namespace LastfmDiscordRPC2.IO;

public class SaveCfgIOService : AbstractIOService
{
    private readonly LoggingService _loggingService;
    public SaveCfg SaveCfg { get; private set; }
    public override string FilePath { get; protected set; } = $"{SaveFolder}/config.json";
    
    public SaveCfgIOService(LoggingService loggingService)
    {
        _loggingService = loggingService;
        ReadConfigData();
    }

    private void ReadConfigData()
    {
        try
        {
            string json;
            lock (FileLock)
            {
                json = File.ReadAllText(FilePath);
            }

            SaveCfg configData = JsonConvert.DeserializeObject<SaveCfg>(json) ?? throw new Exception();
            SaveCfg = configData;
        }
        catch (Exception ex) when (ex is IOException or JsonException)
        {
            SaveConfigData(new SaveCfg());
        }
    }

    public void SaveConfigData(SaveCfg configData)
    {
        try
        {
            lock (FileLock)
            {
                string serializedData = JsonConvert.SerializeObject(configData);
                WriteToFile(serializedData);
                SaveCfg = configData;
            }
        } 
        catch (Exception ex) when (ex is IOException or JsonException)
        {
            _loggingService.Error("Fatal IO Exception!");
        }
    }
}
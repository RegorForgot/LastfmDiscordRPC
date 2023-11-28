using System;
using System.IO;
using LastfmDiscordRPC2.IO.Schema;
using LastfmDiscordRPC2.Logging;
using Newtonsoft.Json;

namespace LastfmDiscordRPC2.IO;

public abstract class AbstractConfigFileIO<T> : AbstractFileIO where T : IFileData, new()
{
    private readonly IRPCLogger _logger;
    public T ConfigData { get; private set; }
    
    protected AbstractConfigFileIO(IRPCLogger logger)
    {
        _logger = logger;
        ReadConfigData();
    }

    protected void ReadConfigData()
    {
        try
        {
            string json;
            lock (FileLock)
            {
                json = File.ReadAllText(FilePath);
            }

            T configData = DeserializeFileData(json);
            ConfigData = configData;
        }
        catch (Exception ex) when (ex is IOException or JsonException)
        {
            SaveConfigData(new T());
        }
    }

    public void SaveConfigData(T configData)
    {
        try
        {
            lock (FileLock)
            {
                string serializedData = JsonConvert.SerializeObject(configData);
                WriteToFile(serializedData);
                ConfigData = configData;
            }
        } 
        catch (Exception ex) when (ex is IOException or JsonException)
        {
            _logger.Error("Fatal IO Exception!");
        }
    }
    
    private static T DeserializeFileData(string json)
    {
        return JsonConvert.DeserializeObject<T>(json) ?? throw new Exception();
    }
}
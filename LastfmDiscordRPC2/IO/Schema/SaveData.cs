using Newtonsoft.Json;

namespace LastfmDiscordRPC2.IO.Schema;

public record SaveData : IFileData
{
    public SaveData() { }
    
    public SaveData(SaveData savedData)
    {
        UserAccount = savedData.UserAccount;
        UserRPCConfig = savedData.UserRPCConfig;
        AppID = savedData.AppID;
        SleepTime = savedData.SleepTime;
    }
    
    [JsonProperty] public Account UserAccount { get; set; } = new Account();
    [JsonProperty] public RPCConfig UserRPCConfig { get; set; } = new RPCConfig();
    [JsonProperty] public string AppID { get; set; } = Utilities.Utilities.DefaultAppID;
    [JsonProperty] public int SleepTime { get; set; } = 3600;

    public record Account
    {
        [JsonProperty] public string Username { get; set; } = Empty;
        [JsonProperty] public string SessionKey { get; set; } = Empty;
    }

    public record RPCConfig
    {
        [JsonProperty] public string Description { get; set; } = Empty;
        [JsonProperty] public string State { get; set; } = Empty;
        [JsonProperty] public Button[] UserButtons { get; set; } = {};
    }
    
    public record Button
    {
        [JsonProperty] public string Link { get; set; } = Empty;
        [JsonProperty] public string Label { get; set; } = Empty;
    }
}
using Newtonsoft.Json;
using ReactiveUI;

namespace LastfmDiscordRPC2.IO;

public record SaveCfg
{
    private const string DefaultAppID = "997756398664421446";
    
    public SaveCfg() { }
    
    public SaveCfg(SaveCfg other)
    {
        UserAccount = other.UserAccount;
        UserRPCCfg = other.UserRPCCfg;
        AppID = other.AppID;
        SleepTime = other.SleepTime;
    }
    
    [JsonProperty] public Account UserAccount { get; set; } = new Account();
    [JsonProperty] public RPCCfg UserRPCCfg { get; set; } = new RPCCfg();
    [JsonProperty] public string AppID { get; set; } = DefaultAppID;
    [JsonProperty] public int SleepTime { get; set; } = 3600;

    public record Account
    {
        [JsonProperty] public string Username { get; set; } = Empty;
        [JsonProperty] public string SessionKey { get; set; } = Empty;
    }

    public record RPCCfg
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
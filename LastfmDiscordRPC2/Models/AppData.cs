using Newtonsoft.Json;
using static System.String;

namespace LastfmDiscordRPC2.Models;

public class AppData
{
    [JsonProperty] public Account UserAccount { get; set; }
    [JsonProperty] public RPCConfig UserRPCConfig { get; set; }
    [JsonProperty] public string AppID { get; set; } = Utilities.DefaultAppID;
    [JsonProperty] public string LoveTrackHotkey { get; set; }
    [JsonProperty] public bool IsHotkeyGlobal { get; set; }

    public class Account
    {
        [JsonProperty] public string Username { get; set; } = Empty;
        [JsonProperty] public string SessionKey { get; set; } = Empty;
    }

    public class RPCConfig
    {
        [JsonProperty] public string Description { get; set; } = Empty;
        [JsonProperty] public string State { get; set; } = Empty;
        [JsonProperty] public Button[] UserButtons { get; set; }

        public RPCConfig()
        {
            UserButtons = new Button[] {};
        }
    }
    
    public class Button
    {
        [JsonProperty] public string Link { get; set; } = Empty;
        [JsonProperty] public string Label { get; set; } = Empty;
    }

    public AppData()
    {
        UserAccount = new Account();
        UserRPCConfig = new RPCConfig();
        LoveTrackHotkey = Empty;
    }
}
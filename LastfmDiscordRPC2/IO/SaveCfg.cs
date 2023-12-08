using System;
using LastfmDiscordRPC2.Models;
using Newtonsoft.Json;
using static LastfmDiscordRPC2.DataTypes.SaveVars;

namespace LastfmDiscordRPC2.IO;

public record SaveCfg
{
    public SaveCfg() { }

    public SaveCfg(SaveCfg other)
    {
        UserAccount = other.UserAccount;
        UserRPCCfg = other.UserRPCCfg;
    }

    [JsonProperty] public Account UserAccount { get; set; } = new Account();
    [JsonProperty] public RPCCfg UserRPCCfg { get; set; } = new RPCCfg();

    public record Account
    {
        [JsonProperty] public string Username { get; set; } = Empty;
        [JsonProperty] public string SessionKey { get; set; } = Empty;

        public bool IsValid() => !(SessionKey == "" || Username == "");
    }

    public record RPCCfg
    {
        [JsonProperty] public string AppID { get; set; } = DefaultAppID;
        [JsonProperty] public string Details { get; set; } = DefaultDetails;
        [JsonProperty] public string State { get; set; } = DefaultState;
        [JsonProperty] public string LargeImageLabel { get; set; } = DefaultLargeImageLabel;
        [JsonProperty] public string SmallImageLabel { get; set; } = DefaultSmallImageLabel;
        [JsonProperty] public RPCButton[] UserButtons { get; set; } = { new RPCButton() };
        [JsonProperty] public bool ExpiryMode { get; set; } = DefaultExpiryMode;
        [JsonProperty] public TimeSpan ExpiryTime { get; set; } = DefaultExpiryTime;
    }
}
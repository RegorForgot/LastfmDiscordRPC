using Newtonsoft.Json;
using static LastfmDiscordRPC2.Utilities.DiscordClientExtensions.ParsingStrings;

namespace LastfmDiscordRPC2.IO;

public record SaveCfg
{
    internal const string? DefaultAppID = "997756398664421446";

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
        [JsonProperty] public string? AppID { get; set; } = DefaultAppID;
        [JsonProperty] public int SleepTime { get; set; } = 3600;
        [JsonProperty] public string? Details { get; set; } = $"🎵 {TrackName}";
        [JsonProperty] public string? State { get; set; } = $"{ArtistName} | 💿 {AlbumName}";
        [JsonProperty] public string? LargeImageText { get; set; } = AlbumName;
        [JsonProperty] public string? SmallImageText { get; set; } = $"{Timestamp}{CurrentState}";
        [JsonProperty] public RPCButton[] UserButtons { get; set; } = { new RPCButton() };
    }

    public record RPCButton
    {
        [JsonProperty] public string? Link { get; set; } = $"https://www.last.fm/music/{ArtistName}/_/{TrackName}";
        [JsonProperty] public string? Label { get; set; } = $"{PlayCount} plays";
    }
}
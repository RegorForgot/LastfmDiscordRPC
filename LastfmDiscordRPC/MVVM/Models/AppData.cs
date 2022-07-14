using Newtonsoft.Json;

namespace LastfmDiscordRPC.MVVM.Models;

public class AppData
{
    [JsonProperty] public string Username { get; set; } = null!;
    [JsonProperty] public string APIKey { get; set; } = null!;
}
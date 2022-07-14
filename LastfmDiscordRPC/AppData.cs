using Newtonsoft.Json;

namespace LastfmDiscordRPC;

public class AppData
{
    [JsonProperty] public string Username { get; set; } = null!;
    [JsonProperty] public string APIKey { get; set; } = null!;
}
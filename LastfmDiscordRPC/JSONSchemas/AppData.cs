using Newtonsoft.Json;

namespace LastfmDiscordRPC.JSONSchemas;

public class AppData
{
    [JsonProperty] public string Username { get; set; }
    [JsonProperty] public string APIKey { get; set; }
}
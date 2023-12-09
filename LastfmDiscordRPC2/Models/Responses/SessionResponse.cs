using Newtonsoft.Json;

namespace LastfmDiscordRPC2.Models.Responses;

public record SessionResponse : ILastfmAPIResponse
{
    [JsonProperty("session")] public Session LfmSession { get; init; }
    
    public record Session 
    {
        [JsonProperty("name")] public string Username { get; init; } = Empty;
        [JsonProperty("key")] public string SessionKey { get; init; } = Empty;
    }
}

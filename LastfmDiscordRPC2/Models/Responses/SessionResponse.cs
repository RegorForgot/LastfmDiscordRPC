using Newtonsoft.Json;

namespace LastfmDiscordRPC2.Models.Responses;

public class SessionResponse : ILastfmResponse
{
    [JsonProperty("session")] public Session LfmSession { get; set; }
    
    public class Session
    {
        [JsonProperty("name")] public string Username { get; set; } = Empty;
        [JsonProperty("key")] public string SessionKey { get; set; } = Empty;
    }
}

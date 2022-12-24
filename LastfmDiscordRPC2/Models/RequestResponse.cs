using Newtonsoft.Json;

namespace LastfmDiscordRPC2.Models;

public interface ILastfmResponse {}

public class TokenResponse : ILastfmResponse
{
    [JsonProperty("token")] public string? Token { get; set; }
}

public class SessionResponse : ILastfmResponse
{
    [JsonProperty("session")] public Session LfmSession { get; set; }
    
    public class Session
    {
        [JsonProperty("name")] public string? Username { get; set; }
        [JsonProperty("key")] public string? SessionKey { get; set; }
    }
}


using Newtonsoft.Json;

namespace LastfmDiscordRPC2.Models.Responses;

public class TokenResponse : ILastfmResponse
{
    [JsonProperty("token")] public string Token { get; set; } = Empty;
}

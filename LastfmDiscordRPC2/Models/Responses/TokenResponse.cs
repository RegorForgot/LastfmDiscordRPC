using LastfmDiscordRPC2.Models.API;
using Newtonsoft.Json;

namespace LastfmDiscordRPC2.Models.Responses;

public class TokenResponse : ILastfmAPIResponse
{
    [JsonProperty("token")] public string Token { get; set; } = Empty;
}

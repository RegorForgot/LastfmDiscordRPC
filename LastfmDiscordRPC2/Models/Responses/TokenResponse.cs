using Newtonsoft.Json;

namespace LastfmDiscordRPC2.Models.Responses;

public record TokenResponse : ILastfmAPIResponse
{
    [JsonProperty("token")] public string Token { get; init; } = Empty;
}

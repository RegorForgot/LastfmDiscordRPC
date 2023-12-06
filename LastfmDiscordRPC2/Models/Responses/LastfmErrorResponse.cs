using LastfmDiscordRPC2.DataTypes;
using Newtonsoft.Json;

namespace LastfmDiscordRPC2.Models.Responses;

public record LastfmErrorResponse : ILastfmAPIResponse
{
    [JsonProperty("message")] public string Message { get; init; } = "OK";
    [JsonProperty("error")] public LastfmErrorCode Error { get; init; } = LastfmErrorCode.OK;
}
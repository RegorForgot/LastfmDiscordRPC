using LastfmDiscordRPC2.Enums;
using Newtonsoft.Json;

namespace LastfmDiscordRPC2.Models.Responses;

public class LastfmErrorResponse : ILastfmAPIResponse
{
    [JsonProperty("message")] public string Message { get; set; } = "OK";
    [JsonProperty("error")] public LastfmErrorCode Error { get; set; } = LastfmErrorCode.OK;
}
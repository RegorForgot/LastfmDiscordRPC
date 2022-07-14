using LastfmAPI.Enums;
using Newtonsoft.Json;

namespace LastfmAPI.Responses;

public class LastfmError : IResponse
{
    [JsonProperty("message")] public string? Message { get; set; }
    [JsonProperty("error")] public ErrorEnum Error { get; set; } = ErrorEnum.OK;
} 
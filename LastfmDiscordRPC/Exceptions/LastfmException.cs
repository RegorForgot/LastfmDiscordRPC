using System;
using Newtonsoft.Json;

namespace LastfmDiscordRPC.Exceptions;

public class LastfmException : Exception
{
    public override string Message { get; }
    public ErrorEnum ErrorCode { get; }
    
    public LastfmException(string message, ErrorEnum code)
    {
        Message = $"{code.ToString()}: {message}";
        ErrorCode = code;
    }
    
    public class LastfmError
    {
        [JsonProperty("message")] public string Message { get; set; } = "OK";
        [JsonProperty("error")] public ErrorEnum Error { get; set; } = ErrorEnum.OK;
    } 
    
    public enum ErrorEnum
    {
        OK = 0,
        InvalidService = 2,
        InvalidMethod = 3,
        AuthFailed = 4,
        InvalidFormat = 5,
        InvalidParam = 6,
        InvalidRes = 7,
        OperationFail = 8,
        InvalidSession = 9,
        InvalidKey = 10,
        Offline = 11,
        InvalidSignature = 13,
        Temporary = 16,
        SuspendedKey = 26,
        RateLimit = 29
    }
}

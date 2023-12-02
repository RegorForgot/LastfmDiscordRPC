using System;
using LastfmDiscordRPC2.Enums;

namespace LastfmDiscordRPC2.Exceptions;

public sealed class LastfmException : Exception
{
    public override string Message { get; }
    public LastfmErrorCode ErrorCode { get; }

    public LastfmException() : base()
    {
        ErrorCode = LastfmErrorCode.Unknown;
        Message = $"{ErrorCode.ToString()}";
    }
    
    public LastfmException(string message, LastfmErrorCode code)
    {
        Message = $"{code.ToString()}: {message}";
        ErrorCode = code;
    }
}

using System;
using LastfmDiscordRPC2.DataTypes;

namespace LastfmDiscordRPC2.Exceptions;

public sealed class LastfmException : Exception
{
    public override string Message { get; }
    public LastfmErrorCodeEnum ErrorCodeEnum { get; }

    public LastfmException() : base()
    {
        ErrorCodeEnum = LastfmErrorCodeEnum.Unknown;
        Message = $"{ErrorCodeEnum.ToString()}";
    }
    
    public LastfmException(string message, LastfmErrorCodeEnum codeEnum)
    {
        Message = $"{codeEnum.ToString()}: {message}";
        ErrorCodeEnum = codeEnum;
    }
}

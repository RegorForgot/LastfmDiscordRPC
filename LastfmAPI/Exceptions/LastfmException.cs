using LastfmAPI.Enums;

namespace LastfmAPI.Exceptions;

public class LastfmException : Exception
{
    public override string Message { get; }
    public ErrorEnum ErrorCode { get; }
    
    public LastfmException(string message, ErrorEnum code)
    {
        Message = $"{code.ToString()}: {message}";
        ErrorCode = code;
    }
}

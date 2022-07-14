// ReSharper disable All
namespace LastfmAPI.Enums;

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
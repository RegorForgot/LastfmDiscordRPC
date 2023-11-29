using DiscordRPC.Logging;
using LastfmDiscordRPC2.IO;

namespace LastfmDiscordRPC2.Logging;

public class TextLogger : IRPCLogger
{
    private readonly LogIOService _logIOService;
    public LogLevel Level { get; set; }

    public TextLogger(LogLevel level, LogIOService logIOService)
    {
        Level = level;
        _logIOService = logIOService;
    }
    
    public void Trace(string message, params object[] args)
    {
        if (Level > LogLevel.Trace)
        {
            return;
        }
        
        _logIOService.Log(IRPCLogger.GetLoggingString(LogLevel.Trace, message, args));
    }

    public void Info(string message, params object[] args)
    {
        if (Level > LogLevel.Info)
        {
            return;
        }
        
        _logIOService.Log(IRPCLogger.GetLoggingString(LogLevel.Info, message, args));
    }

    public void Warning(string message, params object[] args)
    {
        if (Level > LogLevel.Warning)
        {
            return;
        }
        
        _logIOService.Log(IRPCLogger.GetLoggingString(LogLevel.Warning, message, args));
    }

    public void Error(string message, params object[] args)
    {
        if (Level > LogLevel.Error)
        {
            return;
        }
        
        _logIOService.Log(IRPCLogger.GetLoggingString(LogLevel.Error, message, args));
    }
}
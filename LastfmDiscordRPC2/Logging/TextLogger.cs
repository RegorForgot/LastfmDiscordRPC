using DiscordRPC.Logging;
using LastfmDiscordRPC2.IO;

namespace LastfmDiscordRPC2.Logging;

public class TextLogger : IRPCLogger
{
    private readonly LogFileIO _logFileIO;
    public LogLevel Level { get; set; }

    public TextLogger(LogLevel level, LogFileIO logFileIO)
    {
        Level = level;
        _logFileIO = logFileIO;
    }
    
    public void Trace(string message, params object[] args)
    {
        if (Level > LogLevel.Trace)
        {
            return;
        }
        
        _logFileIO.Log(IRPCLogger.GetLoggingString(LogLevel.Trace, message, args));
    }

    public void Info(string message, params object[] args)
    {
        if (Level > LogLevel.Info)
        {
            return;
        }
        
        _logFileIO.Log(IRPCLogger.GetLoggingString(LogLevel.Info, message, args));
    }

    public void Warning(string message, params object[] args)
    {
        if (Level > LogLevel.Warning)
        {
            return;
        }
        
        _logFileIO.Log(IRPCLogger.GetLoggingString(LogLevel.Warning, message, args));
    }

    public void Error(string message, params object[] args)
    {
        if (Level > LogLevel.Error)
        {
            return;
        }
        
        _logFileIO.Log(IRPCLogger.GetLoggingString(LogLevel.Error, message, args));
    }
}
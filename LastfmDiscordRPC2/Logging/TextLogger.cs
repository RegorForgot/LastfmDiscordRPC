using DiscordRPC.Logging;
using LastfmDiscordRPC2.IO;

namespace LastfmDiscordRPC2.Logging;

public class TextLogger : IRPCLogger
{
    private readonly LogFileIO _logFileIO;
    private readonly ViewLogger _viewLogger;
    public LogLevel Level { get; set; }

    public TextLogger(LogLevel level, LogFileIO logFileIO, ViewLogger viewLogger)
    {
        Level = level;
        _logFileIO = logFileIO;
        _viewLogger = viewLogger;
    }
    
    public void Trace(string message, params object[] args)
    {
        if (Level > LogLevel.Trace)
        {
            return;
        }
        
        _viewLogger.Trace(message, args);
        _logFileIO.Log(IRPCLogger.GetLoggingString(LogLevel.Trace, message, args));
    }

    public void Info(string message, params object[] args)
    {
        if (Level > LogLevel.Info)
        {
            return;
        }
        
        _viewLogger.Info(message, args);
        _logFileIO.Log(IRPCLogger.GetLoggingString(LogLevel.Info, message, args));
    }

    public void Warning(string message, params object[] args)
    {
        if (Level > LogLevel.Warning)
        {
            return;
        }
        
        _viewLogger.Warning(message, args);
        _logFileIO.Log(IRPCLogger.GetLoggingString(LogLevel.Warning, message, args));
    }

    public void Error(string message, params object[] args)
    {
        if (Level > LogLevel.Error)
        {
            return;
        }
        
        _viewLogger.Error(message, args);
        _logFileIO.Log(IRPCLogger.GetLoggingString(LogLevel.Error, message, args));
    }
}
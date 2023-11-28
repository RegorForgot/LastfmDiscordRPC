using System.IO;
using DiscordRPC.Logging;
using LastfmDiscordRPC2.Models;

namespace LastfmDiscordRPC2.Logging;

public class TextLogger : IRPCLogger
{
    private readonly ViewLogger _viewLogger;
    private readonly string _filePath;
    private readonly object _fileLock = new object();
    
    public LogLevel Level { get; set; }

    public TextLogger(LogLevel level, ViewLogger viewLogger)
    {
        Level = level;
        
        _viewLogger = viewLogger;
        _filePath = Utilities.SaveAppData.FolderPath + "/Log.log";
        
        using (File.Create(_filePath))
        {
            // Doesn't need anything
        }
    }

    private void WriteToFile(string msg)
    {
        lock (_fileLock)
        {
            if (File.Exists(_filePath))
            {
                File.AppendAllText(_filePath, msg);
            } else
            {
                File.WriteAllText(_filePath, msg);
            }
        }
    }
    
    public void Trace(string message, params object[] args)
    {
        if (Level > LogLevel.Trace)
        {
            return;
        }
        
        _viewLogger.Trace(message, args);
        WriteToFile(IRPCLogger.GetLoggingString(LogLevel.Trace, message, args));
    }

    public void Info(string message, params object[] args)
    {
        if (Level > LogLevel.Info)
        {
            return;
        }
        
        _viewLogger.Info(message, args);
        WriteToFile(IRPCLogger.GetLoggingString(LogLevel.Info, message, args));
    }

    public void Warning(string message, params object[] args)
    {
        if (Level > LogLevel.Warning)
        {
            return;
        }
        
        _viewLogger.Warning(message, args);
        WriteToFile(IRPCLogger.GetLoggingString(LogLevel.Warning, message, args));
    }

    public void Error(string message, params object[] args)
    {
        if (Level > LogLevel.Error)
        {
            return;
        }
        
        _viewLogger.Error(message, args);
        WriteToFile(IRPCLogger.GetLoggingString(LogLevel.Error, message, args));
    }
}
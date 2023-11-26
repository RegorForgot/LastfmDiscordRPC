using System.IO;
using DiscordRPC.Logging;
using static System.DateTime;

namespace LastfmDiscordRPC2.Models;

public class Logger : ILogger
{
    private readonly string _filePath;
    private readonly object _fileLock = new object();
    public LogLevel Level { get; set; }

    public Logger(LogLevel level)
    {
        Level = level;
        _filePath = Utilities.SaveAppData.FolderPath + @"/errorLog.log";
        
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
    private static string GetCurrentTimeString()
    {
        return Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
    
    #region Message Methods

    public void Trace(string message, params object[] args)
    {
        if (Level > LogLevel.Trace)
        {
            return;
        }

        string msgText = @$"\n+ [{GetCurrentTimeString()}] ""TRCE"": " +
                         @$"{(args.Length != 0 ? Format(message, args) : message)}";
        WriteToFile(msgText);
    }

    public void Info(string message, params object[] args)
    {
        if (Level > LogLevel.Info)
        {
            return;
        }
        string msgText = @$"\n+ [{GetCurrentTimeString()}] ""INFO"": " +
                         @$"{(args.Length != 0 ? Format(message, args) : message)}";
        WriteToFile(msgText);
    }

    public void Warning(string message, params object[] args)
    {
        if (Level > LogLevel.Warning)
        {
            return;
        }
        string msgText = @$"\n+ [{GetCurrentTimeString()}] ""WARN"": " +
                         @$"{(args.Length != 0 ? Format(message, args) : message)}";
        WriteToFile(msgText);
    }

    public void Error(string message, params object[] args)
    {
        if (Level > LogLevel.Error)
        {
            return;
        }
        string msgText = @$"\n+ [{GetCurrentTimeString()}] ""ERR "": " +
                         @$"{(args.Length != 0 ? Format(message, args) : message)}";
        WriteToFile(msgText);
    }

    #endregion
}
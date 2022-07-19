using System.IO;
using DiscordRPC.Logging;
using LastfmDiscordRPC.ViewModels;
using static System.DateTime;

namespace LastfmDiscordRPC.Models;

public class Logger : ILogger
{
    private readonly MainViewModel _mainViewModel;
    private readonly object _fileLock = new object();
    private readonly string _filePath;
    public LogLevel Level { get; set; }

    public Logger(string filePath, LogLevel level, MainViewModel mainViewModel)
    {
        Level = level;
        _filePath = $"{filePath}";
        _mainViewModel = mainViewModel;

        SaveAppData.CheckFolderExists();

        using (FileStream fs = File.Create(_filePath))
        {
            // Doesn't need anything
        }
    }

    private void WriteToFile(string errorLevel, LogLevel level, string message, params object[] args)
    {
        SaveAppData.CheckFolderExists();
        string timeString = GetCurrentTimeString();
        string colourCode = GetColourCode(level);
        string errorMessage = $"\n+ [[34m{timeString}[0m] {colourCode}{errorLevel}[0m: " +
                              $"{(args.Length != 0 ? Format(message, args) : message)}";

        lock (_fileLock)
        {
            if (File.Exists(_filePath))
            {
                File.AppendAllText(_filePath, errorMessage);
            } else
            {
                File.WriteAllText(_filePath, errorMessage);
            }
        }

        string GetColourCode(LogLevel logLevel)
        {
            return logLevel switch
            {
                LogLevel.Error => "[31m",
                LogLevel.Warning => "[33m",
                LogLevel.Info => "[36m",
                _ => "[32m"
            };
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
        WriteToFile("TRCE", LogLevel.Trace, message, args);
    }

    public void Info(string message, params object[] args)
    {
        if (Level > LogLevel.Info)
        {
            return;
        }
        WriteToFile("INFO", LogLevel.Info, message, args);
    }

    public void Warning(string message, params object[] args)
    {
        if (Level > LogLevel.Warning)
        {
            return;
        }
        WriteToFile("WARN", LogLevel.Warning, message, args);
    }

    public void Error(string message, params object[] args)
    {
        if (Level > LogLevel.Error)
        {
            return;
        }
        WriteToFile("ERR ", LogLevel.Error, message, args);
    }

    public void ErrorOverride(string message, params object[] args)
    {
        string timeString = GetCurrentTimeString();
        _mainViewModel.OutputText = $"\n+ [{timeString}] {"ERR "}: " +
                                    $"{(args.Length != 0 ? Format(message, args) : message)}";
        WriteToFile("ERR ", LogLevel.Error, message, args);
    }

    public void WarningOverride(string message, params object[] args)
    {
        string timeString = GetCurrentTimeString();
        _mainViewModel.OutputText = $"\n+ [{timeString}] {"WARN"}: " +
                                    $"{(args.Length != 0 ? Format(message, args) : message)}";
        WriteToFile("WARN", LogLevel.Warning, message, args);
    }

    public void InfoOverride(string message, params object[] args)
    {
        string timeString = GetCurrentTimeString();
        _mainViewModel.OutputText = $"\n+ [{timeString}] {"INFO"}: " +
                                    $"{(args.Length != 0 ? Format(message, args) : message)}";
        WriteToFile("INFO", LogLevel.Info, message, args);
    }

    #endregion
}
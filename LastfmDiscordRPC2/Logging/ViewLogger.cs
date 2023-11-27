using DiscordRPC.Logging;
using LastfmDiscordRPC2.ViewModels;
using LastfmDiscordRPC2.ViewModels.Panes;

namespace LastfmDiscordRPC2.Logging;

public class ViewLogger : ILastfmLogger
{
    private readonly SettingsConsoleViewModel _loggingControlViewModel;
    public LogLevel Level { get; set; }
    
    public ViewLogger(LogLevel level, SettingsConsoleViewModel loggingControlViewModel)
    {
        _loggingControlViewModel = loggingControlViewModel;
        Level = level;
    }
    
    
    public void Trace(string message, params object[] args)
    {
        if (Level > LogLevel.Trace)
        {
            return;
        }

        string msgText = @$"\n+ [{ILastfmLogger.GetCurrentTimeString()}] ""TRCE"": " +
                         $"{(args.Length != 0 ? Format(message, args) : message)}";
        
        _loggingControlViewModel.ConsoleText = msgText;
    }
    
    public void Info(string message, params object[] args)
    {
        if (Level > LogLevel.Info)
        {
            return;
        }
        
        string msgText = @$"\n+ [{ILastfmLogger.GetCurrentTimeString()}] ""INFO"": " +
                         $"{(args.Length != 0 ? Format(message, args) : message)}";
        
        _loggingControlViewModel.ConsoleText = msgText;
    }
    
    public void Warning(string message, params object[] args)
    {
        if (Level > LogLevel.Warning)
        {
            return;
        }
        
        string msgText = @$"\n+ [{ILastfmLogger.GetCurrentTimeString()}] ""WARN"": " +
                         $"{(args.Length != 0 ? Format(message, args) : message)}";
        
        _loggingControlViewModel.ConsoleText = msgText;
    }
    
    public void Error(string message, params object[] args)
    {
        if (Level > LogLevel.Error)
        {
            return;
        }

        string msgText = @$"\n+ [{ILastfmLogger.GetCurrentTimeString()}] ""ERR "": " +
                         $"{(args.Length != 0 ? Format(message, args) : message)}";
        
        _loggingControlViewModel.ConsoleText = msgText;
    }
}
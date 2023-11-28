using DiscordRPC.Logging;
using LastfmDiscordRPC2.ViewModels.Controls;

namespace LastfmDiscordRPC2.Logging;

public class ViewLogger : IRPCLogger
{
    private readonly AbstractLoggingControlViewModel _loggingControlViewModel;
    public LogLevel Level { get; set; }
    
    public ViewLogger(LogLevel level, AbstractLoggingControlViewModel loggingControlViewModel)
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

        _loggingControlViewModel.ConsoleText += IRPCLogger.GetLoggingString(LogLevel.Trace, message, args);
    }
    
    public void Info(string message, params object[] args)
    {
        if (Level > LogLevel.Info)
        {
            return;
        }
        
        _loggingControlViewModel.ConsoleText += IRPCLogger.GetLoggingString(LogLevel.Info, message, args);
    }
    
    public void Warning(string message, params object[] args)
    {
        if (Level > LogLevel.Warning)
        {
            return;
        }
        
        _loggingControlViewModel.ConsoleText += IRPCLogger.GetLoggingString(LogLevel.Warning, message, args);
    }
    
    public void Error(string message, params object[] args)
    {
        if (Level > LogLevel.Error)
        {
            return;
        }

        _loggingControlViewModel.ConsoleText += IRPCLogger.GetLoggingString(LogLevel.Error, message, args);
    }
}
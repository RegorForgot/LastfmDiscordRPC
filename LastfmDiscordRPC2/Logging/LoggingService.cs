using System.Collections.Generic;
using System.Linq;
using DiscordRPC.Logging;

namespace LastfmDiscordRPC2.Logging;

public sealed class LoggingService : ILogger
{
    private readonly IEnumerable<IRPCLogger> _loggers;
    public LogLevel Level { get; set; } = LogLevel.None;

    public LoggingService(IEnumerable<IRPCLogger> loggers)
    {
        _loggers = loggers;
    }

    public void Trace(string message, params object[] args)
    {
        foreach (IRPCLogger logger in _loggers)
        {
            logger.Trace(message, args);
        }
    }
    
    public void Info(string message, params object[] args)
    {
        foreach (IRPCLogger logger in _loggers.Where(_ => !message.Contains("Handling Response")))
        {
            logger.Info(message, args);
        }
    }
    
    public void Warning(string message, params object[] args)
    {
        foreach (IRPCLogger logger in _loggers)
        {
            logger.Warning(message, args);
        }
    }
    
    public void Error(string message, params object[] args)
    {
        foreach (IRPCLogger logger in _loggers)
        {
            logger.Error(message, args);
        }
    }
}
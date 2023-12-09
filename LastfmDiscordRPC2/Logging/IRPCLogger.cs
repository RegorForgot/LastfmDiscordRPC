using System;
using DiscordRPC.Logging;

namespace LastfmDiscordRPC2.Logging;

public interface IRPCLogger : ILogger
{
    public static string GetLoggingString(LogLevel level, string message, params object[] args)
    {

        string msgText = $"+ [{GetCurrentTimeString()}] {GetLevelText(level)}: " +
                         $"{(args.Length != 0 ? Format(message, args) : message)}\n";

        return msgText;

        string GetCurrentTimeString()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        string GetLevelText(LogLevel logLevel)
        {
            string levelText = logLevel switch
            {
                LogLevel.Error => "ERR ",
                LogLevel.Warning => "WARN",
                LogLevel.Info => "INFO",
                LogLevel.Trace => "TRCE",
                _ => "    "
            };
            
            return levelText;
        }
    }
}
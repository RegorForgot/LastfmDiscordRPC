using System;
using DiscordRPC.Logging;

namespace LastfmDiscordRPC2.Logging;

public interface ILastfmLogger : ILogger
{
    public static string GetCurrentTimeString()
    {
        return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}
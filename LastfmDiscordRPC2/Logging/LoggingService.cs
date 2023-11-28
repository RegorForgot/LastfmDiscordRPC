using System.Collections.Generic;
using System.Linq;

namespace LastfmDiscordRPC2.Logging;

public class LoggingService : AbstractLoggingService
{
    public LoggingService(IEnumerable<IRPCLogger> loggers) : base(loggers) { }
}
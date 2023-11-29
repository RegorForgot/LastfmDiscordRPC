using System.IO;

namespace LastfmDiscordRPC2.IO;

public sealed class LogIOService : AbstractIOService
{
    public override string FilePath { get; protected set; } = $"{SaveFolder}/log.log";

    public LogIOService()
    {
        if (!FileExists())
        {
            using (File.Create(FilePath)) { }
        }
        AppendToFile("------------------------\n");
    }

    public void Log(string message)
    {
        AppendToFile(message);
    }
}
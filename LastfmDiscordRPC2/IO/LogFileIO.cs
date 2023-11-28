using System.IO;

namespace LastfmDiscordRPC2.IO;

public sealed class LogFileIO : AbstractFileIO
{
    public override string FilePath { get; protected set; } = $"{SaveFolder}/log.log";

    public LogFileIO()
    {
        if (!FileExists())
        {
            using (File.Create(FilePath)) { }
        }
        AppendToFile("------------------------");
    }

    public void Log(string message)
    {
        AppendToFile(message);
    }
}
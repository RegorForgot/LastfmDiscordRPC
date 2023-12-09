using System.IO;

namespace LastfmDiscordRPC2.IO;

public sealed class LogIOService : AbstractIOService
{
    public override string FilePath { get; protected set; } = $"{SaveFolder}/log.log";
    private const long OneMegabyte = 1 << 20;

    public LogIOService()
    {
        if (!FileExists())
        {
            using (File.Create(FilePath)) { }
        }
        
        long size = new FileInfo(FilePath).Length;
        if (size > OneMegabyte)
        {
            WriteToFile(Empty);
        }

        AppendToFile("------------------------\n");
    }

    public void Log(string message)
    {
        AppendToFile(message);
    }
}
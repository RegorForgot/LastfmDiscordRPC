using LastfmDiscordRPC2.IO.Schema;
using LastfmDiscordRPC2.Logging;

namespace LastfmDiscordRPC2.IO;

public class SaveDataFileIO : AbstractConfigFileIO<SaveData>
{
    public override string FilePath { get; protected set; } = $"{SaveFolder}/config.json";
    
    public SaveDataFileIO(AbstractLoggingService loggingService) : base(loggingService) { }
}
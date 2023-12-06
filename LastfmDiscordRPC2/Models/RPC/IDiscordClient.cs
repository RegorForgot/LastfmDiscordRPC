using System.Threading.Tasks;
using LastfmDiscordRPC2.IO;
using LastfmDiscordRPC2.Models.Responses;

namespace LastfmDiscordRPC2.Models.RPC;

public interface IDiscordClient
{
    public void Initialize(SaveCfg saveCfg);
    public void SetPresence(TrackResponse response);
    public void ClearPresence();
    public bool IsReady { get; }
}
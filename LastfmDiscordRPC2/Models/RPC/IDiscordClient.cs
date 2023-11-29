using LastfmDiscordRPC2.Models.Responses;

namespace LastfmDiscordRPC2.Models.RPC;

public interface IDiscordClient
{
    public void Initialize();
    public void SetPresence(TrackResponse response);
    public void ClearPresence();
    public void Deinitialize();
    public bool IsReady { get; protected set; }
}
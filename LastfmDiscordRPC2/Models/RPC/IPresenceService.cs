using System;
using LastfmDiscordRPC2.Models.Responses;

namespace LastfmDiscordRPC2.Models.RPC;

public interface IPresenceService : IDisposable
{
    public void SetPresence();
    protected void UpdatePresence(TrackResponse trackResponse);
    public void ClearPresence();
}
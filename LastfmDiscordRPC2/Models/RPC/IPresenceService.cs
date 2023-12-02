using System;
using System.Threading.Tasks;

namespace LastfmDiscordRPC2.Models.RPC;

public interface IPresenceService : IDisposable
{
    public Task SetPresence();
    public void ClearPresence();
}
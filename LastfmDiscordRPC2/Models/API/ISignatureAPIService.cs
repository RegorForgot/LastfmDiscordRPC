using System.Threading.Tasks;

namespace LastfmDiscordRPC2.Models.API;

public interface ISignatureAPIService : IAPIService
{
    public Task<string> GetSignature(string message);
}
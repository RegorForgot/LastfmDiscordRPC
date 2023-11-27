using System.Threading.Tasks;

namespace LastfmDiscordRPC2.Models.API;

public interface ISignatureAPIClient : IAPIClient
{
    public Task<string> GetSignature(string message);
}
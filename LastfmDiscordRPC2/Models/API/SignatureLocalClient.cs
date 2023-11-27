using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using LastfmDiscordRPC2.Assets;
using RestSharp;

namespace LastfmDiscordRPC2.Models.API;

public class SignatureLocalClient : ISignatureAPIClient
{
    public RestClient APIRestClient { get; }

    public SignatureLocalClient()
    {
        APIRestClient = new RestClient();
    }

    public Task<string> GetSignature(string message)
    {
        using MD5 hash = MD5.Create();
        string input = message + SecretKey.Secret;

        byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
        byte[] hashBytes = hash.ComputeHash(inputBytes);
        return Task.FromResult(Convert.ToHexString(hashBytes));
    }

    public void Dispose()
    {
        APIRestClient.Dispose();
    }
}
using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using RestSharp;

namespace LastfmDiscordRPC2.Models.API;

public class SignatureLocalAPIService : ISignatureAPIService
{
    public RestClient APIRestClient { get; }
    private readonly ISecretKey _secretKey;

    public SignatureLocalAPIService(ISecretKey secretKey)
    {
        _secretKey = secretKey;
        APIRestClient = new RestClient();
    }

    public Task<string> GetSignature(string message)
    {
        using MD5 hash = MD5.Create();
        string input = message + _secretKey.Secret;

        byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
        byte[] hashBytes = hash.ComputeHash(inputBytes);
        return Task.FromResult(Convert.ToHexString(hashBytes));
    }

    public void Dispose()
    {
        APIRestClient.Dispose();
    }
}
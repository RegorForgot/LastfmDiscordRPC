using System;
using System.Net.Http;
using System.Threading.Tasks;
using RestSharp;

namespace LastfmDiscordRPC2.Models.API;

public sealed class SignatureAPIService : ISignatureAPIService
{
    public RestClient APIRestClient { get; init; }

    public SignatureAPIService()
    {
        APIRestClient = new RestClient(@"https://crygup.com/regor/");
        APIRestClient.AddDefaultHeader("User-Agent", "LastfmDiscordRPC 2.0.0");
    }
    
    public async Task<string> GetSignature(string message)
    {
        RestRequest request = new RestRequest("", Method.Post);

        // this was a pain in the ass to write
        string json = @"{""string"": """ + message + @"""}";

        request.AddParameter("application/json", json, ParameterType.RequestBody);
        request.RequestFormat = DataFormat.Json;
        request.Timeout = 20000;

        RestResponse response = await APIRestClient.ExecuteAsync(request);
        if (response.Content != null)
        {
            return response.Content;
        }

        throw new HttpRequestException(Enum.GetName(response.StatusCode), null, response.StatusCode);
    }
    
    public void Dispose()
    {
        APIRestClient.Dispose();
    }
}
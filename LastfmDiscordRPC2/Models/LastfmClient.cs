using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;
using static LastfmDiscordRPC2.Models.LastfmException;

namespace LastfmDiscordRPC2.Models;

public class LastfmClient : IDisposable
{
    private readonly RestClient _client;
    
    public LastfmClient()
    {
        _client = new RestClient(@"https://ws.audioscrobbler.com/2.0/");
        _client.AddDefaultHeader("User-Agent", "LastfmDiscordRPC 1.1.1");
    }

    public async Task<LastfmResponse> CallAPI(string username, string apiKey)
    {
        
        RestRequest request = new RestRequest();

        request.AddParameter("format", "json");
        request.AddParameter("method", "user.getrecenttracks");
        request.AddParameter("limit", "1");
        request.AddParameter("user", username);
        request.AddParameter("api_key", apiKey);
        request.Timeout = 5000;

        RestResponse response = await _client.ExecuteAsync(request);

        if (response.Content != null)
        {
            return GetResponse(response.Content);
        }
        throw new HttpRequestException(Enum.GetName(response.StatusCode), null, response.StatusCode);
    }

    private static LastfmResponse GetResponse(string response)
    {
        LastfmError e = JsonConvert.DeserializeObject<LastfmError>(response)!;
        if (e.Error == ErrorEnum.OK)
        {
            return JsonConvert.DeserializeObject<LastfmResponse>(response)!;
        }
        throw new LastfmException(e.Message, e.Error);
    }

    public void Dispose()
    {
        _client.Dispose();   
        SuppressFinalize(this);
    }
}
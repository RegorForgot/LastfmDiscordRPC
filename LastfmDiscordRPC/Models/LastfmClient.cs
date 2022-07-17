using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;
using static LastfmDiscordRPC.Models.LastfmException;

namespace LastfmDiscordRPC.Models;

public static class LastfmClient
{
    private readonly static RestClient Client;

    static LastfmClient()
    {
        Client = new RestClient(@"https://ws.audioscrobbler.com/2.0/");
        Client.AddDefaultHeader("User-Agent", "LastfmDiscordRPC 1.0.0");
    }

    public static async Task<LastfmResponse> CallAPI(string username, string apiKey)
    {
        RestRequest request = new RestRequest();

        request.AddParameter("format", "json");
        request.AddParameter("method", "user.getrecenttracks");
        request.AddParameter("limit", "1");
        request.AddParameter("user", username);
        request.AddParameter("api_key", apiKey);

        RestResponse response = await Client.ExecuteAsync(request);
        if (response.Content != null) return GetResponse(response.Content);
        throw new HttpRequestException(Enum.GetName(response.StatusCode), null, response.StatusCode);
    }

    private static LastfmResponse GetResponse(string response)
    {
        LastfmError e = JsonConvert.DeserializeObject<LastfmError>(response)!;
        if (e.Error == ErrorEnum.OK) return JsonConvert.DeserializeObject<LastfmResponse>(response)!;
        throw new LastfmException(e.Message, e.Error);
    }
}
using System.Net;
using LastfmAPI.Responses;
using Newtonsoft.Json;
using RestSharp;

namespace LastfmAPI;

public class APIConnector
{
    private readonly static RestClient Client;
    public const string GetTracks = "user.getrecenttracks";
    public const string GetUser = "user.getinfo";

    static APIConnector()
    {
        Client = new RestClient(@"https://ws.audioscrobbler.com/2.0/");
        Client.AddDefaultHeader("User-Agent", "LastfmDiscordRPC 1.0.0");
    }

    public static IResponse CallAPI(string username, string apiKey, string method)
    {
        RestRequest request = new RestRequest();
        
        request.AddParameter("format", "json");
        request.AddParameter("method", method);
        request.AddParameter("user", username);
        request.AddParameter("api_key", apiKey);
        
        if (method == GetTracks) request.AddParameter("limit", "1");

        RestResponse response = Client.Execute(request);
        return response.Content != null ? GetResponse(response.Content, method) : new HTTPError(response.StatusCode);
    }

    private static IResponse GetResponse(string response, string method)
    {
        LastfmError? e = IsLastfmError(response);
        if (e != null) return e;
        
        if (method == GetTracks)
            return JsonConvert.DeserializeObject<TrackResponse>(response);
        return JsonConvert.DeserializeObject<UserResponse>(response);
    }
    
    // Returns the LastfmError if true, null if not.
    private static LastfmError? IsLastfmError(string response)
    {
        LastfmError e = JsonConvert.DeserializeObject<LastfmError>(response);
        return e.Message != null ? e : null;
    }
}
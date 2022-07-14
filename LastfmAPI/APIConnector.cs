using LastfmAPI.Enums;
using LastfmAPI.Exceptions;
using LastfmAPI.Responses;
using Newtonsoft.Json;
using RestSharp;

namespace LastfmAPI;

public static class APIConnector
{
    private readonly static RestClient Client;
    public const string GetTracks = "user.getrecenttracks";
    public const string GetUser = "user.getinfo";

    static APIConnector()
    {
        Client = new RestClient(@"https://ws.audioscrobbler.com/2.0/");
        Client.AddDefaultHeader("User-Agent", "LastfmDiscordRPC 1.0.0");
    }

    public static async Task<IResponse> CallAPI(string username, string apiKey, string method)
    {
        RestRequest request = new RestRequest();
        
        request.AddParameter("format", "json");
        request.AddParameter("method", method);
        request.AddParameter("user", username);
        request.AddParameter("api_key", apiKey);
        
        if (method == GetTracks) request.AddParameter("limit", "1");

        RestResponse response = await Client.ExecuteAsync(request);
        
        if (response.Content != null) return GetResponse(response.Content, method);
        throw new HttpRequestException(Enum.GetName(response.StatusCode), null, response.StatusCode);
    }

    private static IResponse GetResponse(string response, string method)
    {
        IsLastfmError(response);
        if (method == GetTracks) return JsonConvert.DeserializeObject<TrackResponse>(response)!;
        return JsonConvert.DeserializeObject<UserResponse>(response)!;
    }
    
    // Returns the LastfmError if true, null if not.
    private static void IsLastfmError(string response)
    {
        LastfmError e = JsonConvert.DeserializeObject<LastfmError>(response)!;
        if (e.Error == ErrorEnum.OK) return;
        throw new LastfmException(e.Message!, e.Error);
    }
}
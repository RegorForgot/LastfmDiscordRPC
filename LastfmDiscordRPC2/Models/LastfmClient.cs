using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;
using static LastfmDiscordRPC2.Models.LastfmException;

namespace LastfmDiscordRPC2.Models;

public class LastfmClient : IDisposable
{
    private readonly RestClient _lfmClient;
    private readonly RestClient _signatureClient;

    public LastfmClient()
    {
        _lfmClient = new RestClient(@"https://ws.audioscrobbler.com/2.0/");
        _lfmClient.AddDefaultHeader("User-Agent", "LastfmDiscordRPC 2.0.0");

        _signatureClient = new RestClient(@"https://regorforgot.000webhostapp.com");
        _signatureClient.AddDefaultHeader("User-Agent", "LastfmDiscordRPC 2.0.0");
    }

    public async Task<TokenResponse> GetToken()
    {
        RestRequest request = new RestRequest();

        request.AddParameter("format", "json");
        request.AddParameter("method", "auth.gettoken");
        request.AddParameter("api_key", Utilities.APIKey);
        request.Timeout = 20000;

        RestResponse response = await _lfmClient.ExecuteAsync(request);
        if (response.Content != null)
        {
            return GetResponse<TokenResponse>(response.Content);
        }

        throw new HttpRequestException(Enum.GetName(response.StatusCode), null, response.StatusCode);
    }

    public async Task<SessionResponse?> GetSession(string token)
    {
        RestRequest request = new RestRequest();

        request.AddParameter("format", "json");
        request.AddParameter("method", "auth.getsession");
        request.AddParameter("token", token);
        request.AddParameter("api_key", Utilities.APIKey);
        request.Timeout = 20000;

        string signature = await GetSignature($"api_key{Utilities.APIKey}methodauth.getsessiontoken{token}");
        request.AddParameter("api_sig", signature);

        RestResponse response = await _lfmClient.ExecuteAsync(request);
        if (response.Content != null)
        {
            return GetResponse<SessionResponse>(response.Content);
        }
        throw new HttpRequestException(Enum.GetName(response.StatusCode), null, response.StatusCode);
    }

    public async Task<TrackResponse> CallAPI(string username)
    {
        RestRequest request = new RestRequest();

        request.AddParameter("format", "json");
        request.AddParameter("method", "user.getrecenttracks");
        request.AddParameter("limit", "1");
        request.AddParameter("user", username);
        request.AddParameter("api_key", Utilities.APIKey);
        request.Timeout = 20000;

        RestResponse response = await _lfmClient.ExecuteAsync(request);

        if (response.Content != null)
        {
            return GetResponse<TrackResponse>(response.Content);
        }

        throw new HttpRequestException(Enum.GetName(response.StatusCode), null, response.StatusCode);
    }

    private async Task<string> GetSignature(string message)
    {
        RestRequest request = new RestRequest("", Method.Post);

        // this was a pain in the ass to write
        string json = @"{""string"": """ + message + @"""}";

        request.AddParameter("application/json", json, ParameterType.RequestBody);
        request.RequestFormat = DataFormat.Json;
        request.Timeout = 20000;

        RestResponse response = await _signatureClient.ExecuteAsync(request);
        if (response.Content != null)
        {
            return response.Content;
        }
        throw new HttpRequestException(Enum.GetName(response.StatusCode), null, response.StatusCode);
    }

    private static T GetResponse<T>(string response)
    {
        LastfmError e = JsonConvert.DeserializeObject<LastfmError>(response)!;
        if (e.Error == ErrorEnum.OK)
        {
            return JsonConvert.DeserializeObject<T>(response)!;
        }

        throw new LastfmException(e.Message, e.Error);
    }

    public void Dispose()
    {
        _lfmClient.Dispose();
        _signatureClient.Dispose();
        SuppressFinalize(this);
    }
}
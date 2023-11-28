using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using LastfmDiscordRPC2.Enums;
using LastfmDiscordRPC2.Exceptions;
using LastfmDiscordRPC2.Models.Responses;
using Newtonsoft.Json;
using RestSharp;

namespace LastfmDiscordRPC2.Models.API;

public class LastfmAPIClient : IAPIClient
{
    public RestClient APIRestClient { get; }
    private readonly ISignatureAPIClient _signatureAPIClient;

    public LastfmAPIClient(ISignatureAPIClient signatureAPIClient)
    {
        APIRestClient = new RestClient(@"https://ws.audioscrobbler.com/2.0/");
        APIRestClient.AddDefaultHeader("User-Agent", "LastfmDiscordRPC 2.0.0");
        
        _signatureAPIClient = signatureAPIClient;
    }

    public async Task<TokenResponse> GetToken()
    {
        RestRequest request = new RestRequest();

        request.AddParameter("format", "json");
        request.AddParameter("method", "auth.gettoken");
        request.AddParameter("api_key", Utilities.Utilities.LastfmAPIKey);
        request.Timeout = 20000;

        RestResponse response = await APIRestClient.ExecuteAsync(request);
        return GetResponse<TokenResponse>(response);
    }

    public async Task<SessionResponse> GetSession(string token)
    {
        string signature = await _signatureAPIClient.GetSignature($"api_key{Utilities.Utilities.LastfmAPIKey}methodauth.getsessiontoken{token}");

        RestRequest request = new RestRequest();
        request.AddParameter("format", "json");
        request.AddParameter("method", "auth.getsession");
        request.AddParameter("token", token);
        request.AddParameter("api_key", Utilities.Utilities.LastfmAPIKey);
        request.AddParameter("api_sig", signature);
        request.Timeout = 10000;

        PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromSeconds(5));
        
        do
        {
            try
            {
                while (await timer.WaitForNextTickAsync())
                {
                    RestResponse response = await APIRestClient.ExecuteAsync(request);
                    return GetResponse<SessionResponse>(response);
                }

                timer.Dispose();
            }
            catch (LastfmException e) when (e.ErrorCode is LastfmErrorCode.UnauthorizedToken) { }
        } while (true);
    }

    public async Task<TrackResponse> GetRecentTracks(string username)
    {
        RestRequest request = new RestRequest();

        request.AddParameter("format", "json");
        request.AddParameter("method", "user.getrecenttracks");
        request.AddParameter("limit", "1");
        request.AddParameter("user", username);
        request.AddParameter("api_key", Utilities.Utilities.LastfmAPIKey);
        request.Timeout = 20000;

        RestResponse response = await APIRestClient.ExecuteAsync(request);

        return GetResponse<TrackResponse>(response);
    }

    private static T GetResponse<T>(RestResponseBase response) where T : ILastfmAPIResponse
    {
        if (response.Content == null)
        {
            throw new HttpRequestException(Enum.GetName(response.StatusCode), null, response.StatusCode);
        }
        
        LastfmErrorResponse e = JsonConvert.DeserializeObject<LastfmErrorResponse>(response.Content)!;
        if (e.Error != LastfmErrorCode.OK)
        {   
            throw new LastfmException(e.Message, e.Error);
        }

        return JsonConvert.DeserializeObject<T>(response.Content)!;
    }
    
    public void Dispose()
    {
        APIRestClient.Dispose();
    }
}
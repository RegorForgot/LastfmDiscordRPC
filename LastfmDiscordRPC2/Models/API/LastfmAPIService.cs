using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using LastfmDiscordRPC2.DataTypes;
using LastfmDiscordRPC2.Exceptions;
using LastfmDiscordRPC2.IO;
using LastfmDiscordRPC2.Models.Responses;
using Newtonsoft.Json;
using RestSharp;
using static LastfmDiscordRPC2.Utilities.URIOpen;

namespace LastfmDiscordRPC2.Models.API;

public sealed class LastfmAPIService : IAPIService
{
    public DateTimeOffset LastScrobbleTime { get; set; }
    public RestClient APIRestClient { get; init; }
    public const string LastfmAPIKey = "79d35013754ac3b3225b73bba566afca";
    private readonly ISignatureAPIService _signatureAPIService;

    public LastfmAPIService(ISignatureAPIService signatureAPIService)
    {
        APIRestClient = new RestClient(@"https://ws.audioscrobbler.com/2.0/");
        APIRestClient.AddDefaultHeader("User-Agent", "LastfmDiscordRPC 2.0.0");

        _signatureAPIService = signatureAPIService;
    }

    public async Task<TokenResponse> GetToken()
    {
        RestRequest request = new RestRequest();

        request.AddParameter("format", "json");
        request.AddParameter("method", "auth.gettoken");
        request.AddParameter("api_key", LastfmAPIKey);
        request.Timeout = TimeSpan.FromSeconds(10);

        RestResponse response = await APIRestClient.ExecuteAsync(request).ConfigureAwait(false);
        return GetResponse<TokenResponse>(response);
    }

    public async Task<SessionResponse> GetSession(string token)
    {
        OpenURI($"https://www.last.fm/api/auth/?api_key={LastfmAPIKey}&token={token}");

        string signature = await _signatureAPIService.GetSignature($"api_key{LastfmAPIKey}methodauth.getsessiontoken{token}");

        RestRequest request = new RestRequest();
        request.AddParameter("format", "json");
        request.AddParameter("method", "auth.getsession");
        request.AddParameter("token", token);
        request.AddParameter("api_key", LastfmAPIKey);
        request.AddParameter("api_sig", signature);
        request.Timeout = TimeSpan.FromSeconds(10);

        using (PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromSeconds(5)))
        {
            CancellationTokenSource source = new CancellationTokenSource();
            do
            {
                try
                {
                    while (await timer.WaitForNextTickAsync(source.Token))
                    {
                        RestResponse response = await APIRestClient.ExecuteAsync(request).ConfigureAwait(false);
                        return GetResponse<SessionResponse>(response);
                    }

                    source.Cancel();
                }
                catch (LastfmException e) when (e.ErrorCode is LastfmErrorCode.UnauthorizedToken) { }
            } while (true);
        }
    }

    public async Task<LastfmErrorResponse> LoveOrUnloveTrack(bool loved, SaveCfg.Account account, TrackResponse trackResponse)
    {
        Track track = trackResponse.RecentTracks.Tracks[0];

        string loveMethod = loved ? "track.unlove" : "track.love";
        string signatureText = $"api_key{LastfmAPIKey}artist{track.Artist.Name}method{loveMethod}sk{account.SessionKey}track{track.Name}";
        string signature = await _signatureAPIService.GetSignature(signatureText);

        RestRequest request = new RestRequest();

        request.AddParameter("format", "json");
        request.AddParameter("method", loveMethod);
        request.AddParameter("api_key", LastfmAPIKey);
        request.AddParameter("sk", account.SessionKey);
        request.AddParameter("api_sig", signature);
        request.AddParameter("track", track.Name);
        request.AddParameter("artist", track.Artist.Name);
        request.Timeout = TimeSpan.FromSeconds(10);

        RestResponse response = await APIRestClient.PostAsync(request).ConfigureAwait(false);
        
        return GetResponse<LastfmErrorResponse>(response);
    }

    public async Task<TrackResponse> GetRecentTracks(string username)
    {
        RestRequest request = new RestRequest();

        request.AddParameter("format", "json");
        request.AddParameter("method", "user.getrecenttracks");
        request.AddParameter("limit", "1");
        request.AddParameter("user", username);
        request.AddParameter("api_key", LastfmAPIKey);
        request.AddParameter("extended", "1");
        request.Timeout = TimeSpan.FromSeconds(10);

        RestResponse response = await APIRestClient.ExecuteAsync(request).ConfigureAwait(false);

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
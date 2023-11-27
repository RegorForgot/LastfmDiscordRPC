using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using LastfmDiscordRPC2.Assets;
using LastfmDiscordRPC2.Models.Responses;
using Newtonsoft.Json;
using RestSharp;
using static LastfmDiscordRPC2.Models.LastfmException;

namespace LastfmDiscordRPC2.Models;

public static class LastfmClient
{
    private static readonly RestClient LfmClient;
    private static readonly RestClient SignatureClient;

    static LastfmClient()
    {
        LfmClient = new RestClient(@"https://ws.audioscrobbler.com/2.0/");
        LfmClient.AddDefaultHeader("User-Agent", "LastfmDiscordRPC 2.0.0");

        SignatureClient = new RestClient(@"https://crygup.com/regor/");
        SignatureClient.AddDefaultHeader("User-Agent", "LastfmDiscordRPC 2.0.0");
    }

    public async static Task<TokenResponse> GetToken()
    {
        RestRequest request = new RestRequest();

        request.AddParameter("format", "json");
        request.AddParameter("method", "auth.gettoken");
        request.AddParameter("api_key", Utilities.APIKey);
        request.Timeout = 20000;

        RestResponse response = await LfmClient.ExecuteAsync(request);
        return GetResponse<TokenResponse>(response);
    }

    public async static Task<SessionResponse?> GetSession(string token)
    {
        string signature = await GetSignatureTemp($"api_key{Utilities.APIKey}methodauth.getsessiontoken{token}");

        RestRequest request = new RestRequest();
        request.AddParameter("format", "json");
        request.AddParameter("method", "auth.getsession");
        request.AddParameter("token", token);
        request.AddParameter("api_key", Utilities.APIKey);
        request.AddParameter("api_sig", signature);
        request.Timeout = 10000;

        PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromSeconds(5));
        
        do
        {
            try
            {
                while (await timer.WaitForNextTickAsync())
                {
                    RestResponse response = await LfmClient.ExecuteAsync(request);
                    return GetResponse<SessionResponse>(response);
                }

                timer.Dispose();
            }
            catch (LastfmException e)
            {
                if (e.ErrorCode != ErrorEnum.UnauthorizedToken)
                {
                    throw;
                }
            }
        } while (true);
    }

    public async static Task<TrackResponse> CallAPI(string username)
    {
        RestRequest request = new RestRequest();

        request.AddParameter("format", "json");
        request.AddParameter("method", "user.getrecenttracks");
        request.AddParameter("limit", "1");
        request.AddParameter("user", username);
        request.AddParameter("api_key", Utilities.APIKey);
        request.Timeout = 20000;

        RestResponse response = await LfmClient.ExecuteAsync(request);

        return GetResponse<TrackResponse>(response);
    }

    private static Task<String> GetSignatureTemp(string message)
    {
        using MD5 hash = MD5.Create();
        string input = message + SecretKey.Secret;
        return Task.FromResult(Convert.ToHexString(hash.ComputeHash(System.Text.Encoding.ASCII.GetBytes(input))));
    }

    private async static Task<string> GetSignature(string message)
    {
        RestRequest request = new RestRequest("", Method.Post);

        // this was a pain in the ass to write
        string json = @"{""string"": """ + message + @"""}";

        request.AddParameter("application/json", json, ParameterType.RequestBody);
        request.RequestFormat = DataFormat.Json;
        request.Timeout = 20000;

        RestResponse response = await SignatureClient.ExecuteAsync(request);
        if (response.Content != null)
        {
            return response.Content;
        }

        throw new HttpRequestException(Enum.GetName(response.StatusCode), null, response.StatusCode);
    }

    private static T GetResponse<T>(RestResponse response)
    {
        if (response.Content == null)
        {
            throw new HttpRequestException(Enum.GetName(response.StatusCode), null, response.StatusCode);
        }
        
        LastfmError e = JsonConvert.DeserializeObject<LastfmError>(response.Content)!;
        if (e.Error != ErrorEnum.OK)
        {   
            throw new LastfmException(e.Message, e.Error);
        }

        return JsonConvert.DeserializeObject<T>(response.Content)!;
    }

    public static void Dispose()
    {
        LfmClient.Dispose();
        SignatureClient.Dispose();
    }
}
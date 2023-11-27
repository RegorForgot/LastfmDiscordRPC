using System;
using RestSharp;

namespace LastfmDiscordRPC2.Models.API;

public interface IAPIClient : IDisposable
{
    public RestClient APIRestClient { get; }
}
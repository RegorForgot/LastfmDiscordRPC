using System;
using RestSharp;

namespace LastfmDiscordRPC2.Models.API;

public interface IAPIService : IDisposable
{
    public RestClient APIRestClient { get; }
}
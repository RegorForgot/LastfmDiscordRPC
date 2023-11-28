namespace LastfmDiscordRPC2.Models.API;

public class LastfmService : LastfmAPIClient
{
    private readonly LastfmAPIClient _lastfmAPIClient;
    
    public LastfmService(LastfmAPIClient lastfmAPIClient, ISignatureAPIClient apiClient) : base(apiClient)
    {
        _lastfmAPIClient = lastfmAPIClient;
    }
    
    public long LastScrobbleTime { get; set; }
}
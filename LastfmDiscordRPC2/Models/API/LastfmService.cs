namespace LastfmDiscordRPC2.Models.API;

public class LastfmService : AbstractLastfmAPIClient
{
    public LastfmService(ISignatureAPIClient apiClient) : base(apiClient) { }

    public long LastScrobbleTime { get; set; }
}
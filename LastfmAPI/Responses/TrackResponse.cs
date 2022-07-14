using Newtonsoft.Json;
using static System.String;

namespace LastfmAPI.Responses;

public class TrackResponse : IResponse
{
    public Track? Track { get; private set; }
    
    private RecentTrackList _recentTracks = null!;
    
    [JsonProperty("recenttracks")] 
    public RecentTrackList RecentTracks 
    { 
        get => _recentTracks;
        set
        {
            _recentTracks = value;
            Track = _recentTracks.Tracks.Count == 0 ? null : _recentTracks.Tracks[0];
        }
    }
    
    public class RecentTrackList
    {
        [JsonProperty("track")] public List<Track> Tracks { get; set; } = null!;
    }
}

public class Track
{
    [JsonProperty("name")] public string Name { get; set; } = Empty;
    [JsonProperty("artist")] public TrackArtist Artist { get; set; } = null!;
    [JsonProperty("album")] public TrackAlbum Album { get; set; } = null!;
    [JsonProperty("@attr")] public TrackNowPlaying? NowPlaying { get; set; }
    [JsonProperty("date")] public PlayDate? Date { get; set; }

    public string ImageURL { get; private set; } = @"https://lastfm.freetls.fastly.net/i/u/174s/4128a6eb29f94943c9d206c08e625904.jpg";
    
    private List<AlbumImage> _images = null!;
    [JsonProperty("image")] public List<AlbumImage> Images 
    { 
        get => _images;
        set
        {
            _images = value;
            // Get large image
            if (_images[3].URL != "") ImageURL = _images[3].URL.Replace(@"\", "");
        } 
    }

    public class TrackArtist
    {
        [JsonProperty("#text")] public string Name { get; set; } = Empty;
    }

    public class TrackAlbum
    {
        [JsonProperty("#text")] public string Name { get; set; } = Empty;
    }

    public class AlbumImage
    {
        [JsonProperty("#text")] public string URL { get; set; } = Empty;
    }

    public class TrackNowPlaying
    {
        [JsonProperty("nowplaying")] public string State { get; set; } = Empty;
    }

    public class PlayDate
    {
        [JsonProperty("uts")] public string Timestamp { get; set; } = Empty;
    }
}
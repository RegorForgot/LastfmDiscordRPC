using Newtonsoft.Json;
using static System.String;

namespace LastfmAPI.Responses;

public class TrackResponse : IResponse
{
    public Track? Track { get; private set; }
    private RecentTrackList _recentTracks;
    
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
        [JsonProperty("track")] public List<Track> Tracks { get; set; }
    }
}

public class Track
{
    [JsonProperty("name")] public string Name { get; set; }
    [JsonProperty("artist")] public TrackArtist Artist { get; set; }
    [JsonProperty("album")] public TrackAlbum Album { get; set; }
    [JsonProperty("@attr")] public TrackNowPlaying? NowPlaying { get; set; }
    [JsonProperty("date")] public PlayDate? Date { get; set; }
    
    public string? ImageURL { get; private set; }
    
    private List<AlbumImage> _images;
    [JsonProperty("image")] public List<AlbumImage> Images 
    { 
        get => _images;
        set
        {
            _images = value;
            // Get large image
            ImageURL = _images[2].URL;
        } 
    }

    public class TrackArtist
    {
        [JsonProperty("#text")] public string Name { get; set; }
    }

    public class TrackAlbum
    {
        private string? _name;
        [JsonProperty("#text")]
        public string? Name
        {
            get => _name;
            set => _name = value != Empty ? value : null;
        }
    }

    public class AlbumImage
    {
        private string? _url;
        [JsonProperty("#text")]
        public string? URL
        {
            get => _url;
            set => _url = value != Empty ? value : null;
        }
    }

    public class TrackNowPlaying
    {
        [JsonProperty("nowplaying")] public string State { get; set; }
    }

    public class PlayDate
    {
        [JsonProperty("uts")] public string Timestamp { get; set; }
    }
}
using System.Collections.Generic;
using Newtonsoft.Json;
using static System.String;

// I feel safe doing this because all the "nulled" items are either filled by Newtonsoft or handled in LastfmClient with the 
// Lastfm and HTTPRequest exceptions
#pragma warning disable CS8618
#pragma warning disable CS8601

namespace LastfmDiscordRPC2.Models;

public class LastfmResponse
{
    private RecentTrackList _recentTracks;

    [JsonProperty("recenttracks")]
    public RecentTrackList RecentTracks
    {
        get => _recentTracks;
        set
        {
            _recentTracks = value;
            Track = _recentTracks.Tracks.Count == 0 ? null : _recentTracks.Tracks[0];
            Playcount = _recentTracks.Footer.Playcount;
        }
    }

    public Track? Track { get; private set; }
    public string Playcount { get; private set; }

    public class RecentTrackList
    {
        [JsonProperty("track")] public List<Track> Tracks { get; set; }
        [JsonProperty("@attr")] public ResponseFooter Footer { get; set; }
    }
    
    public class ResponseFooter
    {
        [JsonProperty("total")] public string Playcount { get; set; }
    }
}

public class Track
{
    public const string DefaultSingleCover 
        = @"https://lastfm.freetls.fastly.net/i/u/300x300/4128a6eb29f94943c9d206c08e625904.png";

    public const string DefaultAlbumCover
        = @"https://lastfm.freetls.fastly.net/i/u/300x300/2a96cbd8b46e442fc41c2b86b821562f.png";

    [JsonProperty("name")] public string Name { get; set; } = Empty;
    [JsonProperty("artist")] public TrackArtist Artist { get; set; } = new TrackArtist();
    [JsonProperty("album")] public TrackAlbum Album { get; set; } = new TrackAlbum();
    [JsonProperty("@attr")] public TrackNowPlaying NowPlaying { get; set; } = new TrackNowPlaying();
    [JsonProperty("date")] public PlayDate Date { get; set; } = new PlayDate();
    [JsonProperty("image")] public List<AlbumImage> Images { get; set; } = new List<AlbumImage>(4);

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
        private string _url = DefaultSingleCover;
        [JsonProperty("#text")]
        public string URL
        {
            get => _url;
            set
            {
                if (!IsNullOrEmpty(value))
                {
                    _url = value;
                }
            }
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
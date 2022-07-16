using System.Collections.Generic;
using Newtonsoft.Json;
using static System.String;

// I feel safe doing this because all the "nulled" items are either filled by Newtonsoft or handled in LastfmClient with the 
// Lastfm and HTTPRequest exceptions
#pragma warning disable CS8618
#pragma warning disable CS8601

namespace LastfmDiscordRPC.Models;

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
        }
    }

    [JsonProperty("@attr")] public ResponseFooter UserInfo { get; set; }

    public Track? Track { get; private set; }

    public class RecentTrackList
    {
        [JsonProperty("track")] public List<Track> Tracks { get; set; }
    }

    public class ResponseFooter
    {
        [JsonProperty("total")] public string Playcount { get; set; }
    }
}

public class Track
{
    public const string? DefaultCover = @"https://lastfm.freetls.fastly.net/i/u/300x300/4128a6eb29f94943c9d206c08e625904.jpg";

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
        [JsonProperty("#text")] public string URL { get; set; } =  DefaultCover;
    }

    public class TrackNowPlaying
    {
        [JsonProperty("nowplaying")] public string State { get; set; } = "false";
    }

    public class PlayDate
    {
        [JsonProperty("uts")] public string Timestamp { get; set; } = Empty;
    }
}
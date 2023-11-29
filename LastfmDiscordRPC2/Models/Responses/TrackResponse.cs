using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

// I feel safe doing this because all the "nulled" items are either filled by Newtonsoft or handled in LastfmClient with the 
// Lastfm and HTTPRequest exceptions
#pragma warning disable CS8618
#pragma warning disable CS8601

namespace LastfmDiscordRPC2.Models.Responses;

public record TrackResponse : ILastfmAPIResponse
{
    [JsonProperty("recenttracks")] public RecentTrackList RecentTracks { get; init; }

    public record RecentTrackList
    {
        [JsonProperty("track")] public List<Track> Tracks { get; init; }
        [JsonProperty("@attr")] public ResponseFooter Footer { get; init; }
    }

    public record ResponseFooter
    {
        [JsonProperty("total")] public string Playcount { get; init; }
    }
}

public record Track
{
    public virtual bool Equals(Track? other)
    {
        if (other is null)
        {
            return false;
        }

        return Name == other.Name && Artist == other.Artist && Album == other.Album && NowPlaying == other.NowPlaying && Date == other.Date && Images.SequenceEqual(other.Images);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Artist, Album, NowPlaying, Date, Images);
    }


    private const string DefaultSingleCover
        = @"https://lastfm.freetls.fastly.net/i/u/300x300/4128a6eb29f94943c9d206c08e625904.png";

    public const string DefaultAlbumCover
        = @"https://lastfm.freetls.fastly.net/i/u/300x300/2a96cbd8b46e442fc41c2b86b821562f.png";

    [JsonProperty("name")] public string Name { get; init; } = Empty;
    [JsonProperty("artist")] public TrackArtist Artist { get; init; } = new TrackArtist();
    [JsonProperty("album")] public TrackAlbum Album { get; init; } = new TrackAlbum();
    [JsonProperty("@attr")] public TrackNowPlaying NowPlaying { get; init; } = new TrackNowPlaying();
    [JsonProperty("date")] public PlayDate Date { get; init; } = new PlayDate();
    [JsonProperty("image")] public List<AlbumImage> Images { get; init; } = new List<AlbumImage>(4);

    public record TrackArtist
    {
        [JsonProperty("#text")] public string Name { get; init; } = Empty;
    }

    public record TrackAlbum
    {
        [JsonProperty("#text")] public string Name { get; init; } = Empty;
    }

    public record AlbumImage
    {
        private string _url = DefaultSingleCover;
        [JsonProperty("#text")]
        public string URL
        {
            get => _url;
            init
            {
                if (!IsNullOrEmpty(value))
                {
                    _url = value;
                }
            }
        }
    }

    public record TrackNowPlaying
    {
        [JsonProperty("nowplaying")] public string State { get; init; } = Empty;
    }

    public record PlayDate
    {
        [JsonProperty("uts")] public string Timestamp { get; init; } = Empty;
    }
}
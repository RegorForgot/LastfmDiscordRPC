using System;
using static LastfmDiscordRPC2.DataTypes.PresenceParseString;

namespace LastfmDiscordRPC2.DataTypes;

public struct SaveVars
{
    internal const string DefaultAppID = "997756398664421446";
    internal const string DefaultDetails = $"🎵 {TrackName}";
    internal const string DefaultState = $"{ArtistName} | 💿 {AlbumName}";
    internal const string DefaultLargeImageLabel = AlbumName;
    internal const string DefaultSmallImageLabel = $"{Timestamp}{CurrentState}";
    internal const string DefaultButtonLabel = $"{PlayCount} scrobbles";
    internal const string DefaultButtonURL = $"https://www.last.fm/music/{ArtistName}/_/{TrackName}";
    internal const bool DefaultExpiryMode = false;
    internal static readonly TimeSpan DefaultExpiryTime = new TimeSpan(1, 0, 0);
}
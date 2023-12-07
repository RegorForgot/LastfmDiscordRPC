using System;
using System.Collections.Generic;

namespace LastfmDiscordRPC2.DataTypes;

public static class PresenceParseString
{
    internal const string PlayCount = "{PlayCount}";
    internal const string TrackName = "{TrackName}";
    internal const string ArtistName = "{ArtistName}";
    internal const string AlbumName = "{AlbumName}";
    internal const string CurrentState = "{Playing}";
    internal const string Timestamp = "{TimeSincePlayed}";

    public static IEnumerable<Tuple<string, string>> StringList =>
        new List<Tuple<string, string>>
        {
            new Tuple<string, string>(TrackName, "Name of current track"),
            new Tuple<string, string>(ArtistName, "Name of current artist"),
            new Tuple<string, string>(AlbumName, "Name of current album (blank if none)"),
            new Tuple<string, string>(PlayCount, "User's scrobble count"),
            new Tuple<string, string>(CurrentState, "Whether track is currently playing (blank if stopped)"),
            new Tuple<string, string>(Timestamp, "Time since last play (blank if playing)")
        };
}
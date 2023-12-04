using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using LastfmDiscordRPC2.Models.Responses;
using LastfmDiscordRPC2.Models.RPC;

namespace LastfmDiscordRPC2.Utilities;

public static class DiscordClientExtensions
{
    private const string ParserRegex = "{[^{}]+}";

    public static string GetParsedString(this DiscordClient client, TrackResponse response, string? stringToParse)
    {
        string parsedString = new string(stringToParse);
        
        IEnumerable<string> parsingItems = GetItemsToParse(stringToParse ?? Empty);
        
        parsedString = 
            parsingItems.Aggregate(parsedString, (current, parsingItem) => current.Replace(parsingItem, GetParsedItem(response, parsingItem)));
        return GetUTF8String(parsedString);
    }

    private static IEnumerable<string> GetItemsToParse(string stringToParse) =>
        Regex.Matches(stringToParse, ParserRegex).Select(match => match.Value).ToList();

    private static string GetParsedItem(TrackResponse response, string itemToBeParsed)
    {
        Track firstTrack = response.RecentTracks.Tracks[0];
        switch (itemToBeParsed)
        {
            case ParsingStrings.PlayCount:
                return response.RecentTracks.Footer.PlayCount;
            case ParsingStrings.TrackName:
                return firstTrack.Name;
            case ParsingStrings.ArtistName:
                return firstTrack.Artist.Name;
            case ParsingStrings.AlbumName:
                return firstTrack.Album.Name;
            case ParsingStrings.CurrentState:
                return firstTrack.NowPlaying.State == Empty ? Empty : "Now playing";
            case ParsingStrings.Timestamp:
                bool success =
                    long.TryParse(firstTrack.Date.Timestamp, NumberStyles.Number, null, out long unixLastScrobbleTime);
                if (!success)
                {
                    return Empty;
                }
                TimeSpan timeSince = TimeSpan.FromSeconds(DateTimeOffset.Now.ToUnixTimeSeconds() - unixLastScrobbleTime);
                return GetTimeString(timeSince);
            default:
                return itemToBeParsed;
        }
    }

    private static string GetUTF8String(string input)
    {
        if (IsNullOrEmpty(input))
        {
            return input;
        }
        
        if (input.Length < 2)
        {
            input += "\u180E";
        }

        if (Encoding.UTF8.GetByteCount(input) <= 128)
        {
            return input;
        }

        byte[] buffer = new byte[128];
        char[] inputChars = input.ToCharArray();
        Encoding.UTF8.GetEncoder().Convert(inputChars, 0, inputChars.Length, buffer, 0, buffer.Length,
            false, out _, out int bytesUsed, out _);

        return Encoding.UTF8.GetString(buffer, 0, bytesUsed);
    }

    private static string GetTimeString(TimeSpan timeSince)
    {
        string days = timeSince.Days == 0 ? "" : $"{timeSince.Days}d ";
        string hours = timeSince.Hours == 0 ? "" : $"{timeSince.Hours % 24}h ";
        string minutes = timeSince.Minutes == 0 ? "" : $"{timeSince.Minutes % 60}m ";
        string seconds = timeSince.Seconds == 0 ? "" : $"{timeSince.Seconds % 60}s";

        return $"Last played {days}{hours}{minutes}{seconds} ago";
    }

    public struct ParsingStrings
    {
        internal const string PlayCount = "{PlayCount}";
        internal const string TrackName = "{TrackName}";
        internal const string ArtistName = "{ArtistName}";
        internal const string AlbumName = "{AlbumName}";
        internal const string CurrentState = "{Playing}";
        internal const string Timestamp = "{TimeSincePlayed}";
    }
}
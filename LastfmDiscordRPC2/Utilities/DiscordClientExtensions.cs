using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using LastfmDiscordRPC2.DataTypes;
using LastfmDiscordRPC2.Models.Responses;
using LastfmDiscordRPC2.Models.RPC;
using static LastfmDiscordRPC2.DataTypes.ParsingStringStruct;

namespace LastfmDiscordRPC2.Utilities;

public static class DiscordClientExtensions
{
    private const string ParserRegex = "{[^{}]+}";
    
    public static bool ValidatePlaceholderLink(string linkToParse)
    {
        string parsedString = new string(linkToParse);
        
        IEnumerable<string> parsingItems = GetItemsToParse(linkToParse ?? Empty);
        parsedString = 
            parsingItems.Aggregate(parsedString, (current, item) => current.Replace(item, "test"));

        bool success = Uri.TryCreate(parsedString, UriKind.Absolute, out Uri? result);
        return success && (result?.Scheme == Uri.UriSchemeHttps || result?.Scheme == Uri.UriSchemeHttp);
    }
    
    public static string GetParsedLink(this DiscordClient client, TrackResponse response, string linkToParse, BytesEnum bytesToClip)
    {
        string parsedString = new string(linkToParse);
        
        IEnumerable<string> parsingItems = GetItemsToParse(linkToParse ?? Empty);

        parsedString = 
            parsingItems.Aggregate(parsedString, (current, item) => current.Replace(item, HttpUtility.UrlEncode(GetParsedItem(response, item))));
        return GetUTF8String(parsedString, bytesToClip);
    }
    
    public static string GetParsedString(this DiscordClient client, TrackResponse response, string stringToParse, BytesEnum bytesToClip)
    {
        string parsedString = new string(stringToParse);
        
        IEnumerable<string> parsingItems = GetItemsToParse(stringToParse ?? Empty);
        
        parsedString = 
            parsingItems.Aggregate(parsedString, (current, parsingItem) => current.Replace(parsingItem, GetParsedItem(response, parsingItem)));
        return GetUTF8String(parsedString, bytesToClip);
    }

    private static IEnumerable<string> GetItemsToParse(string stringToParse) =>
        Regex.Matches(stringToParse, ParserRegex).Select(match => match.Value).ToList();

    private static string GetParsedItem(TrackResponse response, string itemToBeParsed)
    {
        Track firstTrack = response.RecentTracks.Tracks[0];
        switch (itemToBeParsed)
        {
            case PlayCount:
                return long.Parse(response.RecentTracks.Footer.PlayCount).ToString("N0");
            case TrackName:
                return firstTrack.Name;
            case ArtistName:
                return firstTrack.Artist.Name;
            case AlbumName:
                return firstTrack.Album.Name;
            case CurrentState:
                return firstTrack.NowPlaying.State == Empty ? Empty : "Now playing";
            case Timestamp:
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

    private static string GetUTF8String(string input, BytesEnum bytesToClip)
    {
        if (IsNullOrEmpty(input))
        {
            return input;
        }
        
        if (input.Length < 2)
        {
            input += "\u180E";
        }

        if (Encoding.UTF8.GetByteCount(input) <= (int)bytesToClip)
        {
            return input;
        }

        byte[] buffer = new byte[(int)bytesToClip];
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
}
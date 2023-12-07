using System;
using System.Collections.ObjectModel;
using LastfmDiscordRPC2.IO;
using LastfmDiscordRPC2.Models;
using LastfmDiscordRPC2.ViewModels.Controls;
using static LastfmDiscordRPC2.DataTypes.PresenceParseString;

namespace LastfmDiscordRPC2.DataTypes;

public struct SaveVars
{
    internal const string DefaultAppID = "997756398664421446";
    internal const string DefaultDetails = $"🎵 {TrackName}";
    internal const string DefaultState = $"{ArtistName} | 💿 {AlbumName}";
    internal const string DefaultLargeImageLabel = AlbumName;
    internal const string DefaultSmallImageLabel = $"{Timestamp}{CurrentState}";
    internal const string DefaultButtonLabel = $"{PlayCount} plays";
    internal const string DefaultButtonURL = $"https://www.last.fm/music/{ArtistName}/_/{TrackName}";
    internal static readonly TimeSpan DefaultExpiryTime = new TimeSpan(1, 0, 0);
    internal static readonly RPCButton[] DefaultUserButtons = { new RPCButton() };
}
using System;
using System.Globalization;
using DiscordRPC;
using static System.String;

namespace LastfmDiscordRPC.Models;

public class DiscordClient : IDisposable
{
    private DiscordRpcClient _client;
    
    private const string _playIconURL =  @"https://i.imgur.com/3XSJDvi.png";
    private const string _pauseIconURL = @"https://i.imgur.com/HnwfvVs.png";
    
    public DiscordClient(string appId)
    {
        _client = new DiscordRpcClient(appId);
        _client.Initialize();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_client.IsDisposed) return;
            
        try
        {
            _client.ClearPresence();
            _client.Deinitialize();
            _client.Dispose();
        } catch (ObjectDisposedException)
        { }
    }

    public void SetPresence(LastfmResponse response)
    {
        Track track = response.Track!;
        string smallImage;
        string smallText;
        string albumString = IsNullOrEmpty(track.Album.Name) ? "" : $" | On {track.Album.Name}";
        
        if (response.Track!.NowPlaying.State == "true")
        {
            smallImage = _playIconURL;
            smallText = "Now playing";
        } else
        {
            smallImage = _pauseIconURL;

            if (long.TryParse(response.Track.Date.Timestamp, NumberStyles.Number, null, out long unixTimeStamp))
            {
                TimeSpan timeSince = TimeSpan.FromSeconds(DateTimeOffset.Now.ToUnixTimeSeconds() - unixTimeStamp);
                smallText = $"Last played {GetTimeString(timeSince)}";
            } else
            {
                smallText = "Stopped.";
            }
        }
            
        RichPresence presence = new RichPresence()
        {
            Details = track.Name,
            State = $"By {track.Artist.Name}{albumString}",
            Assets = new Assets
            {
                LargeImageKey = track.Images[3].URL,
                LargeImageText = $"{response.Playcount} scrobbles",
                SmallImageKey = smallImage,
                SmallImageText = smallText
            }
        };
        _client.SetPresence(presence);

        string GetTimeString(TimeSpan timeSince)
        {
            string days = timeSince.Days switch
            {
                0 => "",
                _ => $"{timeSince.Days}d "
            };
            string hours = timeSince.Hours switch
            {
                0 => "",
                _ => $"{timeSince.Hours % 24}h "
            };
            string minutes = timeSince.Minutes switch
            {
                0 => "",
                _ => $"{timeSince.Minutes % 60}m "
            };
            string seconds = timeSince.Seconds switch
            {
                0 => "",
                _ => $"{timeSince.Seconds % 60}s"
            };

            return $"{days}{hours}{minutes}{seconds}";
        }
    }

    public void ClearPresence()
    {
        if (_client.CurrentPresence != null) _client.ClearPresence();  
    }
}
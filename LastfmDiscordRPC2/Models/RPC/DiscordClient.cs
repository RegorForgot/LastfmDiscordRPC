using System;
using System.Globalization;
using System.Linq;
using System.Text;
using DiscordRPC;
using DiscordRPC.Exceptions;
using LastfmDiscordRPC2.Exceptions;
using LastfmDiscordRPC2.IO;
using LastfmDiscordRPC2.Logging;
using LastfmDiscordRPC2.Models.API;
using LastfmDiscordRPC2.Models.Responses;

namespace LastfmDiscordRPC2.Models.RPC;
 
public sealed class DiscordClient : IDisposable, IDiscordClient
{
    private DiscordRpcClient? _client;
    private readonly SaveCfgIOService _saveCfgService;
    private readonly LoggingService _loggingService;
    private readonly LastfmAPIService _lastfmService;

    private const string PauseIconURL = "https://i.imgur.com/AOYINL0.png";
    private const string PlayIconURL = "https://i.imgur.com/wvTxH0t.png";

    public DiscordClient(
        SaveCfgIOService saveCfgService, 
        LastfmAPIService lastfmService, 
        LoggingService loggingService)
    {
        _saveCfgService = saveCfgService;
        _lastfmService = lastfmService;
        _loggingService = loggingService;
    }

    public bool IsReady { get; private set; }

    public void Initialize()
    {
        if (_client is not null && _client.IsInitialized)
        {
            return;
        }

        IsReady = false;

        _client = new DiscordRpcClient(_saveCfgService.SaveCfg.UserRPCCfg.AppID);
        _client.Logger = _loggingService;
        _client.Initialize();


        SetEventHandlers();
    }

    private void SetEventHandlers()
    {
        if (_client == null)
        {
            return;
        }

        _client.OnClose += (_, _) =>
        {
            _loggingService.Error("Could not connect to Discord.");
            IsReady = false;
        };
        _client.OnError += (_, _) =>
        {
            _loggingService.Error("There has been an error trying to connect to Discord.");
            IsReady = false;
        };
        _client.OnConnectionFailed += (_, _) =>
        {
            _loggingService.Error("Connection to discord failed. Check if your Discord app is open.");
            IsReady = false;
        };
        _client.OnConnectionEstablished += (_, _) =>
        {
            _loggingService.Info("Connection to discord succeeded.");
        };
        _client.OnReady += (_, _) =>
        {
            _loggingService.Info("Client ready.");
            IsReady = true;
        };
    }
    
    public void SetPresence(TrackResponse response)
    {
        Track track = response.RecentTracks.Tracks[0];
        string smallImage;
        string smallText;
        string albumName = IsNullOrEmpty(track.Album.Name) ? "" : $" | On {track.Album.Name}";
        string image = IsNullOrEmpty(track.Album.Name) ? Track.DefaultSingleCover : track.Images[3].URL;

        if (track.NowPlaying.State == "true")
        {
            smallImage = PlayIconURL;
            smallText = "Now playing";
            _lastfmService.LastScrobbleTime = DateTimeOffset.Now.ToUnixTimeSeconds();
        }
        else
        {
            smallImage = PauseIconURL;

            if (long.TryParse(track.Date.Timestamp, NumberStyles.Number, null, out long lastScrobbleTime))
            {
                _lastfmService.LastScrobbleTime = lastScrobbleTime;
                TimeSpan timeSince = TimeSpan.FromSeconds(DateTimeOffset.Now.ToUnixTimeSeconds() - _lastfmService.LastScrobbleTime);
                smallText = $"Last played {GetTimeString(timeSince)}";
            }
            else
            {
                smallText = "Stopped.";
            }
        }

        Button[] buttons = _saveCfgService.SaveCfg.UserRPCCfg.UserButtons
            .Select(
                button => new Button
                {
                    Label = button.Label,
                    Url = button.Link
                }
            )
            .ToArray();
        
        RichPresence presence = new RichPresence
        {
            Details = GetUTF8String(track.Name),
            State = GetUTF8String($"By {track.Artist.Name}{albumName}"),
            Assets = new DiscordRPC.Assets
            {
                LargeImageKey = image,
                LargeImageText = $"{(IsNullOrEmpty(track.Album.Name) ? null : track.Album.Name)}",
                SmallImageKey = smallImage,
                SmallImageText = smallText
            },
            Buttons = buttons
        };

        _client?.SetPresence(presence);
        
        return;

        string GetTimeString(TimeSpan timeSince)
        {
            string days = timeSince.Days == 0 ? "" : $"{timeSince.Days}d ";
            string hours = timeSince.Hours == 0 ? "" : $"{timeSince.Hours % 24}h ";
            string minutes = timeSince.Minutes == 0 ? "" : $"{timeSince.Minutes % 60}m ";
            string seconds = timeSince.Seconds == 0 ? "" : $"{timeSince.Seconds % 60}s";

            return $"{days}{hours}{minutes}{seconds}";
        }

        string GetUTF8String(string input)
        {
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
    }

    public void ClearPresence()
    {
        if (_client is null)
        {
            return;
        }
        
        try
        {
            _client.ClearPresence();
        }
        catch (Exception ex) when (ex is ObjectDisposedException or UninitializedException) { }
    }
    
    public void Dispose()
    {
        if (_client is null)
        {
            return;
        }

        try
        {
            _client.ClearPresence();
            _client.Deinitialize();
            _client.Dispose();
            IsReady = false;
        }
        catch (Exception ex) when (ex is ObjectDisposedException or UninitializedException) { }
    }
}
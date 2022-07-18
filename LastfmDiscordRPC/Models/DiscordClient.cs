using System;
using System.Globalization;
using DiscordRPC;
using DiscordRPC.Exceptions;
using LastfmDiscordRPC.ViewModels;

namespace LastfmDiscordRPC.Models;

public class DiscordClient : IDisposable
{
    private DiscordRpcClient? _client;
    private RichPresence? _presence;
    private readonly MainViewModel _mainViewModel;
    private const string PauseIconURL = @"https://i.imgur.com/AOYINL0.png";
    private const string PlayIconURL = @"https://i.imgur.com/wvTxH0t.png";

    private bool IsInitialised => _client != null;

    public bool IsReady { get; private set; }

    public DiscordClient(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
    }

    public void Initialize()
    {
        if (IsInitialised) return;
        _client = new DiscordRpcClient(_mainViewModel.AppID)
        {
            Logger = _mainViewModel.Logger,
            SkipIdenticalPresence = true
        };
        _client.Initialize();
        _client.OnClose += (_, _) =>
        {
            _mainViewModel.Logger.ErrorOverride("Could not connect to Discord.");
        };
        _client.OnReady += (_, _) =>
        {
            _mainViewModel.Logger.InfoOverride("Client ready.");
            IsReady = true;
        };
    }

    /// <summary>
    /// Sets the discord presence of the user to the response that was received from Last.fm
    /// </summary>
    /// <param name="response">The API response received from last.fm</param>
    /// <param name="username">The username provided by user</param>
    public void SetPresence(LastfmResponse response, string username)
    {
        // Null ignore - handling done in SetPresenceCommand's Execute method.
        Track track = response.Track!;
        string smallImage;
        string smallText;
        string albumString = IsNullOrEmpty(track.Album.Name) ? "" : $" | On {track.Album.Name}";
        string trackName = track.Name;

        while (trackName.Length < 2)
        {
            trackName += "\u180E";
        }

        if (response.Track!.NowPlaying.State == "true")
        {
            smallImage = PlayIconURL;
            smallText = "Now playing";
        } else
        {
            smallImage = PauseIconURL;

            if (long.TryParse(response.Track.Date.Timestamp, NumberStyles.Number, null, out long unixTimeStamp))
            {
                TimeSpan timeSince = TimeSpan.FromSeconds(DateTimeOffset.Now.ToUnixTimeSeconds() - unixTimeStamp);
                smallText = $"Last played {GetTimeString(timeSince)}";
            } else
            {
                smallText = "Stopped.";
            }
        }
        string formatted_scrobbles = String.Format("{0:n0}", int.Parse(response.Playcount));
        Button button = new Button
        {
            Label = $"{formatted_scrobbles} scrobbles", Url = @$"https://www.last.fm/user/{username}/"
        };

        _presence = new RichPresence
        {
            Details = trackName,
            State = $"By {track.Artist.Name}{albumString}",
            Assets = new Assets
            {
                LargeImageKey = track.Images[3].URL,
                LargeImageText = $"{response.Playcount} scrobbles",
                SmallImageKey = smallImage,
                SmallImageText = smallText
            },
            Buttons = new[]
            {
                button
            }
        };
        _client?.SetPresence(_presence);
        _presence = null;

        string GetTimeString(TimeSpan timeSince)
        {
            string days = timeSince.Days == 0 ? "" : $"{timeSince.Days}d ";
            string hours = timeSince.Hours == 0 ? "" : $"{timeSince.Hours % 24}h ";
            string minutes = timeSince.Minutes == 0 ? "" : $"{timeSince.Minutes % 60}m ";
            string seconds = timeSince.Seconds == 0 ? "" : $"{timeSince.Seconds % 60}s";

            return $"{days}{hours}{minutes}{seconds}";
        }
    }

    public void ClearPresence()
    {
        _client?.ClearPresence();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (!IsInitialised) return;
        if (_client!.IsDisposed) return;

        try
        {
            ClearPresence();
            _client.Deinitialize();
            _client.Dispose();
        } catch (ObjectDisposedException)
        { } catch (UninitializedException)
        { } finally
        {
            SuppressFinalize(this);
        }
    }
}
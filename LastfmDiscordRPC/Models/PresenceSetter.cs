using System;
using System.Net.Http;
using System.Threading;
using LastfmDiscordRPC.ViewModels;
using static LastfmDiscordRPC.Models.LastfmException;

namespace LastfmDiscordRPC.Models;

public class PresenceSetter : IDisposable
{
    private static readonly SemaphoreSlim PresenceLock = new SemaphoreSlim(1, 1);
    private readonly MainViewModel _mainViewModel;
    private readonly LastfmClient _lastfmClient;
    private PeriodicTimer? _timer;
    private long _timeOfStart;
    private long _timeSinceStart;
    private long _timeSinceLastScrobble;
    private bool _firstSuccess;
    private bool _turnOffPresence;
    private int _exceptionCount;

    public PresenceSetter(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
        _lastfmClient = mainViewModel.LastfmClient;
        _turnOffPresence = false;
    }

    public async void UpdatePresence(string username, string apiKey)
    {
        _timeOfStart = DateTimeOffset.Now.ToUnixTimeSeconds();
        _turnOffPresence = false;
        await PresenceLock.WaitAsync();

        try
        {
            _mainViewModel.DiscordClient.Initialize();
            _mainViewModel.HasNotRun = false;
            _firstSuccess = false;

            using (_timer = new PeriodicTimer(TimeSpan.FromSeconds(2)))
            {
                while (await _timer.WaitForNextTickAsync() & !_turnOffPresence)
                {
                    try
                    {
                        LastfmResponse response = await _lastfmClient.CallAPI(username, apiKey);
                        TrySetPresence(username, response);
                    } catch (Exception e)
                    {
                        HandleError(e, username, apiKey);
                    }

                    // Turn off rich presence updating and close presence when time since last scrobble
                    // AND time since presence starting is longer than an hour.
                    long currentTime = DateTimeOffset.Now.ToUnixTimeSeconds();
                    _timeSinceStart = currentTime - _timeOfStart;
                    _timeSinceLastScrobble = currentTime - _mainViewModel.DiscordClient.LastScrobbleTime;
                    _turnOffPresence = _timeSinceStart > 3600 & _timeSinceLastScrobble > 3600;
                }
            }

            if (_turnOffPresence)
            {
                _mainViewModel.Logger.InfoOverride("Turned off presence due to inactivity");
                Dispose();
            }
            PresenceLock.Release();
        } catch
        {
            PresenceLock.Release();
            // All exceptions here will be thrown by the timer being disposed
            // all which are not will be handled in the inner catch block (which is logged).
        } 
    }

    private void TrySetPresence(string username, LastfmResponse? response)
    {
        if (response?.Track == null)
        {
            _mainViewModel.Logger.InfoOverride("No tracks found for user.");
            Dispose();
        } else
        {
            Track track = response.Track;
            string albumString = IsNullOrEmpty(track.Album.Name) ? "" : $" | On {track.Album.Name}";
            _mainViewModel.PreviewViewModel.Description = track.Name;
            _mainViewModel.PreviewViewModel.State = $"By {track.Artist.Name}{albumString}";
            _mainViewModel.PreviewViewModel.Tooltip = $"{response.Playcount} scrobbles";
            _mainViewModel.PreviewViewModel.ImageURL = track.Images[3].URL;

            if (!_firstSuccess)
            {
                _mainViewModel.Logger.InfoOverride("Track successfully received! Attempting to connect to presence...");
            }

            if (_mainViewModel.DiscordClient.IsReady)
            {
                _exceptionCount = 0;
                _mainViewModel.DiscordClient.SetPresence(response, username);

                if (!_firstSuccess)
                {
                    _mainViewModel.Logger.InfoOverride("Presence has been set!");
                }

                _firstSuccess = true;
            } else
            {
                _mainViewModel.Logger.WarningOverride("Discord client not initialised. Please restart and use a valid ID.");
                Dispose();
            }
        }
    }

    private void HandleError(Exception e, string username, string apiKey)
    {
        if (e.GetType() == typeof(LastfmException))
        {
            _mainViewModel.Logger.ErrorOverride("Last.fm {0}", e.Message);

            if (((LastfmException)e).ErrorCode is ErrorEnum.Temporary or ErrorEnum.OperationFail && _exceptionCount < 3)
            {
                _exceptionCount++;
                Dispose();
                UpdatePresence(username, apiKey);
                _mainViewModel.Logger.InfoOverride($"Attempting to reconnect... Try {_exceptionCount}");
            } else
            {
                Dispose();
            }
        } else if (e.GetType() == typeof(HttpRequestException))
        {
            _mainViewModel.Logger.ErrorOverride("HTTP {0}: {1}", ((HttpRequestException)e).StatusCode, e.Message);

            if (_exceptionCount < 3)
            {
                _exceptionCount++;
                Dispose();
                UpdatePresence(username, apiKey);
                _mainViewModel.Logger.InfoOverride($"Attempting to reconnect... Try {_exceptionCount}");
            } else
            {
                Dispose();
            }
        } else
        {
            _mainViewModel.Logger.ErrorOverride("Unhandled exception! Please report to developers {0}", e.Message);
            Dispose();
        }
    }

    public void Dispose()
    {
        _timer?.Dispose();
        _mainViewModel.DiscordClient.ClearPresence();
    }
}
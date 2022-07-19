using System;
using System.Net.Http;
using System.Threading;
using LastfmDiscordRPC.Commands;
using LastfmDiscordRPC.ViewModels;
using static LastfmDiscordRPC.Models.LastfmException;

namespace LastfmDiscordRPC.Models;

public class PresenceSetter : IDisposable
{
    private readonly static SemaphoreSlim PresenceLock = new SemaphoreSlim(1, 1);
    private readonly MainViewModel _mainViewModel;
    private readonly LastfmClient _lastfmClient;
    private LastfmResponse? _response;
    private PeriodicTimer? _timer;
    private bool _firstSuccess;
    private bool _retryAllowed;
    private int _exceptionCount;

    public bool RetryAllowed
    {
        get => _retryAllowed;
        set
        {
            _retryAllowed = value;
            ((SetPresenceCommand)_mainViewModel.SetPresenceCommand).RaiseCanExecuteChanged();
        }
    }

    public bool IsReady { get; set; } = true;

    public PresenceSetter(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
        _lastfmClient = mainViewModel.LastfmClient;
    }

    public async void UpdatePresence(string username, string apiKey)
    {
        _mainViewModel.DiscordClient.Initialize();
        _mainViewModel.HasNotRun = false;
        _firstSuccess = false;
        await PresenceLock.WaitAsync();

        try
        {
            using (_timer = new PeriodicTimer(TimeSpan.FromSeconds(2)))
            {
                try
                {
                    while (await _timer.WaitForNextTickAsync())
                    {
                        try
                        {
                            _response = await _lastfmClient.CallAPI(username, apiKey);
                            TrySetPresence(username);
                            _response = null;
                        } catch (Exception e)
                        {
                            ConnectionError(e, username, apiKey);
                        }
                    }
                } catch (Exception)
                {
                    // ignored
                }
            }
        } finally
        {
            PresenceLock.Release();
        }
    }

    private void TrySetPresence(string username)
    {
        if (_response?.Track == null)
        {
            _mainViewModel.Logger.InfoOverride("No tracks found for user.");
            Dispose();
        } else
        {
            Track track = _response.Track;
            string albumString = IsNullOrEmpty(track.Album.Name) ? "" : $" | On {track.Album.Name}";
            _mainViewModel.PreviewViewModel.Description = track.Name;
            _mainViewModel.PreviewViewModel.State = $"By {track.Artist.Name}{albumString}";
            _mainViewModel.PreviewViewModel.Tooltip = $"{_response.Playcount} scrobbles";
            _mainViewModel.PreviewViewModel.ImageURL = track.Images[3].URL;

            if (!_firstSuccess)
                _mainViewModel.Logger.InfoOverride("Track successfully received! Attempting to connect to presence...");

            if (_mainViewModel.DiscordClient.IsReady)
            {
                _exceptionCount = 0;
                _mainViewModel.DiscordClient.SetPresence(_response, username);
                if (!_firstSuccess)
                    _mainViewModel.Logger.InfoOverride("Presence has been set!");
                _firstSuccess = true;
            } else
            {
                _mainViewModel.Logger.WarningOverride("Discord client not initialised. Please restart and use a valid ID.");
                Dispose();
            }
        }
    }

    private void ConnectionError(Exception e, string username, string apiKey)
    {
        if (e.GetType() == typeof(LastfmException))
        {
            _mainViewModel.Logger.ErrorOverride("Last.fm {0}", e.Message);

            switch (((LastfmException)e).ErrorCode)
            {
                case ErrorEnum.RateLimit:
                    Dispose();
                    return;
                case ErrorEnum.Temporary or ErrorEnum.OperationFail when _exceptionCount < 3:
                    _exceptionCount++;
                    UpdatePresence(username, apiKey);
                    _mainViewModel.Logger.InfoOverride($"Attempting to reconnect... Try {_exceptionCount}");

                    break;
                case ErrorEnum.Temporary or ErrorEnum.OperationFail:
                    Dispose();
                    RetryAllowed = true;

                    break;
                default:
                    Dispose();
                    RetryAllowed = true;

                    break;
            }
        }
        else if (e.GetType() == typeof(HttpRequestException))
        {
            _mainViewModel.Logger.ErrorOverride("HTTP {0}: {1}", ((HttpRequestException)e).StatusCode, e.Message);
            Dispose();
            RetryAllowed = true;
        } else
        {
            _mainViewModel.Logger.ErrorOverride("Unhandled exception! {0}", e.Message);
            Dispose();
            RetryAllowed = true;
        }
    }

    public void Dispose()
    {
        _timer?.Dispose();
        _mainViewModel.DiscordClient.ClearPresence();
    }
}
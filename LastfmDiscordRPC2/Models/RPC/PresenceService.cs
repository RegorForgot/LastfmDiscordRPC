using System;
using System.Net.Http;
using System.Threading;
using LastfmDiscordRPC2.Enums;
using LastfmDiscordRPC2.Exceptions;
using LastfmDiscordRPC2.IO;
using LastfmDiscordRPC2.Logging;
using LastfmDiscordRPC2.Models.API;
using LastfmDiscordRPC2.Models.Responses;

namespace LastfmDiscordRPC2.Models.RPC;

public class PresenceService : IPresenceService
{
    private readonly LoggingService _loggingService;
    private readonly LastfmAPIService _lastfmService;
    private readonly IDiscordClient _discordClient;
    private readonly SaveCfgIOService _saveCfgService;

    private static readonly SemaphoreSlim PresenceLock = new SemaphoreSlim(1, 1);
    private PeriodicTimer? _timer;
    private bool _firstSuccess;
    private int _exceptionCount;

    public PresenceService(
        LoggingService loggingService,
        LastfmAPIService lastfmService,
        IDiscordClient discordClient,
        SaveCfgIOService saveCfgService)
    {
        _loggingService = loggingService;
        _lastfmService = lastfmService;
        _discordClient = discordClient;
        _saveCfgService = saveCfgService;
    }

    public async void SetPresence()
    {
        long timeOfStart = DateTimeOffset.Now.ToUnixTimeSeconds();
        bool turnOffPresence = false;
        await PresenceLock.WaitAsync();

        try
        {
            _discordClient.Initialize();
            _firstSuccess = false;

            using (_timer = new PeriodicTimer(TimeSpan.FromSeconds(2)))
            {
                while (await _timer.WaitForNextTickAsync() && !turnOffPresence)
                {
                    try
                    {
                        TrackResponse response = await _lastfmService.GetRecentTracks(_saveCfgService.SaveCfg.UserAccount.Username);
                        UpdatePresence(response);
                    }
                    catch (Exception e)
                    {
                        HandleError(e);
                    }

                    // Turn off rich presence updating and close presence when time since last scrobble
                    // AND time since presence starting is longer than an hour.
                    long currentTime = DateTimeOffset.Now.ToUnixTimeSeconds();
                    long timeSinceStart = currentTime - timeOfStart;
                    
                    // Use a LastfmService here too... there needs to be a fucking rewrite for this
                    long timeSinceLastScrobble = currentTime - _lastfmService.LastScrobbleTime;
                    turnOffPresence = timeSinceStart > _saveCfgService.SaveCfg.SleepTime &&
                                      timeSinceLastScrobble > _saveCfgService.SaveCfg.SleepTime;
                }
            }

            if (turnOffPresence)
            {
                _loggingService.Info("Turned off presence due to inactivity");
                Dispose();
            }
            
            PresenceLock.Release();
        }
        catch
        {
            PresenceLock.Release();
        }
    }

    public void UpdatePresence(TrackResponse response)
    {
        if (response.RecentTracks.Tracks.Count == 0)
        {
            _loggingService.Info("No tracks found for user.");
            ClearPresence();
        }
        else
        {
            // Track track = response.Track;
            // string albumString = IsNullOrEmpty(track.Album.Name) ? "" : $" | On {track.Album.Name}";
            //
            // I need to make a previewer as well
            // _mainViewModel.PreviewViewModel.Description = track.Name;
            // _mainViewModel.PreviewViewModel.State = $"By {track.Artist.Name}{albumString}";
            // _mainViewModel.PreviewViewModel.Tooltip = $"{response.Playcount} scrobbles";
            // _mainViewModel.PreviewViewModel.ImageURL = track.Images[3].URL;

            if (!_firstSuccess)
            {
                _loggingService.Info("Track successfully received! Attempting to connect to presence...");
            }

            if (_discordClient.IsReady)
            {
                _exceptionCount = 0;
                _discordClient.SetPresence(response);

                if (!_firstSuccess)
                {
                    _loggingService.Info("Presence has been set!");
                }

                _firstSuccess = true;
            }
            else
            {
                _loggingService.Warning("Discord client not initialised. Please restart and use a valid ID.");
                Dispose();
            }
        }
    }
    
    public void ClearPresence()
    {
        _timer?.Dispose();
        _discordClient.ClearPresence();
    }

    private void HandleError(Exception e)
    {
        switch (e)
        {
            case LastfmException exception:
            {
                _loggingService.Error("Last.fm {0}", exception.Message);

                if (exception.ErrorCode is LastfmErrorCode.Temporary or LastfmErrorCode.OperationFail && IsRetry())
                {
                    TryReconnect();
                }
                else
                {
                    ClearPresence();
                }
                break;
            }
            case HttpRequestException requestException:
            {
                _loggingService.Error("HTTP {0}: {1}", requestException.StatusCode ?? 0, requestException.Message);

                if (IsRetry())
                {
                    TryReconnect();
                }
                else
                {
                    ClearPresence();
                }
                break;
            }
            default:
                _loggingService.Error("Unhandled exception! Please report to developers {0}", e.Message);
                ClearPresence();
                break;
        }
    }

    private void TryReconnect()
    {
        ClearPresence();
        SetPresence();
        _loggingService.Info($"Attempting to reconnect... Try {_exceptionCount}");
    }

    private bool IsRetry()
    {
        return _exceptionCount++ < 3;
    }

    public void Dispose() => ClearPresence();
}
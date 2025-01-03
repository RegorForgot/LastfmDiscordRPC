﻿using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using LastfmDiscordRPC2.DataTypes;
using LastfmDiscordRPC2.Exceptions;
using LastfmDiscordRPC2.IO;
using LastfmDiscordRPC2.Logging;
using LastfmDiscordRPC2.Models.API;
using LastfmDiscordRPC2.Models.Responses;
using LastfmDiscordRPC2.ViewModels;
using Newtonsoft.Json;

namespace LastfmDiscordRPC2.Models.RPC;

public sealed class PresenceService : IPresenceService
{
    private readonly LoggingService _loggingService;
    private readonly LastfmAPIService _lastfmService;
    private readonly IDiscordClient _discordClient;
    private readonly SaveCfgIOService _saveCfgService;
    private readonly UIContext _context;

    private readonly PeriodicTimer _timer;
    private CancellationTokenSource _timerCancellationTokenSource;

    private bool _isFirstSuccess;
    private int _exceptionCount;
    private DateTimeOffset _presenceStartedTime;

    private bool IsRetry => _exceptionCount <= 3;

    public PresenceService(
        LoggingService loggingService,
        LastfmAPIService lastfmService,
        IDiscordClient discordClient,
        SaveCfgIOService saveCfgService,
        UIContext context)
    {
        _loggingService = loggingService;
        _lastfmService = lastfmService;
        _discordClient = discordClient;
        _saveCfgService = saveCfgService;
        _context = context;
        _timer = new PeriodicTimer(TimeSpan.FromSeconds(2));
        _timerCancellationTokenSource = new CancellationTokenSource();
    }

    public async Task SetPresence()
    {
        _timerCancellationTokenSource = new CancellationTokenSource();
        _exceptionCount = 0;
        SaveCfg saveSnapshot = _saveCfgService.GetSaveSnapshot();

        _isFirstSuccess = true;
        _discordClient.Initialize(saveSnapshot);
        _presenceStartedTime = DateTimeOffset.Now;
        await PresenceLoop(saveSnapshot);
    }

    private async Task PresenceLoop(SaveCfg saveSnapshot)
    {
        int count = 0;
        try
        {
            while (await _timer.WaitForNextTickAsync(_timerCancellationTokenSource.Token).ConfigureAwait(false))
            {
                if (count++ < 3 && !_discordClient.IsReady)
                {
                    if (count == 3)
                    {
                        UnsetPresence();
                    }
                    continue;
                }
                count = 0;
                try
                {
                    TrackResponse response = await _lastfmService.GetRecentTracks(saveSnapshot.UserAccount.Username);
                    UpdatePresence(response);
                }
                catch (Exception e)
                {
                    bool retry = HandleError(e);
                    if (retry)
                    {
                        _loggingService.Info($"Attempting to reconnect... Try {_exceptionCount}");
                    }
                    else
                    {
                        _timerCancellationTokenSource.Cancel();
                    }
                }

                PresenceExpiry(saveSnapshot.UserRPCCfg.ExpiryMode, saveSnapshot.UserRPCCfg.ExpiryTime);
            }
        }
        catch (OperationCanceledException)
        {
            _loggingService.Info("Presence has been expired.");
            ClearPresence();
        }
    }

    private void UpdatePresence(TrackResponse response)
    {
        if (response.RecentTracks.Tracks.Count == 0)
        {
            _loggingService.Info("No tracks found for user.");
            UnsetPresence();
            return;
        }

        if (_isFirstSuccess)
        {
            _loggingService.Info("Track successfully received! Attempting to connect to presence...");
        }

        if (_discordClient.IsReady)
        {
            _exceptionCount = 0;
            _discordClient.SetPresence(response);

            if (_isFirstSuccess)
            {
                _loggingService.Info("Presence has been set!");
            }

            _isFirstSuccess = false;
            return;
        }

        _loggingService.Warning("Discord client not initialised. Please restart and use a valid ID.");
        UnsetPresence();
    }

    public void UnsetPresence()
    {
        _timerCancellationTokenSource.Cancel();
    }


    private void ClearPresence()
    {
        _context.IsRichPresenceActivated = false;
        _discordClient.ClearPresence();
    }

    private bool HandleError(Exception e)
    {
        switch (e)
        {
            case LastfmException exception:
            {
                _loggingService.Error($"Last.fm {exception.Message}");
                _exceptionCount++;
                return exception.ErrorCode is LastfmErrorCode.Temporary or LastfmErrorCode.OperationFail && IsRetry;
            }
            case HttpRequestException requestException:
            {
                _loggingService.Error($"HTTP {requestException.StatusCode ?? 0}: {requestException.Message}");
                _exceptionCount++;
                return IsRetry;
            }
            case JsonReaderException readerException:
            {
                _loggingService.Error($"Malformed JSON received\n{readerException.Message}");
                _exceptionCount++;
                return IsRetry;
            }
            default:
                _loggingService.Error($"Unhandled exception! Please report to developers\n{e.Message}\n{e.StackTrace ?? Empty}");
                return false;
        }
    }

    private void PresenceExpiry(bool expiryMode, TimeSpan expiryTime)
    {
        if (!expiryMode)
        {
            return;
        }

        DateTimeOffset currentTime = DateTimeOffset.Now;

        TimeSpan timeSincePresenceStarted = currentTime - _presenceStartedTime;
        TimeSpan timeSinceLastScrobble = currentTime - _lastfmService.LastScrobbleTime;

        bool expired = timeSincePresenceStarted > expiryTime && timeSinceLastScrobble > expiryTime;
        if (expired)
        {
            _timerCancellationTokenSource.Cancel();
        }
    }

    public void Dispose()
    {
        ClearPresence();
        _timer.Dispose();
        _timerCancellationTokenSource.Dispose();
    }
}
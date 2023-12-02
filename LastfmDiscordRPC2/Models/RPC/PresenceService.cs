using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using LastfmDiscordRPC2.Enums;
using LastfmDiscordRPC2.Exceptions;
using LastfmDiscordRPC2.IO;
using LastfmDiscordRPC2.Logging;
using LastfmDiscordRPC2.Models.API;
using LastfmDiscordRPC2.Models.Responses;
using LastfmDiscordRPC2.ViewModels;

namespace LastfmDiscordRPC2.Models.RPC;

public sealed class PresenceService : IPresenceService
{
    private readonly LoggingService _loggingService;
    private readonly LastfmAPIService _lastfmService;
    private readonly IDiscordClient _discordClient;
    private readonly SaveCfgIOService _saveCfgService;
    private readonly UIContext _context;

    private PeriodicTimer? _timer;
    private bool _isFirstSuccess;
    private bool _isPresenceExpired;
    private bool _isPresenceTurnedOff;
    
    private int _exceptionCount;
    private long _presenceStartedTime;
    
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
    }

    public async Task SetPresence()
    {
        _presenceStartedTime = DateTimeOffset.Now.ToUnixTimeSeconds();
        _isPresenceExpired = false;
        _isPresenceTurnedOff = false;
        _exceptionCount = 0;

        try
        {
            _discordClient.Initialize();
            _isFirstSuccess = true;

            using (_timer = new PeriodicTimer(TimeSpan.FromSeconds(2)))
            {
                while (await _timer.WaitForNextTickAsync() && !_isPresenceExpired && !_isPresenceTurnedOff)
                {
                    try
                    {
                        TrackResponse response = await _lastfmService.GetRecentTracks(_saveCfgService.SaveCfg.UserAccount.Username);
                        _isPresenceTurnedOff = !UpdatePresence(response);
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
                            _isPresenceTurnedOff = true;
                        }
                    }

                    _isPresenceExpired = IsPresenceExpired();
                }
            }

        }
        catch
        {
            // ignored
        }

        if (_isPresenceExpired)
        {
            _loggingService.Info("Presence disconnected due to inactivity.");
        }
        
        if (_isPresenceTurnedOff)
        {
            _loggingService.Info("Presence disabled.");
        }
        
        ClearPresence();
    }

    private bool UpdatePresence(TrackResponse response)
    {
        if (response.RecentTracks.Tracks.Count == 0)
        {
            _loggingService.Info("No tracks found for user.");
            return false;
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
            return true;
        }
        
        _loggingService.Warning("Discord client not initialised. Please restart and use a valid ID.");
        return false;
    }

    public void UnsetPresence()
    {
        _isPresenceTurnedOff = true;
    }
    

    public void ClearPresence()
    {
        _timer?.Dispose();
        _discordClient.ClearPresence();
        
        _context.IsRichPresenceActivated = false;
    }

    private bool HandleError(Exception e)
    {
        switch (e)
        {
            case LastfmException exception:
            {
                _loggingService.Error("Last.fm {0}", exception.Message);
                _exceptionCount++;
                return exception.ErrorCode is LastfmErrorCode.Temporary or LastfmErrorCode.OperationFail && IsRetry;
            }
            case HttpRequestException requestException:
            {
                _loggingService.Error("HTTP {0}: {1}", requestException.StatusCode ?? 0, requestException.Message);
                _exceptionCount++;
                return IsRetry;
            }
            default:
                _loggingService.Error("Unhandled exception! Please report to developers {0}", e.Message);
                return false;
        }
    }
        
    private bool IsPresenceExpired()
    {
        long currentTime = DateTimeOffset.Now.ToUnixTimeSeconds();

        long timeSincePresenceStarted = currentTime - _presenceStartedTime;
        long timeSinceLastScrobble = currentTime - _lastfmService.LastScrobbleTime;

        return timeSincePresenceStarted > _saveCfgService.SaveCfg.UserRPCCfg.SleepTime &&
                             timeSinceLastScrobble > _saveCfgService.SaveCfg.UserRPCCfg.SleepTime;
    }

    public void Dispose() => ClearPresence();
}
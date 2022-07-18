using System;
using System.Net.Http;
using System.Threading;
using LastfmDiscordRPC.ViewModels;

namespace LastfmDiscordRPC.Models;

public class PresenceSetter : IDisposable
{
    private readonly static SemaphoreSlim PresenceLock = new SemaphoreSlim(1, 1);
    private readonly MainViewModel _mainViewModel;
    private readonly LastfmClient _lastfmClient;
    private LastfmResponse? _response;
    private PeriodicTimer? _timer;
    private bool _firstSuccess;
    public bool IsReady { get; set; }

    public PresenceSetter(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
        _lastfmClient = mainViewModel.LastfmClient;
    }

    public async void UpdatePresence(string username, string apiKey)
    {
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
                            ConnectionError(e);
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
            _mainViewModel.WriteToOutput("\n+ No tracks found for user.\n+");
            Dispose();
        } else
        {
            Track track = _response.Track;
            _mainViewModel.PreviewViewModel.Name = track.Name;
            _mainViewModel.PreviewViewModel.AlbumName = track.Album.Name;
            _mainViewModel.PreviewViewModel.ArtistName = track.Artist.Name;
            _mainViewModel.PreviewViewModel.ImageURL = track.Images[3].URL;

            if (!_firstSuccess)
                _mainViewModel.WriteToOutput("\n+ Track successfully received! Attempting to connect to presence...");

            if (_mainViewModel.DiscordClient.IsInitialised)
            {
                _mainViewModel.DiscordClient.SetPresence(_response, username);
                if (!_firstSuccess)
                    _mainViewModel.WriteToOutput("\n+ Connected to presence!" +
                                                 "\n+ If presence is not showing, please check log file.\n");
                _firstSuccess = true;
            } else
            {
                _mainViewModel.WriteToOutput("\n+ DiscordClient not initialised. Please enter a valid Discord " +
                                            "App ID, save, then restart.\n");
                Dispose();
            }
        }
    }

    private void ConnectionError(Exception e)
    {
        Dispose();
        if (e.GetType() == typeof(LastfmException))
            _mainViewModel.WriteToOutput($"\n+ Error '{((LastfmException)e).ErrorCode}' from Last.fm: {e.Message}\n");
        else if (e.GetType() == typeof(HttpRequestException))
            _mainViewModel.WriteToOutput($"\n+ HTTP Error '{((HttpRequestException)e).StatusCode}': {e.Message}\n");
        else
            _mainViewModel.WriteToOutput($"\n+ Unhandled Exception: {e.Message}\n");
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
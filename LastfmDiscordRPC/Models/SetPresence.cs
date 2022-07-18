using System;
using System.Net.Http;
using System.Threading;
using LastfmDiscordRPC.ViewModels;
using static LastfmDiscordRPC.Models.LastfmClient;

namespace LastfmDiscordRPC.Models;

public static class SetPresence
{
    private readonly static SemaphoreSlim PresenceLock;
    private static LastfmResponse? _response;
    private static PeriodicTimer? _timer;
    private static bool _firstSuccess;
    public static bool IsReady { get; set; }

    static SetPresence()
    {
        PresenceLock = new SemaphoreSlim(1, 1);
    }

    public static async void UpdatePresence(string username, string apiKey, MainViewModel mainViewModel)
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
                            _response = await CallAPI(username, apiKey);
                            TrySetPresence(mainViewModel, username);
                            _response = null;
                        } catch (Exception e)
                        {
                            ConnectionError(mainViewModel, e);
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

    private static void TrySetPresence(MainViewModel mainViewModel, string username)
    {
        if (_response?.Track == null)
        {
            mainViewModel.WriteToOutput("\n+ No tracks found for user.\n+");
            Dispose();
        } else
        {
            Track track = _response.Track;
            mainViewModel.PreviewViewModel.Name = track.Name;
            mainViewModel.PreviewViewModel.AlbumName = track.Album.Name;
            mainViewModel.PreviewViewModel.ArtistName = track.Artist.Name;
            mainViewModel.PreviewViewModel.ImageURL = track.Images[3].URL;

            if (!_firstSuccess)
                mainViewModel.WriteToOutput(
                    "\n+ Track successfully received! Attempting to connect to presence...");

            if (mainViewModel.Client.IsInitialised)
            {
                mainViewModel.Client.SetPresence(_response, username);
                if (!_firstSuccess)
                    mainViewModel.WriteToOutput("\n+ Connected to presence!\n+ If presence " +
                                                "is not showing, please check log file.\n");
                _firstSuccess = true;
            } else
            {
                mainViewModel.WriteToOutput("\n+ Client not initialised. Please enter a valid Discord " +
                                            "App ID, save, then restart.\n");
                Dispose();
            }
        }
    }

    private static void ConnectionError(MainViewModel mainViewModel, Exception e)
    {
        Dispose();
        if (e.GetType() == typeof(LastfmException))
            mainViewModel.WriteToOutput($"\n+ Error '{((LastfmException)e).ErrorCode}' from Last.fm: {e.Message}\n");
        else if (e.GetType() == typeof(HttpRequestException))
            mainViewModel.WriteToOutput($"\n+ HTTP Error '{((HttpRequestException)e).StatusCode}': {e.Message}\n");
        else
            mainViewModel.WriteToOutput($"\n+ Unhandled Exception: {e.Message}\n");
    }

    public static void Dispose() => _timer?.Dispose();
}
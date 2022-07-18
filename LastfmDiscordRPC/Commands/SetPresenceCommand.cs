using System;
using System.Net.Http;
using System.Threading;
using LastfmDiscordRPC.Models;
using LastfmDiscordRPC.ViewModels;
using static LastfmDiscordRPC.Models.LastfmClient;

namespace LastfmDiscordRPC.Commands;

public class SetPresenceCommand : CommandBase, IDisposable
{
    private LastfmResponse? _response;
    private PeriodicTimer? _timer;
    private bool _isActivated;
    private readonly static SemaphoreSlim PresenceLock = new SemaphoreSlim(1, 1);

    public SetPresenceCommand(MainViewModel mainViewModel) : base(mainViewModel)
    { }

    public override void Execute(object? parameter)
    {
        UpdateButtonExecute();
        TimedPresenceUpdater(MainViewModel.Username, MainViewModel.APIKey);
    }

    private async void UpdateButtonExecute()
    {
        using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromSeconds(3));
        Dispose();
        _isActivated = true;
        RaiseCanExecuteChanged();

        while (await timer.WaitForNextTickAsync())
        {
            _isActivated = false;
            RaiseCanExecuteChanged();
            timer.Dispose();
        }
    }

    private async void TimedPresenceUpdater(string username, string apiKey)
    {
        bool firstSuccess = false;

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

                            if (_response?.Track == null)
                            {
                                MainViewModel.WriteToOutput("\n+ No tracks found for user.\n+");
                                Dispose();
                            } else
                            {
                                Track track = _response.Track;
                                MainViewModel.PreviewViewModel.Name = track.Name;
                                MainViewModel.PreviewViewModel.AlbumName = track.Album.Name;
                                MainViewModel.PreviewViewModel.ArtistName = track.Artist.Name;
                                MainViewModel.PreviewViewModel.ImageURL = track.Images[3].URL;

                                if (!firstSuccess)
                                    MainViewModel.WriteToOutput(
                                        "\n+ Track successfully received! Attempting to connect to presence...");

                                if (MainViewModel.Client.IsInitialised)
                                {
                                    MainViewModel.Client.SetPresence(_response, username);
                                    if (!firstSuccess)
                                        MainViewModel.WriteToOutput("\n+ Connected to presence!\n+ If presence " +
                                                                    "is not showing, please check log file.\n");
                                    firstSuccess = true;
                                } else
                                {
                                    MainViewModel.WriteToOutput("\n+ Client not initialised. Please enter a valid Discord " +
                                                                "App ID, save, then restart.\n");
                                    Dispose();
                                }
                            }

                            _response = null;
                        } catch (Exception e)
                        {
                            Dispose();
                            if (e.GetType() == typeof(LastfmException))
                                MainViewModel.WriteToOutput(
                                    $"\n+ Error '{((LastfmException)e).ErrorCode}' from Last.fm: {e.Message}\n");
                            else if (e.GetType() == typeof(HttpRequestException))
                                MainViewModel.WriteToOutput(
                                    $"\n+ HTTP Error '{((HttpRequestException)e).StatusCode}': {e.Message}\n");
                            else
                                MainViewModel.WriteToOutput($"\n+ Unhandled Exception: {e.Message}\n");
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

    public override bool CanExecute(object? parameter)
    {
        return !_isActivated && !MainViewModel.HasErrors;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _timer?.Dispose();
    }
}
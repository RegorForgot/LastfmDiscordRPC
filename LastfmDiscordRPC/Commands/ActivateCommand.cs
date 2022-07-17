using System;
using System.Net.Http;
using System.Threading;
using LastfmDiscordRPC.Exceptions;
using LastfmDiscordRPC.Models;
using LastfmDiscordRPC.ViewModels;
using static LastfmDiscordRPC.Models.LastfmClient;
using Track = LastfmDiscordRPC.Models.Track;

namespace LastfmDiscordRPC.Commands;

public class ActivateCommand : CommandBase, IDisposable
{
    private readonly MainViewModel _mainViewModel;
    private PeriodicTimer? _timer;
    public bool IsWorking { get; set; }

    public ActivateCommand(MainViewModel mainViewModel) => _mainViewModel = mainViewModel;
    
    private async void TimedPresenceUpdater(string username, string apiKey)
    {
        _timer = new PeriodicTimer(TimeSpan.FromSeconds(2));
        bool firstSuccess = false;
        IsWorking = true;
        RaiseCanExecuteChanged();

        while (await _timer.WaitForNextTickAsync()) {
            try
            {
                IsWorking = true;
                LastfmResponse trackResponse = await CallAPI(username, apiKey);

                if (trackResponse.Track == null)
                {
                    _mainViewModel.OutputText += "\n+ No tracks found for user.";
                    _timer.Dispose();
                } else
                {

                    Track track = trackResponse.Track;
                    _mainViewModel.PreviewViewModel.Name = track.Name;
                    _mainViewModel.PreviewViewModel.AlbumName = track.Album.Name;
                    _mainViewModel.PreviewViewModel.ArtistName = track.Artist.Name;
                    _mainViewModel.PreviewViewModel.ImageURL = track.Images[3].URL;


                    if (!firstSuccess) _mainViewModel.OutputText +=
                            "\n+ Track successfully received! Attempting to connect to presence...";
                    firstSuccess = true;
                    _mainViewModel.Client.SetPresence(trackResponse, username);
                    ((DeactivateCommand) _mainViewModel.DeactivateCommand).RaiseCanExecuteChanged();
                }
            } catch (Exception e)
            {
                _timer.Dispose();
                if (e.GetType() == typeof(LastfmException))
                    _mainViewModel.OutputText += $"\n+ Error '{((LastfmException)e).ErrorCode}' from Last.fm: {e.Message}";
                else if (e.GetType() == typeof(HttpRequestException))
                    _mainViewModel.OutputText += $"\n+ HTTP Error '{((HttpRequestException)e).StatusCode}': {e.Message}";
                else
                    _mainViewModel.OutputText += $"\n+ Unhandled Exception: {e.Message}";
            } 
        } 
        
        Dispose();
    }

    public override bool CanExecute(object? parameter)
    {
        return !_mainViewModel.Client.HasPresence() && !IsWorking;
    }

    public override void Execute(object? parameter)
    { 
        _mainViewModel.Client.ClearPresence();
        TimedPresenceUpdater(_mainViewModel.Username, _mainViewModel.APIKey);
    } 

    public void Dispose()
    {
        _mainViewModel.Client.ClearPresence();
        _timer?.Dispose();
        IsWorking = false;
        RaiseCanExecuteChanged();
    } 
}
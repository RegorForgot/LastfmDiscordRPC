using System.Net.Http;
using LastfmDiscordRPC.Exceptions;
using LastfmDiscordRPC.Models;
using LastfmDiscordRPC.ViewModels;
using static LastfmDiscordRPC.Models.LastfmClient;
using Track = LastfmDiscordRPC.Models.Track;

namespace LastfmDiscordRPC.Commands;

public class ActivateCommand : CommandBase
{
    private readonly MainViewModel _mainViewModel;

    public ActivateCommand(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
    }

    public override async void Execute(object? parameter)
    {
        string username = _mainViewModel.Username;
        string apiKey = _mainViewModel.APIKey;
        PreviewViewModel previewViewModel = _mainViewModel.PreviewViewModel;

        try
        {
            LastfmResponse trackResponse = await CallAPI(username, apiKey);

            if (trackResponse.Track == null)
            {
                _mainViewModel.OutputText += "\n+ No tracks found for user.";
                return;
            }
            
            Track track = trackResponse.Track;
            previewViewModel.Name = track.Name;
            previewViewModel.AlbumName = track.Album.Name;
            previewViewModel.ArtistName = track.Artist.Name;
            previewViewModel.ImageURL = track.Images[3].URL;

            _mainViewModel.OutputText += "\n+ Current track successfully received!";
            _mainViewModel.Client.SetPresence(trackResponse, username);
        } catch (LastfmException e)
        {
            _mainViewModel.OutputText += $"\n+ Error '{e.ErrorCode}' from Last.fm: {e.Message}";
        } catch (HttpRequestException e)
        {
            _mainViewModel.OutputText += $"\n+ HTTP Error '{e.StatusCode}': {e.Message}";
        }
    }
}
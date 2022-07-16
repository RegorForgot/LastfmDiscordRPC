using System.Net.Http;
using LastfmAPI.Exceptions;
using LastfmAPI.Responses;
using LastfmDiscordRPC.MVVM.Models;
using LastfmDiscordRPC.MVVM.ViewModels;
using static LastfmAPI.APIConnector;

namespace LastfmDiscordRPC.MVVM.Commands;

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
            UserResponse userResponse = (UserResponse)await CallAPI(username, apiKey, GetUser);
            _mainViewModel.OutputText = "\n+ User found... collecting tracks";
            
            TrackResponse trackResponse = (TrackResponse) await CallAPI(username, apiKey, GetTracks);

            if (trackResponse.Track == null)
            {
                _mainViewModel.OutputText += "\n+ No tracks found for user.";
                return; 
            }

            UserObject user = userResponse.User;
            Track track = trackResponse.Track;
            previewViewModel.Name = track.Name;
            previewViewModel.AlbumName = track.Album.Name;
            previewViewModel.ArtistName = track.Artist.Name;
            previewViewModel.ImageURL = track.ImageURL;

            _mainViewModel.OutputText += "\n+ Current track successfully received!";
            _mainViewModel.Client.InitialiseClient(userResponse.User, track);
        } catch (LastfmException e)
        {
            _mainViewModel.OutputText += $"\n+ Error '{e.ErrorCode}' from Last.fm: {e.Message}";
        } catch (HttpRequestException e)
        {
            _mainViewModel.OutputText += $"\n+ HTTP Error '{e.StatusCode}': {e.Message}";
        }
    }
}
using System.Net.Http;
using System.Windows;
using LastfmAPI.Exceptions;
using LastfmAPI.Responses;
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
            TrackResponse trackResponse = (TrackResponse) await CallAPI(username, apiKey, GetTracks);

            Track? track = trackResponse.Track;
            if (track == null)
            {
                MessageBox.Show("No track found", username);
                return; 
            }

            previewViewModel.Name = track.Name;
            previewViewModel.AlbumName = track.Album.Name;
            previewViewModel.ArtistName = track.Artist.Name;
            previewViewModel.ImageURL = track.ImageURL;
        } catch (LastfmException e)
        {
            MessageBox.Show(e.Message, e.ErrorCode.ToString());
        } catch (HttpRequestException e)
        {
            MessageBox.Show(e.Message, e.StatusCode.ToString());
        }
    }
}
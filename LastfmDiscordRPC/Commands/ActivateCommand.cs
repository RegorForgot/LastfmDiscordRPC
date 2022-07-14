using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using LastfmAPI.Exceptions;
using LastfmAPI.Responses;
using static LastfmAPI.APIConnector;

namespace LastfmDiscordRPC.Commands;

public class ActivateCommand : CommandBase
{
    private readonly ViewModel _viewModel;
    public ActivateCommand(ViewModel viewModel)
    {
        _viewModel = viewModel;
    }
    
    public override async void Execute(object? parameter)
    {
        string username = _viewModel.Username;
        string apiKey = _viewModel.APIKey;
        PreviewViewModel previewViewModel = _viewModel.PreviewViewModel;

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
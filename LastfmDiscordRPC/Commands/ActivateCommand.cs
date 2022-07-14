using System.Net.Http;
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
    
    public override void Execute(object? parameter)
    {
        string username = _viewModel.Username;
        string apiKey = _viewModel.APIKey;

        try
        {
            UserResponse userResponse = (UserResponse) CallAPI(username, apiKey, GetUser);
            TrackResponse trackResponse = (TrackResponse) CallAPI(username, apiKey, GetTracks);

            Track? track = trackResponse.Track;
            if (track == null)
            {
                MessageBox.Show("No track found", username);
                return; 
            }

            UserObject user = userResponse.User;
            MessageBox.Show($"{track.Artist.Name} - {track.Album.Name} - {track.Name}", $"{username}: {user.PlayCount} plays");
        } catch (LastfmException e)
        {
            MessageBox.Show(e.Message, e.ErrorCode.ToString());
        } catch (HttpRequestException e)
        {
            MessageBox.Show(e.Message, e.StatusCode.ToString());
        }
    }
}
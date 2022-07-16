using System;
using DiscordRPC;
using LastfmAPI.Responses;

namespace LastfmDiscordRPC.MVVM.Models;

public class DiscordClient : IDisposable
{
    private DiscordRpcClient? _client;
    private RichPresence? _presence;
    private bool _isConnected;
    private bool IsConnected
    {
        get => _isConnected;
        set
        {
            if (value == _isConnected) return;
            
            _isConnected = value;
            if (_isConnected || (bool)_client?.IsDisposed) return;
            
            try
            {
                _client?.ClearPresence();
            } catch (ObjectDisposedException)
            { } finally
            {
                _client?.Dispose();
            }
        }
    }
    
    public string AppID { get; set; }
    
    public DiscordClient(string appId)
    {
        AppID = appId;
        _presence = null;
        _isConnected = false;
    }

    /// <inheritdoc />
    public void Dispose() => _client?.Dispose();

    private void SetPresence(UserObject user, Track track)
    {
        RichPresence presence = new RichPresence()
        {
            State = track.Name,
            Details = track.Artist.Name,
            Assets = new Assets()
            {
                LargeImageKey = track.ImageURL,
                LargeImageText = track.Album.Name
            }
        };
        _client?.SetPresence(presence);
    }

    public void InitialiseClient(UserObject user, Track track)
    {
        _client = new DiscordRpcClient(AppID);
        IsConnected = true;
        
        SetPresence(user, track);
    }
}
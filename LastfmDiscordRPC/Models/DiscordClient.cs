using System;
using DiscordRPC;
using static System.String;

namespace LastfmDiscordRPC.Models;

public class DiscordClient : IDisposable
{
    private DiscordRpcClient? _client;
    private RichPresence? _presence;
    private bool _isConnected;
    public bool IsConnected
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
                _client?.Deinitialize();
            } catch (ObjectDisposedException)
            { } finally
            {
                _client?.Dispose();
            }
        }
    }
    
    private string AppID { get; set; }
    
    public DiscordClient(string appId)
    {
        AppID = appId;
        _presence = null;
        _isConnected = false;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        IsConnected = false;
    }

    private void SetPresence(LastfmResponse response)
    {
        Track track = response.Track ?? throw new NullReferenceException("Cannot connect - track does not exist in response");
        string albumString = IsNullOrEmpty(track.Album.Name) ? "" : $" | On {track.Album.Name}";
        RichPresence presence = new RichPresence
        {
            Details = track.Name,
            State = $"By {track.Artist.Name}{albumString}",
            Assets = new Assets
            {
                LargeImageKey = track.Images[3].URL,
                LargeImageText = track.Album.Name
            }
        };
        _client?.SetPresence(presence);
    }

    public void InitialiseClient(LastfmResponse response)
    {
        _client = new DiscordRpcClient(AppID);
        _client.Initialize();
        IsConnected = true;
        SetPresence(response);
    }
}
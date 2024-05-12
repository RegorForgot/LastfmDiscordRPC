using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using DiscordRPC;
using DiscordRPC.Exceptions;
using LastfmDiscordRPC2.DataTypes;
using LastfmDiscordRPC2.IO;
using LastfmDiscordRPC2.Logging;
using LastfmDiscordRPC2.Models.API;
using LastfmDiscordRPC2.Models.Responses;
using LastfmDiscordRPC2.Utilities;
using LastfmDiscordRPC2.ViewModels.Controls;

namespace LastfmDiscordRPC2.Models.RPC;

public sealed class DiscordClient : IDiscordClient
{
    private const string PauseIconURL = "https://i.imgur.com/AOYINL0.png";
    private const string PlayIconURL = "https://i.imgur.com/wvTxH0t.png";

    private DiscordRpcClient? _client;

    private readonly LoggingService _loggingService;
    private readonly PreviewControlViewModel _previewControlViewModel;
    private readonly LastfmAPIService _lastfmService;

    private SaveCfg _saveSnapshot;

    public bool IsReady { get; private set; }

    public DiscordClient(
        LastfmAPIService lastfmService,
        LoggingService loggingService,
        PreviewControlViewModel previewControlViewModel)
    {
        _lastfmService = lastfmService;
        _loggingService = loggingService;
        _previewControlViewModel = previewControlViewModel;
    }

    public void Initialize(SaveCfg saveCfg)
    {
        _saveSnapshot = saveCfg;

        if (_client is not null && _client.IsInitialized)
        {
            return;
        }

        IsReady = false;

        _client = new DiscordRpcClient(_saveSnapshot.UserRPCCfg.AppID);
        _client.Logger = _loggingService;
        _client.Initialize();

        SetEventHandlers();
    }

    private void SetEventHandlers()
    {
        if (_client == null)
        {
            return;
        }

        _client.OnClose += (_, _) =>
        {
            _loggingService.Error("Could not connect to Discord.");
            IsReady = false;
        };
        _client.OnError += (_, _) =>
        {
            _loggingService.Error("There has been an error trying to connect to Discord.");
            IsReady = false;
        };
        _client.OnConnectionFailed += (_, _) =>
        {
            _loggingService.Error("Connection to discord failed. Check if your Discord app is open.");
            IsReady = false;
            _client.Deinitialize();
        };
        _client.OnConnectionEstablished += (_, _) =>
        {
            _loggingService.Info("Connection to discord succeeded.");
        };
        _client.OnReady += (_, _) =>
        {
            _loggingService.Info("Client ready.");
            IsReady = true;
        };
    }

    private RichPresence GetRichPresence(TrackResponse response)
    {
        _previewControlViewModel.IsVisible = true;
        Track track = response.RecentTracks.Tracks[0];

        if (track.NowPlaying.State == "true")
        {
            _lastfmService.LastScrobbleTime = DateTimeOffset.Now;
        }
        else
        {
            bool success = long.TryParse(track.Date.Timestamp, NumberStyles.Number, null, out long unixLastScrobbleTime);
            if (success)
            {
                _lastfmService.LastScrobbleTime = DateTimeOffset.FromUnixTimeSeconds(unixLastScrobbleTime);
            }
        }

        Button[] buttons = _saveSnapshot.UserRPCCfg.UserButtons.Select(
            button => new Button
            {
                Label = this.GetParsedString(response, button.Label, ByteCount.ButtonLabel),
                Url = this.GetParsedLink(response, button.URL, ByteCount.ButtonLink)
            }
        ).ToArray();

        return new RichPresence
        {
            Details = this.GetParsedString(response, _saveSnapshot.UserRPCCfg.Details, ByteCount.Default),
            State = this.GetParsedString(response, _saveSnapshot.UserRPCCfg.State, ByteCount.Default),
            Assets = new DiscordRPC.Assets
            {
                LargeImageKey = IsNullOrEmpty(track.Album.Name) ? Track.DefaultSingleCover : track.Images[3].URL,
                LargeImageText = this.GetParsedString(response, _saveSnapshot.UserRPCCfg.LargeImageLabel, ByteCount.Default),
                SmallImageKey = track.NowPlaying.State == "true" ? PlayIconURL : PauseIconURL,
                SmallImageText = this.GetParsedString(response, _saveSnapshot.UserRPCCfg.SmallImageLabel, ByteCount.Default)
            },
            Buttons = buttons
        };
    }

    private void SetPreview(RichPresence presence, TrackResponse trackResponse)
    {
        _previewControlViewModel.CurrentTrack = trackResponse;
        _previewControlViewModel.IsTrackLoved = trackResponse.RecentTracks.Tracks[0].Loved == "1";
        _previewControlViewModel.State = presence.State;
        _previewControlViewModel.Details = presence.Details;
        _previewControlViewModel.LargeImage.Label = presence.Assets.LargeImageText;
        _previewControlViewModel.LargeImage.URL = presence.Assets.LargeImageKey;
        _previewControlViewModel.SmallImage.Label = presence.Assets.SmallImageText;
        _previewControlViewModel.SmallImage.URL = presence.Assets.SmallImageKey;
        _previewControlViewModel.Buttons =
            new ObservableCollection<RPCButton>(presence.Buttons.ToList().Select(
                button => new RPCButton
                {
                    URL = button.Url,
                    Label = button.Label
                }
            ));
    }

    public void SetPresence(TrackResponse response)
    {
        RichPresence presence = GetRichPresence(response);
        SetPreview(presence, response);
        _client?.SetPresence(presence);
    }

    public void ClearPresence()
    {
        _previewControlViewModel.IsVisible = false;
        _client?.ClearPresence();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        if (_client == null)
        {
            return;
        }

        try
        {
            _client.ClearPresence();
            _client.Deinitialize();
            _client.Dispose();
            IsReady = false;
        }
        catch (Exception ex) when (ex is ObjectDisposedException or UninitializedException) { }
    }
}
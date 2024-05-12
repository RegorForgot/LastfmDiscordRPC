using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using LastfmDiscordRPC2.Exceptions;
using LastfmDiscordRPC2.IO;
using LastfmDiscordRPC2.Logging;
using LastfmDiscordRPC2.Models;
using LastfmDiscordRPC2.Models.API;
using LastfmDiscordRPC2.Models.Responses;
using LastfmDiscordRPC2.Utilities;
using ReactiveUI;

namespace LastfmDiscordRPC2.ViewModels.Controls;

public class PreviewControlViewModel : AbstractControlViewModel
{
    public override string Name => "PreviewControl";

    private bool _isVisible;
    private string _details = Empty;
    private string _state = Empty;
    private bool _isTrackLoved;
    private bool _isCoolDown;

    private TrackResponse _currentTrack;
    private ObservableCollection<RPCButton> _buttons = new ObservableCollection<RPCButton>();

    public bool IsTrackLoved
    {
        get => _isTrackLoved;
        set => this.RaiseAndSetIfChanged(ref _isTrackLoved, value);
    }

    public bool IsCoolDown
    {
        get => _isCoolDown;
        private set => this.RaiseAndSetIfChanged(ref _isCoolDown, value);
    }
    
    public string Details
    {
        get => _details;
        set => this.RaiseAndSetIfChanged(ref _details, value);
    }

    public string State
    {
        get => _state;
        set => this.RaiseAndSetIfChanged(ref _state, value);
    }

    public TrackResponse CurrentTrack
    {
        get => _currentTrack;
        set => this.RaiseAndSetIfChanged(ref _currentTrack, value);
    }

    public ObservableCollection<RPCButton> Buttons
    {
        get => _buttons;
        set => this.RaiseAndSetIfChanged(ref _buttons, value);
    }

    public RPCImage LargeImage { get; } = new RPCImage();
    public RPCImage SmallImage { get; } = new RPCImage();

    public bool IsVisible
    {
        get => _isVisible;
        set => this.RaiseAndSetIfChanged(ref _isVisible, value);
    }

    public ReactiveCommand<string, Unit> OpenArtCmd { get; }
    public ReactiveCommand<Unit, Unit> LoveTrackCmd { get; }
    private readonly LastfmAPIService _lastfmService;
    private readonly SaveCfgIOService _saveCfgIOService;
    private readonly LoggingService _loggingService;

    public PreviewControlViewModel(LastfmAPIService lastfmService, SaveCfgIOService ioService, LoggingService loggingService)
    {
        _lastfmService = lastfmService;
        _saveCfgIOService = ioService;
        _loggingService = loggingService;

        OpenArtCmd = ReactiveCommand.Create<string>(OpenArt);
        LoveTrackCmd = ReactiveCommand.CreateFromTask(LoveTrack);
    }

    private async Task LoveTrack()
    {
        try
        {
            _ = Task.Run(StartCountDown);
            _ = await _lastfmService.LoveOrUnloveTrack(IsTrackLoved, _saveCfgIOService.SaveCfg.UserAccount, CurrentTrack);
            IsTrackLoved = !IsTrackLoved;
        }
        catch (LastfmException ex)
        {
            _loggingService.Error($"Error loving track - {ex.Message}");
        }
        catch (HttpRequestException ex)
        {
            _loggingService.Error($"HTTP Exception when loving track - {ex.StatusCode}: {ex.Message}");
        }
    }

    private static void OpenArt(string uri)
    {
        if (uri != RPCImage.TransparentImage)
        {
            URIOpen.OpenURI(uri);
        }
    }
    
    private async Task StartCountDown()
    {
        IsCoolDown = true;

        try
        {
            using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromSeconds(3));
            while (await timer.WaitForNextTickAsync())
            {
                IsCoolDown = false;
                timer.Dispose();
            }
        }
        catch (Exception)
        {
            // ignored
        }
    }
}
using System.Collections.ObjectModel;
using System.Reactive;
using LastfmDiscordRPC2.Models;
using LastfmDiscordRPC2.Models.RPC;
using ReactiveUI;

namespace LastfmDiscordRPC2.ViewModels.Controls;

public class PreviewControlViewModel : AbstractControlViewModel
{
    public override string Name => "PreviewControl";

    private bool _isReady;
    private ObservableCollection<RPCButton> _buttons = new ObservableCollection<RPCButton>();
    private string _details = Empty;
    private string _state = Empty;
    private RPCImage _largeImage = new RPCImage();
    private RPCImage _smallImage = new RPCImage();

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

    public ObservableCollection<RPCButton> Buttons
    {
        get => _buttons;
        set => this.RaiseAndSetIfChanged(ref _buttons, value);
    }

    public RPCImage LargeImage
    {
        get => _largeImage;
        private set
        {
            this.RaiseAndSetIfChanged(ref _largeImage, value);
            _largeImage.URL = value.URL.Replace(@"/300x300", "");
            _largeImage.Label = value.Label;
        }
    }

    public RPCImage SmallImage
    {
        get => _smallImage;
        private set
        {
            this.RaiseAndSetIfChanged(ref _smallImage, value);
            _smallImage.URL = value.URL;
            _smallImage.Label = value.Label;
        }
    }

    public bool IsReady
    {
        get => _isReady;
        set => this.RaiseAndSetIfChanged(ref _isReady, value);
    }

    public ReactiveCommand<string, Unit> OpenArtCmd { get; } = ReactiveCommand.Create<string>(OpenArt);

    private static void OpenArt(string uri)
    {
        if (uri != RPCImage.TransparentImage)
        {
            Utilities.URIOpen.OpenURI(uri);
        }
    }

    public void ClearAll()
    {
        Buttons = new ObservableCollection<RPCButton>();
        Details = Empty;
        State = Empty;
        LargeImage = new RPCImage();
        SmallImage = new RPCImage();
    }
}
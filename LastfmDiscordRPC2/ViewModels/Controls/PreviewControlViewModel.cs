using System.Collections.ObjectModel;
using System.Reactive;
using ReactiveUI;

namespace LastfmDiscordRPC2.ViewModels.Controls;

public class PreviewControlViewModel : AbstractControlViewModel
{
    public override string Name => "PreviewControl";

    private bool _isReady;
    private ObservableCollection<PreviewButton> _buttons = new ObservableCollection<PreviewButton>();
    private string _details = Empty;
    private string _state = Empty;
    private PreviewImage _largeImage = new PreviewImage();
    private PreviewImage _smallImage = new PreviewImage();

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

    public ObservableCollection<PreviewButton> Buttons
    {
        get => _buttons;
        set => this.RaiseAndSetIfChanged(ref _buttons, value);
    }

    public PreviewImage LargeImage
    {
        get => _largeImage;
        private set
        {
            this.RaiseAndSetIfChanged(ref _largeImage, value);
            _largeImage.URL = value.URL.Replace(@"/300x300", "");
            _largeImage.Label = value.Label;
        }
    }

    public PreviewImage SmallImage
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
        if (uri != PreviewImage.TransparentImage)
        {
            Utilities.URIOpen.OpenURI(uri);
        }
    }

    public void ClearAll()
    {
        Buttons = new ObservableCollection<PreviewButton>();
        Details = Empty;
        State = Empty;
        LargeImage = new PreviewImage();
        SmallImage = new PreviewImage();
    }
}
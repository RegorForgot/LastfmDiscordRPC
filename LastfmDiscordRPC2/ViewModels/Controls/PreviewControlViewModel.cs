using System.Collections.ObjectModel;
using ReactiveUI;

namespace LastfmDiscordRPC2.ViewModels.Controls;

public class PreviewControlViewModel : AbstractControlViewModel
{
    public override string Name => "PreviewControl";

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
            _largeImage.URL = value.URL;
            _largeImage.Text = value.Text;
        }
    }

    public PreviewImage SmallImage
    {
        get => _smallImage;
        private set
        {
            this.RaiseAndSetIfChanged(ref _smallImage, value);
            _smallImage.URL = value.URL;
            _smallImage.Text = value.Text;
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

    public class PreviewObject : ReactiveObject
    {
        private string _url = Empty;
        private string _text = Empty;

        public string URL
        {
            get => _url;
            set => this.RaiseAndSetIfChanged(ref _url, value);
        }

        public string Text
        {
            get => _text;
            set => this.RaiseAndSetIfChanged(ref _text, value);
        }
    }

    public sealed class PreviewImage : PreviewObject
    {
        private const string TransparentImage = "https://i.imgur.com/qFmcbT0.png";
        
        private string _url = TransparentImage;
        public new string URL
        {
            get => _url;
            set => this.RaiseAndSetIfChanged(ref _url, value);
        }
    }
    public sealed class PreviewButton : PreviewObject {}
}
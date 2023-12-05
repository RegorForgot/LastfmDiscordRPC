using ReactiveUI;

namespace LastfmDiscordRPC2.ViewModels;

public abstract class PreviewObject : ReactiveObject
{
    private string _url = Empty;
    private string _label = Empty;

    public string URL
    {
        get => _url;
        set => this.RaiseAndSetIfChanged(ref _url, value);
    }

    public string Label
    {
        get => _label;
        set => this.RaiseAndSetIfChanged(ref _label, value);
    }
}

public sealed class PreviewImage : PreviewObject
{
    internal const string TransparentImage = "https://i.imgur.com/qFmcbT0.png";

    private string _url = TransparentImage;
    public new string URL
    {
        get => _url;
        set => this.RaiseAndSetIfChanged(ref _url, value.Replace(@"/300x300", ""));
    }
}

public sealed class PreviewButton : PreviewObject { }
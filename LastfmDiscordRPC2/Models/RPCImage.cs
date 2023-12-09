using ReactiveUI;

namespace LastfmDiscordRPC2.Models;

public record RPCImage : ReactiveRecord
{
    internal const string TransparentImage = "https://i.imgur.com/qFmcbT0.png";

    private string _url = TransparentImage;
    public string URL
    {
        get => _url;
        set => this.RaiseAndSetIfChanged(ref _url, value.Replace(@"/300x300", ""));
    }
    
    private string _label = Empty;
    public string Label
    {
        get => _label;
        set => this.RaiseAndSetIfChanged(ref _label, value);
    }
}
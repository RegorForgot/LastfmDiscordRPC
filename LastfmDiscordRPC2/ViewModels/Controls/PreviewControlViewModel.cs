using System.Collections.ObjectModel;
using System.Reactive;
using LastfmDiscordRPC2.Models;
using LastfmDiscordRPC2.Utilities;
using ReactiveUI;

namespace LastfmDiscordRPC2.ViewModels.Controls;

public class PreviewControlViewModel : AbstractControlViewModel
{
    public override string Name => "PreviewControl";

    private bool _isVisible;
    private ObservableCollection<RPCButton> _buttons = new ObservableCollection<RPCButton>();
    private string _details = Empty;
    private string _state = Empty;

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

    public RPCImage LargeImage { get; } = new RPCImage();
    public RPCImage SmallImage { get; } = new RPCImage();

    public bool IsVisible
    {
        get => _isVisible;
        set => this.RaiseAndSetIfChanged(ref _isVisible, value);
    }

    public ReactiveCommand<string, Unit> OpenArtCmd { get; } = ReactiveCommand.Create<string>(OpenArt);

    private static void OpenArt(string uri)
    {
        if (uri != RPCImage.TransparentImage)
        {
            URIOpen.OpenURI(uri);
        }
    }
}
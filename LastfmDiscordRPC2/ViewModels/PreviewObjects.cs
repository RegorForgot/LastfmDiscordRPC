using System;
using LastfmDiscordRPC2.Utilities;
using ReactiveUI;

namespace LastfmDiscordRPC2.ViewModels;

public abstract class PreviewObject : ReactiveObject
{
    public virtual string URL { get; set; } = Empty;

    private string _label = Empty;
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
    public override string URL
    {
        get => _url;
        set => this.RaiseAndSetIfChanged(ref _url, value.Replace(@"/300x300", ""));
    }
}

public sealed class PreviewButton : PreviewObject
{
    private string _url = Empty;
    private bool _isInvalidUrl;
    private readonly Action? _action;

    public override string URL
    {
        get => _url;
        set
        {
            if (_action is null)
            {
                this.RaiseAndSetIfChanged(ref _url, value);
                return;
            }
            
            bool validUrl = DiscordClientExtensions.ValidatePlaceholderLink(value);
            if (validUrl)
            {
                this.RaiseAndSetIfChanged(ref _url, value);
            }
            IsInvalidUrl = !validUrl;
        }
    }

    public bool IsInvalidUrl
    {
        get => _isInvalidUrl;
        private set
        {
            if (_isInvalidUrl == value)
            {
                return;
            }
            this.RaiseAndSetIfChanged(ref _isInvalidUrl, value);
            _action?.Invoke();
        }
    }

    public PreviewButton()
    {
        _action = null;
    }
    
    public PreviewButton(Action? action)
    {
        _action = action;
    }
}
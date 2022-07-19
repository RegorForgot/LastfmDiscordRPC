using LastfmDiscordRPC.Models;

namespace LastfmDiscordRPC.ViewModels;

public sealed class PreviewViewModel : ViewModelBase
{
    private string _imageHyperlink;
    public string ImageHyperlink
    {
        get => _imageHyperlink;
        set
        {
            if (value == _imageHyperlink)
            {
                return;
            }
            _imageHyperlink = value.Replace(@"/300x300", "");
            OnPropertyChanged(nameof(ImageHyperlink));
        }
    }
    
    private string _imageURL;
    public string ImageURL
    {
        get => _imageURL;
        set
        {
            if (value == _imageURL)
            {
                return;
            }
            _imageURL = value;
            ImageHyperlink = value is Track.DefaultAlbumCover or Track.DefaultSingleCover ? Empty : _imageURL;
            OnPropertyChanged(nameof(ImageURL));
        }
    }

    private string _tooltip;
    public string Tooltip
    {
        get => _tooltip;
        set
        {
            if (value == _tooltip)
            {
                return;
            }
            _tooltip = value;
            OnPropertyChanged(nameof(Tooltip));
        }
    }

    private string _description;
    public string Description
    {
        get => _description;
        set
        {
            if (value == _description)
            {
                return;
            }
            _description = value;
            OnPropertyChanged(nameof(Description));
        }
    }

    private string _state;
    public string State
    {
        get => _state;
        set
        {
            if (value == _state)
            {
                return;
            }
            _state = value;
            OnPropertyChanged(nameof(State));
        }
    }
    
    public PreviewViewModel()
    {
        _imageURL = Empty;
        _tooltip = Empty;
        _description = Empty;
        _state = Empty;
    }
}
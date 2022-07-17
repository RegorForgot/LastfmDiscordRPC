using static System.String;

namespace LastfmDiscordRPC.ViewModels;

public sealed class PreviewViewModel : ViewModelBase
{
    private string _imageURL;
    public string ImageURL
    {
        get => _imageURL;
        set
        {
            if (value == _imageURL) return;
            _imageURL = value.Replace(@"/300x300", "");
            OnPropertyChanged(nameof(ImageURL));
        }
    }

    private string _name;
    public string Name
    {
        get => _name;
        set
        {
            if (value == _name) return;
            _name = value;
            OnPropertyChanged(nameof(Name));
        }
    }

    private string _artistName;
    public string ArtistName
    {
        get => _artistName;
        set
        {
            if (value == _artistName) return;
            _artistName = value;
            OnPropertyChanged(nameof(ArtistName));
        }
    }

    private string _albumName;
    public string AlbumName
    {
        get => _albumName;
        set
        {
            if (value == _albumName) return;
            _albumName = value;
            OnPropertyChanged(nameof(AlbumName));
        }
    }

    public PreviewViewModel()
    {
        _imageURL = Empty;
        _name = Empty;
        _artistName = Empty;
        _albumName = Empty;
    }
}
using static System.String;

namespace LastfmDiscordRPC.MVVM.ViewModels;

public sealed class PreviewViewModel : ViewModelBase
{
    private string _imageURL;
    public string ImageURL
    {
        get => _imageURL;
        set
        {
            _imageURL = value;
            HighResURL = _imageURL.Replace(@"/300x300", "");
            OnPropertyChanged(nameof(ImageURL));
        }
    }
    
    private string _highResURL;
    public string HighResURL
    {
        get => _highResURL;
        set
        {
            _highResURL = value;
            OnPropertyChanged(nameof(HighResURL));
        }
    }

    private string _name;
    public string Name
    {
        get => _name;
        set
        {
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
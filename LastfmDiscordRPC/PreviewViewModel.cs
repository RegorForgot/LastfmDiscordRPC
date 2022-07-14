using System.ComponentModel;
using System.Runtime.CompilerServices;
using static System.String;

namespace LastfmDiscordRPC;

public class PreviewViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    [NotifyPropertyChangedInvocator]
    virtual protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private string _imageURL;
    public string ImageURL
    {
        get => _imageURL;
        set
        {
            _imageURL = value;
            OnPropertyChanged(nameof(ImageURL));
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
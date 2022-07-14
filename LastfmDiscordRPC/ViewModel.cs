using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using LastfmDiscordRPC.Commands;

namespace LastfmDiscordRPC;

public sealed class AppViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    [NotifyPropertyChangedInvocator]
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    private string _username;
    public string Username
    {
        get => _username;
        set
        {
            _username = value;
            OnPropertyChanged(nameof(Username));
        }
    }

    private string _apiKey;
    public string APIKey
    {
        get => _apiKey;
        set
        {
            _apiKey = value;
            OnPropertyChanged(nameof(APIKey));
        }
    }

    public ICommand ActivateCommand { get; private set; }
    public ICommand SaveCommand { get; private set; }
    
    public AppViewModel(string username, string apiKey)
    {
        _username = username;
        _apiKey = apiKey;
        ActivateCommand = new ActivateCommand(this);
        SaveCommand = new SaveCommand(this);
    }
}
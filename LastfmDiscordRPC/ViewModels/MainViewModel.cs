using System.Windows.Input;
using LastfmDiscordRPC.Commands;
using LastfmDiscordRPC.Models;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace LastfmDiscordRPC.ViewModels;

public sealed class MainViewModel : ViewModelBase
{
    private string _username;
    public string Username
    {
        get => _username;
        set
        {
            if (value == _username) return;
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
            if (value == _apiKey) return;
            _apiKey = value;
            OnPropertyChanged(nameof(APIKey));
        }
    }
    
    private string _appKey;
    public string AppKey
    {
        get => _appKey;
        set
        {
            if (value == _appKey) return;
            _appKey = value;
            OnPropertyChanged(nameof(AppKey));
        }
    }

    private string _outputText;
    public string OutputText
    {
        get => _outputText;
        set
        {
            if (value == OutputText) return;
            _outputText = value;
            OnPropertyChanged(nameof(OutputText));
        }
    }
    
    public ICommand ActivateCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand DefaultKeyCommand { get; }
    
    public PreviewViewModel PreviewViewModel { get; }
    public readonly DiscordClient Client;
    
    public MainViewModel(string username, string apiKey, string appKey, DiscordClient client)
    {
        _username = username;
        _apiKey = apiKey;
        _appKey = appKey;
        Client = client;
        _outputText = "+ Started!";
        ActivateCommand = new ActivateCommand(this);
        SaveCommand = new SaveCommand(this);
        DefaultKeyCommand = new DefaultKeyCommand(this);
        PreviewViewModel = new PreviewViewModel();
    }
}
using System.Windows;
using System.Windows.Input;
using LastfmDiscordRPC.Commands;
using LastfmDiscordRPC.Models;
using static System.String;

namespace LastfmDiscordRPC.ViewModels;

public class MainViewModel : ViewModelBase
{
    private string _username = null!;
    public string Username
    {
        get => _username;
        set
        {
            if (value == _username) return;
            _username = value;
            UsernamePlaceholder = IsNullOrEmpty(_username) ? Visibility.Visible : Visibility.Hidden;
            OnPropertyChanged(nameof(Username)); 
        }
    }

    private string _apiKey = null!;
    public string APIKey
    {
        get => _apiKey;
        set
        {
            if (value == _apiKey) return;
            _apiKey = value;
            APIKeyPlaceholder = IsNullOrEmpty(_apiKey) ? Visibility.Visible : Visibility.Hidden;
            OnPropertyChanged(nameof(APIKey));
        }
    }
    
    private string _appKey = null!;
    public string AppKey
    {
        get => _appKey;
        set
        {
            if (value == _appKey) return;
            _appKey = value;
            AppKeyPlaceholder = IsNullOrEmpty(_appKey) ? Visibility.Visible : Visibility.Hidden;
            OnPropertyChanged(nameof(AppKey));
        }
    }

    private string _outputText = null!;
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

    private Visibility _usernamePlaceholder;
    public Visibility UsernamePlaceholder
    {
        get => _usernamePlaceholder;
        set
        {
            if (value == _usernamePlaceholder) return;
            _usernamePlaceholder = value;
            OnPropertyChanged(nameof(UsernamePlaceholder));
        }
    }

    private Visibility _apiKeyPlaceholder;
    public Visibility APIKeyPlaceholder
    {
        get => _apiKeyPlaceholder;
        set
        {
            if (value == _apiKeyPlaceholder) return;
            _apiKeyPlaceholder = value;
            OnPropertyChanged(nameof(APIKeyPlaceholder));
        }
    }

    private Visibility _appKeyPlaceholder;
    public Visibility AppKeyPlaceholder
    {
        get => _appKeyPlaceholder;
        set
        {
            if (value == _appKeyPlaceholder) return;
            _appKeyPlaceholder = value;
            OnPropertyChanged(nameof(AppKeyPlaceholder));
        }
    }
    
    public ICommand ActivateCommand { get; }
    public ICommand DeactivateCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand DefaultKeyCommand { get; }
    
    public PreviewViewModel PreviewViewModel { get; }
    public readonly DiscordClient Client;
    
    public MainViewModel(string username, string apiKey, string appKey, DiscordClient client)
    {
        Username = username;
        APIKey = apiKey;
        AppKey = appKey;
        Client = client;
        OutputText = "+ Started!";
        ActivateCommand = new ActivateCommand(this);
        DeactivateCommand = new DeactivateCommand(this);
        SaveCommand = new SaveCommand(this);
        DefaultKeyCommand = new DefaultKeyCommand(this);
        PreviewViewModel = new PreviewViewModel();
    }
}
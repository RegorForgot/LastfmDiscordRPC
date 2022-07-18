using System;
using System.Collections;
using System.Windows.Input;
using LastfmDiscordRPC.Commands;
using LastfmDiscordRPC.Models;

namespace LastfmDiscordRPC.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private string _username = null!;
    public string Username
    {
        get => _username;
        set
        {
            if (value == _username) return;
            _username = value;
            ValidateUsername();
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
            ValidateAPIKey();
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
            ValidateAppKey();
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
    
    public void WriteToOutput(string text) => OutputText += text;

    public ICommand SetPresenceCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand DefaultKeyCommand { get; }

    public PreviewViewModel PreviewViewModel { get; }
    public readonly DiscordClient Client;
    
    public MainViewModel(string username, string apiKey, string appKey, DiscordClient client)
    {
        SetPresenceCommand = new SetPresenceCommand(this);
        SaveCommand = new SaveCommand(this);
        DefaultKeyCommand = new DefaultKeyCommand(this);
        PreviewViewModel = new PreviewViewModel();
        Username = username;
        APIKey = apiKey;
        AppKey = appKey;
        Client = client;
        OutputText = "+ Started!\n";
    }
}
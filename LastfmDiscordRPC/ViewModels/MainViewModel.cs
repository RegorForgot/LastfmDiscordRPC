using System;
using System.Windows.Input;
using LastfmDiscordRPC.Commands;
using LastfmDiscordRPC.Models;
using static LastfmDiscordRPC.Models.SaveAppData;

namespace LastfmDiscordRPC.ViewModels;

public partial class MainViewModel : ViewModelBase, IDisposable
{
    private string _username;
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

    private string _apiKey;
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

    private string _appKey;
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

    public void WriteToOutput(string text)
    {
        OutputText += text;
    }

    public ICommand SetPresenceCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand DefaultKeyCommand { get; }
    public PreviewViewModel PreviewViewModel { get; }
    
    public readonly DiscordClient DiscordClient;
    public readonly LastfmClient LastfmClient;
    public readonly PresenceSetter PresenceSetter;
    
    public MainViewModel(AppData appData)
    {
        OutputText = "+ Started!\n";
        Username = appData.Username;
        APIKey = appData.APIKey;
        AppKey = appData.AppKey;
        SetPresenceCommand = new SetPresenceCommand(this);
        SaveCommand = new SaveCommand(this);
        DefaultKeyCommand = new DefaultKeyCommand(this);
        PreviewViewModel = new PreviewViewModel();
        DiscordClient = new DiscordClient(SavedData.AppKey);
        LastfmClient = new LastfmClient();
        PresenceSetter = new PresenceSetter(this);
    }

    public void Dispose()
    {
        PresenceSetter.Dispose();
        LastfmClient.Dispose();
        DiscordClient.Dispose();
        SuppressFinalize(this);
    }
}
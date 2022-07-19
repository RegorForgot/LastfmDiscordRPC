using System;
using System.Windows.Input;
using DiscordRPC.Logging;
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
            if (value == _username)
            {
                return;
            }
            _username = value;
            ValidateUsername();
            OnPropertyChanged(nameof(Username));
            ((CommandBase)SetPresenceCommand).RaiseCanExecuteChanged();
        }
    }

    private string _apiKey;

    public string APIKey
    {
        get => _apiKey;
        set
        {
            if (value == _apiKey)
            {
                return;
            }
            _apiKey = value;
            ValidateAPIKey();
            OnPropertyChanged(nameof(APIKey));
        }
    }

    private string _appID;

    public string AppID
    {
        get => _appID;
        set
        {
            if (value == _appID)
            {
                return;
            }
            _appID = value;
            ValidateAppID();
            OnPropertyChanged(nameof(AppID));
        }
    }

    private string _outputText = "";

    public string OutputText
    {
        get => _outputText;
        set
        {
            _outputText += value;
            OnPropertyChanged(nameof(OutputText));
        }
    }

    private bool _hasNotRun;

    public bool HasNotRun
    {
        get => _hasNotRun;
        set
        {
            if (value == _hasNotRun) return;
            _hasNotRun = value;
            OnPropertyChanged(nameof(HasNotRun));
        }
    }
    
    public ICommand SetPresenceCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand DefaultCommand { get; }
    public PreviewViewModel PreviewViewModel { get; }
    public DiscordClient DiscordClient { get; }
    public LastfmClient LastfmClient { get; }
    public PresenceSetter PresenceSetter { get; }
    public Logger Logger { get; }

    public MainViewModel(AppData appData)
    {
        HasNotRun = true;
        OutputText = "+ Started!";
        
        Logger = new Logger($@"{FolderPath}\errLog.log", LogLevel.Warning, this);
        LastfmClient = new LastfmClient();
        DiscordClient = new DiscordClient(this);
        PresenceSetter = new PresenceSetter(this);
        
        SetPresenceCommand = new SetPresenceCommand(this);
        SaveCommand = new SaveCommand(this);
        DefaultCommand = new DefaultCommand(this);
        
        PreviewViewModel = new PreviewViewModel();
        
        Username = appData.Username;
        APIKey = appData.APIKey;
        AppID = appData.AppID;
    }

    public void Dispose()
    {
        PresenceSetter.Dispose();
        LastfmClient.Dispose();
        DiscordClient.Dispose();
        SuppressFinalize(this);
    }
}
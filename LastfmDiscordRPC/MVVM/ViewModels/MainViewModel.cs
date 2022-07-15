using System.Windows.Input;
using LastfmDiscordRPC.MVVM.Commands;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace LastfmDiscordRPC.MVVM.ViewModels;

public sealed class MainViewModel : ViewModelBase
{
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

    private string _outputText;
    public string OutputText
    {
        get => _outputText;
        set
        {
            _outputText = value;
            OnPropertyChanged(nameof(OutputText));
        }
    }
    
    public ICommand ActivateCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand DefaultKeyCommand { get; }
    
    public PreviewViewModel PreviewViewModel { get; }
    
    public MainViewModel(string username, string apiKey)
    {
        _username = username;
        _apiKey = apiKey;
        _outputText = " [+] Started!";
        ActivateCommand = new ActivateCommand(this);
        SaveCommand = new SaveCommand(this);
        DefaultKeyCommand = new DefaultKeyCommand(this);
        PreviewViewModel = new PreviewViewModel();
    }
}
using System;
using System.ComponentModel.DataAnnotations;
using System.Reactive;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LastfmDiscordRPC2.Models;
using ReactiveUI;
using static LastfmDiscordRPC2.Models.Utilities.SaveAppData;

namespace LastfmDiscordRPC2.ViewModels;

public class SettingsViewModel : ViewModelBase
{
    public ReactiveCommand<bool, Unit> LaunchOnStartup { get; }
    public ReactiveCommand<bool, Unit> LastfmLogin { get; }
    public ReactiveCommand<Unit, Unit> SaveAppID { get; }
    public bool StartUpVisible { get; set; }

    private bool _saveEnabled;
    private bool _startUpChecked;
    private bool _isInProgress;
    private string _loginMessage;
    private string _appID;

    public bool SaveEnabled
    {
        get => _saveEnabled;
        set => this.RaiseAndSetIfChanged(ref _saveEnabled, value);
    }

    public bool StartUpChecked
    {
        get => _startUpChecked;
        set => this.RaiseAndSetIfChanged(ref _startUpChecked, value);
    }

    public bool IsInProgress
    {
        get => _isInProgress;
        set => this.RaiseAndSetIfChanged(ref _isInProgress, value);
    }

    public string LoginMessage
    {
        get => _loginMessage;
        set => this.RaiseAndSetIfChanged(ref _loginMessage, value);
    }

    public bool IsLoggedIn
    {
        get => SavedData.UserAccount.SessionKey == "" || SavedData.UserAccount.Username == "";
    }

    private readonly MainViewModel _mainViewModel;


    [Required(ErrorMessage = "Please input a Discord Application ID.")]
    [RegularExpression(@"^\d{17,21}$", ErrorMessage = "Please ensure you are using a valid ID.")]
    public string AppID
    {
        get => _appID;
        set => this.RaiseAndSetIfChanged(ref _appID, value);
    }

    public SettingsViewModel(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;

        LaunchOnStartup = ReactiveCommand.Create<bool>(SetLaunchOnStartup);
        LastfmLogin = ReactiveCommand.CreateFromTask<bool>(SetLastfmLogin);
        SaveAppID = ReactiveCommand.Create(SaveDiscordAppID);
        SaveEnabled = true;

        if (Utilities.OS == OSPlatform.Windows)
        {
            StartUpVisible = true;
            StartUpChecked = Utilities.CheckRegistryExists();
        }

        AppID = SavedData.AppID;
        LoginMessage = IsLoggedIn ? "Not logged in" : $"Logged in as {SavedData.UserAccount.Username}";
    }

    private void SaveDiscordAppID()
    {
        AppData data = new AppData(SavedData)
        {
            AppID = AppID
        };
        SaveData(data);
    }

    private void SetLaunchOnStartup(bool startUpValue)
    {
        Utilities.SetRegistry(startUpValue);
    }

    private async Task SetLastfmLogin(bool logIn)
    {
        IsInProgress = true;
        if (logIn)
        {
            await LogUserIn();
        }
        else
        {
            LogUserOut();
        }

        this.RaisePropertyChanged(nameof(IsLoggedIn));
        IsInProgress = false;
    }

    private void LogUserOut()
    {
        AppData data = new AppData(SavedData)
        {
            UserAccount = new AppData.Account()
        };
        SaveData(data);

        LoginMessage = "Not logged in";
    }

    private async Task LogUserIn()
    {
        try
        {
            TokenResponse token = await _mainViewModel.LastfmClient.GetToken();
            Utilities.OpenWebpage(@$"https://www.last.fm/api/auth/?api_key={Utilities.APIKey}&token={token.Token}");
            
            SessionResponse? sessionResponse = await _mainViewModel.LastfmClient.GetSession(token.Token);

            AppData data = new AppData(SavedData)
            {
                UserAccount = new AppData.Account
                {
                    SessionKey = sessionResponse.LfmSession.SessionKey,
                    Username = sessionResponse.LfmSession.Username
                }
            };
            SaveData(data);

            LoginMessage = $"Logged in as {SavedData.UserAccount.Username}";
        }
        catch (Exception e)
        {
            LoginMessage = e.Message;
        }
    }
}
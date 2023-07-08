using System;
using System.Diagnostics;
using System.Reactive;
using System.Threading.Tasks;
using LastfmDiscordRPC2.Models;
using ReactiveUI;
using static LastfmDiscordRPC2.Models.Utilities.SaveAppData;

namespace LastfmDiscordRPC2.ViewModels;

public class SettingsViewModel : ViewModelBase
{
    private bool _startUpVisible;
    private bool _startUpChecked;
    private bool _isInProgress;
    private string _loginMessage;
    public ReactiveCommand<bool, Unit> LaunchOnStartup { get; }
    public ReactiveCommand<bool, Unit> LastfmLogin { get; }

    private readonly MainViewModel _mainViewModel;
    public bool IsLogin => SavedData.UserAccount.SessionKey == "" || SavedData.UserAccount.Username == "";


    public bool StartUpVisible
    {
        get => _startUpVisible;
        set => this.RaiseAndSetIfChanged(ref _startUpVisible, value);
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

    public SettingsViewModel(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
        LaunchOnStartup = ReactiveCommand.Create<bool>(SetLaunchOnStartup);
        LastfmLogin = ReactiveCommand.CreateFromTask<bool>(SetLastfmLogin);
        if (OperatingSystem.IsWindows())
        {
            StartUpVisible = true;
            StartUpChecked = Utilities.CheckRegistryExists();
        }

        LoginMessage = IsLogin ? "Not logged in" : $"Logged in as {SavedData.UserAccount.Username}";
    }

    private void SetLaunchOnStartup(bool parameter)
    {
        Utilities.SetRegistry(parameter);
    }

    private async Task SetLastfmLogin(bool parameter)
    {
        IsInProgress = true;
        if (parameter)
        {
            await LogUserIn();
        }
        else
        {
            LogUserOut();
        }
        
        this.RaisePropertyChanged(nameof(IsLogin));
        IsInProgress = false;
    }

    private void LogUserOut()
    {
        SavedData.UserAccount.SessionKey = Empty;
        SavedData.UserAccount.Username = Empty;
        LoginMessage = "Not logged in";
        SaveData();
    }

    private async Task LogUserIn()
    {
        TokenResponse? token = null;
        try
        {
            token = await _mainViewModel.LastfmClient.GetToken();
        }
        catch (Exception e)
        {
            LoginMessage = e.Message;
        }

        if (token?.Token != null)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = @$"https://www.last.fm/api/auth/?api_key={Utilities.APIKey}&token={token.Token}",
                UseShellExecute = true
            };
            Process.Start(psi);

            try
            {
                SessionResponse? sessionResponse = await _mainViewModel.LastfmClient.GetSession(token.Token);

                SavedData.UserAccount.SessionKey = sessionResponse.LfmSession.SessionKey;
                SavedData.UserAccount.Username = sessionResponse.LfmSession.Username;
                SaveData();
                
                LoginMessage = $"Logged in as {SavedData.UserAccount.Username}";
            }
            catch (Exception e)
            {
                LoginMessage = e.Message;
            }
        }
    }
}
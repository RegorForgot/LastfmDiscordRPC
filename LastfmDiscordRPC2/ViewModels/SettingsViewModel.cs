using System;
using System.Diagnostics;
using System.Reactive;
using System.Threading;
using LastfmDiscordRPC2.Models;
using ReactiveUI;
using static LastfmDiscordRPC2.Models.Utilities.SaveAppData;

namespace LastfmDiscordRPC2.ViewModels;

public class SettingsViewModel : ViewModelBase
{
    private bool _startUpVisible;
    private bool _startUpChecked;
    private bool _isInProgress;
    public ReactiveCommand<bool, Unit> LaunchOnStartup { get; }
    public ReactiveCommand<bool, Unit> LastfmLogin { get; }
    
    private readonly MainViewModel _mainViewModel;
    public bool IsLogin => SavedData.UserAccount.SessionKey == "" || SavedData.UserAccount.Username == "";
    public string UsernameLogin => "Logged in as " + $"{SavedData.UserAccount.Username}";

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
    
    public SettingsViewModel(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
        LaunchOnStartup = ReactiveCommand.Create<bool>(SetLaunchOnStartup);
        LastfmLogin = ReactiveCommand.Create<bool>(SetLastfmLogin);
        if (OperatingSystem.IsWindows())
        {
            StartUpVisible = true;
            StartUpChecked = Utilities.CheckRegistryExists();
        }
    }
    
    private void SetLaunchOnStartup(bool parameter)
    {
        Utilities.SetRegistry(parameter);
    }

    private async void SetLastfmLogin(bool parameter)
    {
        IsInProgress = true;
        if (parameter)
        {
            TokenResponse token = await _mainViewModel.LastfmClient.GetToken();
            if (token.Token == null) return;
            
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = @$"https://www.last.fm/api/auth/?api_key={Utilities.APIKey}&token={token.Token}",
                UseShellExecute = true
            };
            Process.Start(psi);
            
            SessionResponse? sessionResponse = null;
            PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromSeconds(5));
            do
            {
                try
                {
                    while (await timer.WaitForNextTickAsync())
                    {
                        sessionResponse = await _mainViewModel.LastfmClient.GetSession(token.Token);
                    }
                    timer.Dispose();
                }
                catch (LastfmException e)
                {
                    if (e.ErrorCode != LastfmException.ErrorEnum.UnauthorizedToken)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            } while (sessionResponse == null);
            
            SavedData.UserAccount.SessionKey = sessionResponse.LfmSession.SessionKey;
            SavedData.UserAccount.Username = sessionResponse.LfmSession.Username;
            SaveData();
        }
        else
        {
            SavedData.UserAccount.SessionKey = Empty;
            SavedData.UserAccount.Username = Empty;
            SaveData();
        }
        this.RaisePropertyChanged(nameof(UsernameLogin));
        this.RaisePropertyChanged(nameof(IsLogin));
        IsInProgress = false;
    }
}
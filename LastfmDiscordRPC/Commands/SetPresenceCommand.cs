using System;
using System.Threading;
using LastfmDiscordRPC.Models;
using LastfmDiscordRPC.ViewModels;

namespace LastfmDiscordRPC.Commands;

public class SetPresenceCommand : CommandBase
{
    private readonly PresenceSetter _presenceSetter;
    private string _previousUsername;
    private string _previousAPIKey;

    public SetPresenceCommand(MainViewModel mainViewModel) : base(mainViewModel)
    {
        _presenceSetter = mainViewModel.PresenceSetter;
        _previousUsername = Empty;
        _previousAPIKey = Empty;
    }
    
    public override bool CanExecute(object? parameter)
    {
        return _presenceSetter.IsReady 
               && !MainViewModel.HasErrors 
               && (_previousUsername != MainViewModel.Username 
                   || _previousAPIKey != MainViewModel.APIKey)
               || MainViewModel.PresenceSetter.RetryAllowed;
    }

    public override void Execute(object? parameter)
    {
        _previousUsername = MainViewModel.Username;
        _previousAPIKey = MainViewModel.APIKey;
        MainViewModel.PresenceSetter.RetryAllowed = false;
        UpdateButtonExecute();
        _presenceSetter.UpdatePresence(_previousUsername, _previousAPIKey);
    }

    private async void UpdateButtonExecute()
    {
        using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromSeconds(3));
        _presenceSetter.Dispose();
        _presenceSetter.IsReady = false;
        RaiseCanExecuteChanged();

        while (await timer.WaitForNextTickAsync())
        {
            _presenceSetter.IsReady = true;
            RaiseCanExecuteChanged();
            timer.Dispose();
        }
    }
}
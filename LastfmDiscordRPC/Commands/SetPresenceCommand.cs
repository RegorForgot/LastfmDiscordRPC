using System;
using System.Threading;
using LastfmDiscordRPC.Models;
using LastfmDiscordRPC.ViewModels;

namespace LastfmDiscordRPC.Commands;

public class SetPresenceCommand : CommandBase
{
    private readonly PresenceSetter _presenceSetter;

    public SetPresenceCommand(MainViewModel mainViewModel) : base(mainViewModel)
    {
        _presenceSetter = mainViewModel.PresenceSetter;
    }
    
    public override bool CanExecute(object? parameter)
    {
        return _presenceSetter.IsReady && !MainViewModel.HasErrors;
    }

    public override void Execute(object? parameter)
    {
        UpdateButtonExecute();
        _presenceSetter.UpdatePresence(MainViewModel.Username, MainViewModel.APIKey);
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
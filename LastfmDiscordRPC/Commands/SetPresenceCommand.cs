using System;
using System.Threading;
using LastfmDiscordRPC.Models;
using LastfmDiscordRPC.ViewModels;

namespace LastfmDiscordRPC.Commands;

public class SetPresenceCommand : CommandBase
{
    public SetPresenceCommand(MainViewModel mainViewModel) : base(mainViewModel)
    { }
    
    public override bool CanExecute(object? parameter) => SetPresence.IsReady && !MainViewModel.HasErrors;

    public override void Execute(object? parameter)
    {
        string username = MainViewModel.Username;
        string apiKey = MainViewModel.APIKey;
        UpdateButtonExecute();
        SetPresence.UpdatePresence(username, apiKey, MainViewModel);
    }

    private async void UpdateButtonExecute()
    {
        using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromSeconds(3));
        SetPresence.Dispose();
        SetPresence.IsReady = false;
        RaiseCanExecuteChanged();

        while (await timer.WaitForNextTickAsync())
        {
            SetPresence.IsReady = true;
            RaiseCanExecuteChanged();
            timer.Dispose();
        }
    }
}
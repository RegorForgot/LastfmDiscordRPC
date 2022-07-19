using System;
using System.Threading;
using LastfmDiscordRPC.ViewModels;

namespace LastfmDiscordRPC.Commands;

public class SetPresenceCommand : CommandBase
{
    private bool CanSetPresence { get; set; } = true;
    
    public SetPresenceCommand(MainViewModel mainViewModel) : base(mainViewModel)
    { }
    
    public override bool CanExecute(object? parameter)
    {
        return CanSetPresence && !MainViewModel.HasErrors;
    }

    public override void Execute(object? parameter)
    {
        string username = MainViewModel.Username;
        string apiKey = MainViewModel.APIKey;
        
        UpdateButtonExecute();
        MainViewModel.PresenceSetter.UpdatePresence(username, apiKey);
    }

    // Dispose PresenceSetter's timer - event handler (in this case for the button click) - hence why it is async void and not
    // async Task.
    private async void UpdateButtonExecute()
    {
        using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromSeconds(5));
        MainViewModel.PresenceSetter.Dispose();
        CanSetPresence = false;
        RaiseCanExecuteChanged();

        while (await timer.WaitForNextTickAsync())
        {
            CanSetPresence = true;
            RaiseCanExecuteChanged();
            timer.Dispose();
        }
    }
}
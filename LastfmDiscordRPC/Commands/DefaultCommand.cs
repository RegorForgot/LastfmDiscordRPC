using LastfmDiscordRPC.Models;
using LastfmDiscordRPC.ViewModels;

namespace LastfmDiscordRPC.Commands;

public class DefaultCommand : CommandBase
{
    public DefaultCommand(MainViewModel mainViewModel) : base(mainViewModel)
    { }

    public override bool CanExecute(object? parameter)
    {
        return true;
    }

    public override void Execute(object? parameter)
    {
        MainViewModel.APIKey = SaveAppData.DefaultAPIKey;
        MainViewModel.AppID = SaveAppData.DefaultAppID;
    }
}
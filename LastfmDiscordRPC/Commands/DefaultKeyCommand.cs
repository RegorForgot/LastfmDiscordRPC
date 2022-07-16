using LastfmDiscordRPC.Models;
using LastfmDiscordRPC.ViewModels;

namespace LastfmDiscordRPC.Commands;

public class DefaultKeyCommand : CommandBase
{
    private readonly MainViewModel _mainViewModel;
    public DefaultKeyCommand(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
    }

    public override void Execute(object? parameter)
    {
        _mainViewModel.APIKey = SaveAppData.DefaultAPIKey;
    }
}
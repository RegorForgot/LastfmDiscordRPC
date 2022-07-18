using LastfmDiscordRPC.Models;
using LastfmDiscordRPC.ViewModels;

namespace LastfmDiscordRPC.Commands;

public class DefaultKeyCommand : CommandBase
{
    public DefaultKeyCommand(MainViewModel mainViewModel) : base(mainViewModel)
    { }

    public override void Execute(object? parameter) => MainViewModel.APIKey = SaveAppData.DefaultAPIKey;
}
namespace LastfmDiscordRPC.Commands;

public class DefaultKeyCommand : CommandBase
{
    private readonly ViewModel _viewModel;
    public DefaultKeyCommand(ViewModel viewModel)
    {
        _viewModel = viewModel;
    }

    public override void Execute(object? parameter)
    {
        _viewModel.APIKey = SaveAppData.DefaultAPIKey;
    }
}
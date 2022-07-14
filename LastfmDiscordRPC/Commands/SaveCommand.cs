namespace LastfmDiscordRPC.Commands;

public class SaveCommand : CommandBase
{
    private readonly ViewModel _viewModel;
    public SaveCommand(ViewModel viewModel)
    {
        _viewModel = viewModel;
    }
    
    public override void Execute(object? parameter)
    {
        SaveAppData.SaveData(_viewModel.Username, _viewModel.APIKey);
    }
}
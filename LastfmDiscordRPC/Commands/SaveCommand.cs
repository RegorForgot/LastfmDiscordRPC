namespace LastfmDiscordRPC.Commands;

public class SaveCommand : CommandBase
{
    private readonly AppViewModel _appViewModel;
    public SaveCommand(AppViewModel appViewModel)
    {
        _appViewModel = appViewModel;
    }
    
    public override void Execute(object? parameter)
    {
        SaveAppData.SaveData(_appViewModel.Username, _appViewModel.APIKey);
        new ActivateCommand(_appViewModel).Execute(null);
    }
}
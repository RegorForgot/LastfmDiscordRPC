using System.Windows;

namespace LastfmDiscordRPC.Commands;

public class ActivateCommand : CommandBase
{
    private readonly AppViewModel _appViewModel;
    public ActivateCommand(AppViewModel appViewModel)
    {
        _appViewModel = appViewModel;
    }
    
    public override void Execute(object? parameter)
    {
        MessageBox.Show(_appViewModel.Username, _appViewModel.APIKey);
    }
}
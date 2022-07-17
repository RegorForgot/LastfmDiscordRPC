using LastfmDiscordRPC.ViewModels;

namespace LastfmDiscordRPC.Commands;

public class DeactivateCommand : CommandBase
{
    private readonly MainViewModel _mainViewModel;

    public DeactivateCommand(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
    }

    public override void Execute(object? parameter)
    {
        ((ActivateCommand) _mainViewModel.ActivateCommand).Dispose();
        RaiseCanExecuteChanged();
    }

    public override bool CanExecute(object? paramater)
    {
        return _mainViewModel.Client.HasPresence();
    }
}
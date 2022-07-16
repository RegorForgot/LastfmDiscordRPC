using LastfmDiscordRPC.MVVM.Models;
using LastfmDiscordRPC.MVVM.ViewModels;

namespace LastfmDiscordRPC.MVVM.Commands;

public class SaveCommand : CommandBase
{
    private readonly MainViewModel _mainViewModel;
    public SaveCommand(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
    }
    
    public override void Execute(object? parameter)
    {
        SaveAppData.SaveData(_mainViewModel.Username, _mainViewModel.APIKey, _mainViewModel.AppKey);
    }
}
using LastfmDiscordRPC.Models;
using LastfmDiscordRPC.ViewModels;

namespace LastfmDiscordRPC.Commands;

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
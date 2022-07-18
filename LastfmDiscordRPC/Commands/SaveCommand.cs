using System.IO;
using LastfmDiscordRPC.Models;
using LastfmDiscordRPC.ViewModels;

namespace LastfmDiscordRPC.Commands;

public class SaveCommand : CommandBase
{
    public SaveCommand(MainViewModel mainViewModel) : base(mainViewModel)
    { }

    public override void Execute(object? parameter)
    {
        try
        {
            SaveAppData.SaveData(MainViewModel.Username, MainViewModel.APIKey, MainViewModel.AppID);
        } catch (IOException e)
        {
            MainViewModel.Logger.ErrorOverride("Error writing to file: {0}", e.Message);
        }
    }

    public override bool CanExecute(object? parameter)
    {
        return !MainViewModel.HasErrors;
    }
}
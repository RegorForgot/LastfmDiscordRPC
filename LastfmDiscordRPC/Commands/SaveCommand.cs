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
            SaveAppData.SaveData(MainViewModel.Username, MainViewModel.APIKey, MainViewModel.AppKey);
        } catch (IOException e)
        {
            MainViewModel.WriteToOutput($"Error saving file: {e.Message}");
        }
    }

    public override bool CanExecute(object? parameter) => !MainViewModel.PropertyHasError(nameof(MainViewModel.AppKey));
}
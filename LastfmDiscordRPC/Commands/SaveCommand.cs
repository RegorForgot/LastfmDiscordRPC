using System.IO;
using LastfmDiscordRPC.Models;
using LastfmDiscordRPC.ViewModels;

namespace LastfmDiscordRPC.Commands;

/// <summary>
/// Command that stores the user-entered info using SaveAppData
/// </summary>
public class SaveCommand : CommandBase
{
    public SaveCommand(MainViewModel mainViewModel) : base(mainViewModel)
    { }

    /// <summary>
    ///  Try saving the data, catch an IOException which will leave the user unable to save their settings
    /// </summary>
    /// <param name="parameter">w</param>
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

    /// <summary>
    /// Only allow user to save if the textboxes have no validation errors
    /// </summary>
    /// <param name="parameter"></param>
    /// <returns>Whether the user is allowed to save or not</returns>
    public override bool CanExecute(object? parameter)
    {
        return !MainViewModel.HasErrors;
    }
}
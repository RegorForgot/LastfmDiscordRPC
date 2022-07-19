using LastfmDiscordRPC.Models;
using LastfmDiscordRPC.ViewModels;

namespace LastfmDiscordRPC.Commands;

/// <summary>
///  The command that sets the APIKey and AppID text-boxes in the UI to their respective defaults
/// </summary>
public class DefaultCommand : CommandBase
{
    public DefaultCommand(MainViewModel mainViewModel) : base(mainViewModel)
    { }
    
    public override bool CanExecute(object? parameter)
    {
        return true;
    }
    
    /// <summary>
    /// Set the APIKey and the AppID to the defaults specified in SaveAppData.
    /// </summary>
    /// <param name="parameter"></param>
    public override void Execute(object? parameter)
    {
        MainViewModel.APIKey = SaveAppData.DefaultAPIKey;
        MainViewModel.AppID = SaveAppData.DefaultAppID;
    }
}
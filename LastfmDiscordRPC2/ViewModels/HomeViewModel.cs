using System.ComponentModel.DataAnnotations;
using ReactiveUI;
using static LastfmDiscordRPC2.Models.Utilities.SaveAppData;

namespace LastfmDiscordRPC2.ViewModels;

public class HomeViewModel : ViewModelBase
{
    private readonly MainViewModel _mainViewModel;
    private string _appID;
    
    [Required]
    [ValidDiscordID]
    public string AppID
    {
        get => _appID;
        set
        {
            this.RaiseAndSetIfChanged(ref _appID, value);
        }
    }

    public HomeViewModel(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
        AppID = SavedData.AppID;
    }

}
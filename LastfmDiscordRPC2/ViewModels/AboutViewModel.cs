using System.Diagnostics;
using System.Reactive;
using LastfmDiscordRPC2.Models;
using ReactiveUI;

namespace LastfmDiscordRPC2.ViewModels;

public class AboutViewModel : ViewModelBase
{
    MainViewModel _mainViewModel;
    public ReactiveCommand<Unit, Unit> GithubPage { get; }

    public AboutViewModel(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
        GithubPage = ReactiveCommand.Create(OpenGithubPage);
    }

    private void OpenGithubPage()
    {
        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = @"https://www.github.com/RegorForgotTheirPassword/LastfmDiscordRPC",
            UseShellExecute = true
        };
        Process.Start(psi);
    }
}
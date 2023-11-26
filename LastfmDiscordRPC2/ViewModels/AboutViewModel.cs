using System.Reactive;
using LastfmDiscordRPC2.Models;
using ReactiveUI;

namespace LastfmDiscordRPC2.ViewModels;

public class AboutViewModel : ViewModelBase
{
    public ReactiveCommand<Unit, Unit> GithubPage { get; }

    public AboutViewModel()
    {
        GithubPage = ReactiveCommand.Create(OpenGithubPage);
    }

    private void OpenGithubPage()
    {
        Utilities.OpenWebpage(@"https://www.github.com/RegorForgotTheirPassword/LastfmDiscordRPC");
    }
}
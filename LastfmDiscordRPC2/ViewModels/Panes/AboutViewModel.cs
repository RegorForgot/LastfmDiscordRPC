using System.Reactive;
using LastfmDiscordRPC2.Models;
using ReactiveUI;

namespace LastfmDiscordRPC2.ViewModels.Panes;

public sealed class AboutViewModel : ReactiveObject, IPaneViewModel
{
    public ReactiveCommand<Unit, Unit> GithubPage { get; }
    public string PaneName { get; }

    public AboutViewModel()
    {
        PaneName = "About";
        GithubPage = ReactiveCommand.Create(OpenGithubPage);
    }

    private void OpenGithubPage()
    {
        Utilities.OpenWebpage("https://www.github.com/RegorForgotTheirPassword/LastfmDiscordRPC");
    }
}
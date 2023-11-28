using System.Reactive;
using ReactiveUI;

namespace LastfmDiscordRPC2.ViewModels.Panes;

public sealed class AboutViewModel : AbstractPaneViewModel
{
    public ReactiveCommand<Unit, Unit> GithubPage { get; }
    public override string Name { get; }

    public AboutViewModel()
    {
        Name = "About";
        GithubPage = ReactiveCommand.Create(OpenGithubPage);
    }

    private void OpenGithubPage()
    {
        Utilities.Utilities.OpenWebpage("https://www.github.com/RegorForgotTheirPassword/LastfmDiscordRPC");
    }
}
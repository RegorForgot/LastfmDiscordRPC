using System.Reactive;
using LastfmDiscordRPC2.Models;
using ReactiveUI;

namespace LastfmDiscordRPC2.ViewModels.Panes;

public sealed class AboutViewModel : AbstractPaneViewModel
{
    public ReactiveCommand<Unit, Unit> GithubPage { get; }
    public override string Name => "About";

    public AboutViewModel(CurrentState state) : base(state)
    {
        GithubPage = ReactiveCommand.Create(OpenGithubPage);
    }

    private void OpenGithubPage()
    {
        Utilities.Utilities.OpenWebpage("https://www.github.com/RegorForgotTheirPassword/LastfmDiscordRPC");
    }
}
using System.Reactive;
using ReactiveUI;
using static LastfmDiscordRPC2.Utilities.Utilities;

namespace LastfmDiscordRPC2.ViewModels.Panes;

public sealed class AboutViewModel : AbstractPaneViewModel
{
    public ReactiveCommand<Unit, Unit> OpenGithubCmd { get; }
    public override string Name => "About";

    public AboutViewModel(UIContext context) : base(context)
    {
        OpenGithubCmd = ReactiveCommand.Create(OpenGithub);
    }

    private static void OpenGithub()
    {
        OpenWebpage("https://www.github.com/RegorForgotTheirPassword/LastfmDiscordRPC");
    }
}
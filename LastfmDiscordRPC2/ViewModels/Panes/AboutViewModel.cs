using System.Reactive;
using LastfmDiscordRPC2.IO;
using LastfmDiscordRPC2.ViewModels.Controls;
using ReactiveUI;
using static LastfmDiscordRPC2.Utilities.Utilities;

namespace LastfmDiscordRPC2.ViewModels.Panes;

public sealed class AboutViewModel : AbstractPaneViewModel
{
    public ReactiveCommand<Unit, Unit> OpenGithubCmd { get; }
    public ReactiveCommand<Unit, Unit> OpenFolderCmd { get; }
    public LoggingControlViewModel LoggingControlViewModel { get; }

    public override string Name => "About";

    public AboutViewModel(UIContext context, LoggingControlViewModel loggingControlViewModel) : base(context)
    {
        LoggingControlViewModel = loggingControlViewModel;
        OpenGithubCmd = ReactiveCommand.Create(OpenGithub);
        OpenFolderCmd = ReactiveCommand.Create(AbstractIOService.OpenFolder);
    }

    private static void OpenGithub()
    {
        OpenURI("https://www.github.com/RegorForgotTheirPassword/LastfmDiscordRPC");
    }
}
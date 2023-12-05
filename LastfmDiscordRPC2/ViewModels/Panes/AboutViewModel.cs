using System;
using System.Reactive;
using LastfmDiscordRPC2.IO;
using LastfmDiscordRPC2.ViewModels.Controls;
using ReactiveUI;
using static LastfmDiscordRPC2.Utilities.URIOpen;

namespace LastfmDiscordRPC2.ViewModels.Panes;

public sealed class AboutViewModel : AbstractPaneViewModel
{
    public ReactiveCommand<Unit, Unit> OpenFolderCmd { get; }
    public ReactiveCommand<string, Unit> OpenGithubCmd { get; }
    
    
    public LoggingControlViewModel LoggingControlViewModel { get; }
    public string GithubPage { get; init; } = "https://www.github.com/RegorForgotTheirPassword/LastfmDiscordRPC";

    public override string Name => "About";

    public AboutViewModel(UIContext context, LoggingControlViewModel loggingControlViewModel) : base(context)
    {
        LoggingControlViewModel = loggingControlViewModel;
        OpenGithubCmd = ReactiveCommand.Create<string>(OpenURI);
        OpenFolderCmd = ReactiveCommand.Create(AbstractIOService.OpenFolder);
    }
}
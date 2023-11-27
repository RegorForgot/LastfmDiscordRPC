using ReactiveUI;

namespace LastfmDiscordRPC2.ViewModels;

public interface ILoggingControlViewModel : IReactiveObject
{
    public string ConsoleText { get; set; }
}
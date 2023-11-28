using ReactiveUI;

namespace LastfmDiscordRPC2.ViewModels.Logging;

public interface ILoggingControlViewModel : IReactiveObject
{
    public string ConsoleText { get; set; }
}
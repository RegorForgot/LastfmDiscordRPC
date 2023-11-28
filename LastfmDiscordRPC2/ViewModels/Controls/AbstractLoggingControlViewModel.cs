using ReactiveUI;

namespace LastfmDiscordRPC2.ViewModels.Controls;

public class AbstractLoggingControlViewModel : AbstractControlViewModel
{
    private string _consoleText = "";
    
    public string ConsoleText
    {
        get => _consoleText;
        set => this.RaiseAndSetIfChanged(ref _consoleText, value);
    }
}
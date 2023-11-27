using ReactiveUI;

namespace LastfmDiscordRPC2.ViewModels;

public class SettingsConsoleViewModel : ReactiveObject, ILoggingControlViewModel
{
    private string _consoleText = "";
    
    public string ConsoleText
    {
        get => _consoleText;
        set => this.RaiseAndSetIfChanged(ref _consoleText, value);
    }
}
using ReactiveUI;

namespace LastfmDiscordRPC2.ViewModels.Controls;

public class LoggingControlViewModel : AbstractControlViewModel
{
    public override string Name { get => "LoggingControl"; }
    
    private string _consoleText = "";
    
    public string ConsoleText
    {
        get => _consoleText;
        set => this.RaiseAndSetIfChanged(ref _consoleText, value);
    }
}
using ReactiveUI;

namespace LastfmDiscordRPC2.ViewModels;

public abstract class AbstractViewModel : ReactiveObject
{
    public virtual string Name { get; }
}
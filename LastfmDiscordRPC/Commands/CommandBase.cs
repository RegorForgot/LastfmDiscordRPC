using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LastfmDiscordRPC.Commands;

public abstract class CommandBase : ICommand
{
    public abstract void Execute(object? parameter);

    public event EventHandler? CanExecuteChanged;
    public virtual bool CanExecute(object? parameter)
    {
        return true;
    }

    public virtual void RaiseCanExecuteChanged()
    {
        EventHandler? canExecuteChangedHandler = CanExecuteChanged;
        canExecuteChangedHandler?.Invoke(this, EventArgs.Empty);
    }
}
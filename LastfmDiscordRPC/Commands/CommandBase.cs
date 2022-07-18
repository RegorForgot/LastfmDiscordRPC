using System;
using System.Windows.Input;
using LastfmDiscordRPC.ViewModels;

namespace LastfmDiscordRPC.Commands;

public abstract class CommandBase : ICommand
{
    protected MainViewModel MainViewModel;

    protected CommandBase(MainViewModel mainViewModel)
    {
        MainViewModel = mainViewModel;
    }
    
    public abstract void Execute(object? parameter);

    public event EventHandler? CanExecuteChanged;
    public virtual bool CanExecute(object? parameter)
    {
        return true;
    }

    public void RaiseCanExecuteChanged()
    {
        EventHandler? canExecuteChangedHandler = CanExecuteChanged;
        canExecuteChangedHandler?.Invoke(this, EventArgs.Empty);
    }
}
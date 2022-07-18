using System;
using System.Windows.Input;
using LastfmDiscordRPC.ViewModels;

namespace LastfmDiscordRPC.Commands;

public abstract class CommandBase : ICommand
{
    readonly protected MainViewModel MainViewModel;
    public event EventHandler? CanExecuteChanged;

    protected CommandBase(MainViewModel mainViewModel)
    {
        MainViewModel = mainViewModel;
    }
    
    public abstract void Execute(object? parameter);
    public abstract bool CanExecute(object? parameter);

    public void RaiseCanExecuteChanged()
    {
        EventHandler? canExecuteChangedHandler = CanExecuteChanged;
        canExecuteChangedHandler?.Invoke(this, EventArgs.Empty);
    }
}
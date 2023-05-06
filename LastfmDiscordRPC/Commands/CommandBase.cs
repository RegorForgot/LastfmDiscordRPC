using System;
using System.Windows.Input;
using LastfmDiscordRPC.ViewModels;

namespace LastfmDiscordRPC.Commands;

/// <summary>
/// Base class for all button commands in the solution - stores the view model + holds RaiseCanExecuteChanged
/// </summary>
public abstract class CommandBase : ICommand
{
    protected readonly MainViewModel MainViewModel;
    public event EventHandler? CanExecuteChanged;
    public abstract void Execute(object? parameter);
    public abstract bool CanExecute(object? parameter);

    protected CommandBase(MainViewModel mainViewModel)
    {
        MainViewModel = mainViewModel;
    }

    /// <summary>
    /// This method simply calls CanExecute and changes the Enabled property of the control as necessary
    /// </summary>
    public void RaiseCanExecuteChanged()
    {
        EventHandler? canExecuteChangedHandler = CanExecuteChanged;
        canExecuteChangedHandler?.Invoke(this, EventArgs.Empty);
    }
}
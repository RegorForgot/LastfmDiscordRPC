using System;
using System.Windows.Input;

namespace LastfmDiscordRPC.Commands;

public abstract class CommandBase : ICommand
{
    public abstract void Execute(object? parameter);

    public event EventHandler? CanExecuteChanged;
    public bool CanExecute(object? parameter)
    {
        return true;
    }
}
using System;
using System.Reactive;
using Avalonia;
using Avalonia.Platform;
using LastfmDiscordRPC2.Models;
using ReactiveUI;

namespace LastfmDiscordRPC2.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private string? _os;
    private bool _startUpVisible;
    private bool _startUpChecked;
    public ReactiveCommand<bool, Unit> LaunchOnStartup { get; }

    public string? OS
    {
        get => _os;
        set => this.RaiseAndSetIfChanged(ref _os, value);
    }

    public bool StartUpVisible
    {
        get => _startUpVisible;
        set => this.RaiseAndSetIfChanged(ref _startUpVisible, value);
    }

    public bool StartUpChecked
    {
        get => _startUpChecked;
        set => this.RaiseAndSetIfChanged(ref _startUpChecked, value);
    }

    public MainWindowViewModel()
    {
        OS = AvaloniaLocator.Current.GetService<IRuntimePlatform>()?.GetRuntimeInfo().OperatingSystem.ToString();
        LaunchOnStartup = ReactiveCommand.Create<bool>(SetLaunchOnStartup);
        if (OperatingSystem.IsWindows())
        {
            StartUpVisible = true;
            StartUpChecked = Utilities.CheckRegistryExists();
        }
    }

    private void SetLaunchOnStartup(bool parameter)
    {
        Utilities.SetRegistry(parameter);
    }
}
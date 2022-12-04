using System;
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
        set
        {
            this.RaiseAndSetIfChanged(ref _startUpChecked, value);
            Utilities.SetRegistry(_startUpChecked);
        }
    }


    public MainWindowViewModel()
    {
        OS = AvaloniaLocator.Current.GetService<IRuntimePlatform>()?.GetRuntimeInfo().OperatingSystem.ToString();
        if (OperatingSystem.IsWindows())
        {
            StartUpVisible = true;
            StartUpChecked = Utilities.CheckRegistryExists();
        }
    }
}
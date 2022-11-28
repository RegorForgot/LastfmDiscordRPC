using System;
using System.Reactive;
using Avalonia.Animation;
using ReactiveUI;

namespace LastfmDiscordRPC2.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public MainWindowViewModel()
    {
        Animate = ReactiveCommand.Create(AnimateCommand);
    }
    
    public string Greeting => "Welcome to Avalonia!";
     
    public ReactiveCommand<Unit, Unit> Animate {get;}

    private void AnimateCommand()
    {
        var transition = new PageSlide(TimeSpan.FromMilliseconds(500), PageSlide.SlideAxis.Vertical);
    }
}
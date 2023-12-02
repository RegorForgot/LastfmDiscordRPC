using System;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using LastfmDiscordRPC2.Models.RPC;
using ReactiveUI;

namespace LastfmDiscordRPC2.ViewModels.Panes;

public sealed class HomeViewModel : AbstractPaneViewModel, IUpdatableViewModel
{
    private bool _isCoolDown;
    private bool IsCoolDown
    {
        get => _isCoolDown;
        set
        {
            this.RaiseAndSetIfChanged(ref _isCoolDown, value);
            this.RaisePropertyChanged(nameof(CanSetPresence));
        }
    }

    private readonly IPresenceService _presenceService;

    public ReactiveCommand<bool, Unit> SetPresence { get; }
    public bool CanSetPresence => Context.IsLoggedIn && !IsCoolDown;

    public override string Name => "Home";

    public HomeViewModel(
        IPresenceService presenceService,
        UIContext context) : base(context)
    {
        _presenceService = presenceService;
        SetPresence = ReactiveCommand.Create<bool>(StartPresenceCommand);
    }

    private void StartPresenceCommand(bool activated)
    {
        if (activated)
        {
            _presenceService.UnsetPresence();
        }
        else
        {
            Task.Run(_presenceService.SetPresence);
        }

        Task.Run(StartCountDown);
        Context.IsRichPresenceActivated = !activated;
    }

    private async Task StartCountDown()
    {
        IsCoolDown = true;

        try
        {
            using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromSeconds(5));
            while (await timer.WaitForNextTickAsync())
            {
                IsCoolDown = false;
                timer.Dispose();
            }
        }
        catch (Exception)
        {
            // ignored
        }
    }
    
    public void UpdateProperties()
    {
        this.RaisePropertyChanged(nameof(CanSetPresence));
    }
}
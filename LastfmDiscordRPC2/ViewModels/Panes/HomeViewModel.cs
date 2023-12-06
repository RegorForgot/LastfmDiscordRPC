using System;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using LastfmDiscordRPC2.Models.RPC;
using LastfmDiscordRPC2.ViewModels.Controls;
using ReactiveUI;

namespace LastfmDiscordRPC2.ViewModels.Panes;

public sealed class HomeViewModel : AbstractPaneViewModel
{
    private bool _isCoolDown;
    private bool IsCoolDown
    {
        get => _isCoolDown;
        set => this.RaiseAndSetIfChanged(ref _isCoolDown, value);
    }

    private readonly IPresenceService _presenceService;

    public ReactiveCommand<bool, Unit> SetPresence { get; }
    public PreviewControlViewModel PreviewControlViewModel { get; }

    public override string Name => "Home";

    public HomeViewModel(
        IPresenceService presenceService,
        PreviewControlViewModel previewControlViewModel,
        UIContext context) : base(context)
    {
        PreviewControlViewModel = previewControlViewModel;
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
}
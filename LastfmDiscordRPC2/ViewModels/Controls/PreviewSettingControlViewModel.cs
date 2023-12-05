using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using LastfmDiscordRPC2.IO;
using ReactiveUI;

namespace LastfmDiscordRPC2.ViewModels.Controls;

public class PreviewSettingControlViewModel : AbstractControlViewModel
{
    private readonly SaveCfgIOService _saveCfgIOService;
    public override string Name => "PreviewSettingControl";

    private string _details = Empty;
    private string _state = Empty;
    private string _largeImageLabel = Empty;
    private string _smallImageLabel = Empty;
    private ObservableCollection<PreviewButton> _buttons = new ObservableCollection<PreviewButton>();

    public string Details
    {
        get => _details;
        set
        {
            if (value is null)
            {
                return;
            }
            this.RaiseAndSetIfChanged(ref _details, value);
        }
    }

    public string State
    {
        get => _state;
        set
        {
            if (value is null)
            {
                return;
            }
            this.RaiseAndSetIfChanged(ref _state, value);
        }
    }

    public string LargeImageLabel
    {
        get => _largeImageLabel;
        set
        {
            if (value is null)
            {
                return;
            }
            this.RaiseAndSetIfChanged(ref _largeImageLabel, value);
        }
    }

    public string SmallImageLabel
    {
        get => _smallImageLabel;
        set
        {
            if (value is null)
            {
                return;
            }
            this.RaiseAndSetIfChanged(ref _smallImageLabel, value);
        }
    }

    public ObservableCollection<PreviewButton> Buttons
    {
        get => _buttons;
        set => this.RaiseAndSetIfChanged(ref _buttons, value);
    }

    public UIContext Context { get; }
    public ReactiveCommand<Unit, Unit> SavePreviewCmd { get; }

    public PreviewSettingControlViewModel(UIContext context, SaveCfgIOService saveCfgIOService)
    {
        _saveCfgIOService = saveCfgIOService;
        Context = context;
        SetProperties();
        SavePreviewCmd = ReactiveCommand.Create(SavePreview);
    }
    
    private void SavePreview()
    {
        _saveCfgIOService.SaveCfg.UserRPCCfg.Details = Details;
        _saveCfgIOService.SaveCfg.UserRPCCfg.State = State;
        _saveCfgIOService.SaveCfg.UserRPCCfg.LargeImageLabel = LargeImageLabel;
        _saveCfgIOService.SaveCfg.UserRPCCfg.SmallImageLabel = SmallImageLabel;
        _saveCfgIOService.SaveConfigData();
    }

    public void SetProperties()
    {
        Details = _saveCfgIOService.SaveCfg.UserRPCCfg.Details;
        State = _saveCfgIOService.SaveCfg.UserRPCCfg.State;
        LargeImageLabel = _saveCfgIOService.SaveCfg.UserRPCCfg.LargeImageLabel;
        SmallImageLabel = _saveCfgIOService.SaveCfg.UserRPCCfg.SmallImageLabel;
        Buttons = new ObservableCollection<PreviewButton>(
            _saveCfgIOService.SaveCfg.UserRPCCfg.UserButtons.Select(button =>
                new PreviewButton
                {
                    URL = button.URL,
                    Label = button.Label
                }
            )
        );
    }
}
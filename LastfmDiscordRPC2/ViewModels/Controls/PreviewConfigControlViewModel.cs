using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using LastfmDiscordRPC2.IO;
using LastfmDiscordRPC2.Models;
using LastfmDiscordRPC2.ViewModels.Update;
using LastfmDiscordRPC2.Views;
using ReactiveUI;
using static LastfmDiscordRPC2.DataTypes.SaveVars;

namespace LastfmDiscordRPC2.ViewModels.Controls;

public class PreviewConfigControlViewModel : AbstractControlViewModel, ISettableViewModel
{
    private readonly SaveCfgIOService _saveCfgIOService;
    public override string Name => "PreviewConfigControl";

    private string _details = Empty;
    private string _state = Empty;
    private string _largeImageLabel = Empty;
    private string _smallImageLabel = Empty;
    private ObservableCollection<RPCButton> _buttons = new ObservableCollection<RPCButton>();

    public string Details
    {
        get => _details;
        set => this.RaiseAndSetIfChanged(ref _details, value);
    }

    public string State
    {
        get => _state;
        set => this.RaiseAndSetIfChanged(ref _state, value);
    }

    public string LargeImageLabel
    {
        get => _largeImageLabel;
        set => this.RaiseAndSetIfChanged(ref _largeImageLabel, value);
    }

    public string SmallImageLabel
    {
        get => _smallImageLabel;
        set => this.RaiseAndSetIfChanged(ref _smallImageLabel, value);
    }

    public ObservableCollection<RPCButton> Buttons
    {
        get => _buttons;
        set => this.RaiseAndSetIfChanged(ref _buttons, value);
    }

    public bool CanRemove => Buttons.Count > 0;
    public bool CanAdd => Buttons.Count < 2;
    public bool CanSave => Buttons.All(button => !button.IsInvalidUrl);

    public UIContext Context { get; }
    public ReactiveCommand<Unit, Unit> SavePreviewCmd { get; }
    public ReactiveCommand<Unit, Unit> ResetPreviewCmd { get; }
    public ReactiveCommand<Unit, Unit> VarDialogCmd { get; }
    public ReactiveCommand<Unit, Unit> AddButtonCmd { get; }
    public ReactiveCommand<Unit, Unit> RemoveButtonCmd { get; }

    public PreviewConfigControlViewModel(UIContext context, SaveCfgIOService saveCfgIOService)
    {
        _saveCfgIOService = saveCfgIOService;
        Context = context;

        SavePreviewCmd = ReactiveCommand.Create(SavePreview);
        ResetPreviewCmd = ReactiveCommand.Create(ResetPreview);
        VarDialogCmd = ReactiveCommand.Create(VarDialog);
        AddButtonCmd = ReactiveCommand.Create(AddButton);
        RemoveButtonCmd = ReactiveCommand.Create(RemoveButton);

        SetProperties();
    }

    private void VarDialog()
    {
        DialogWindow.Show(null);
    }

    private void SavePreview()
    {
        _saveCfgIOService.SaveCfg.UserRPCCfg.Details = Details;
        _saveCfgIOService.SaveCfg.UserRPCCfg.State = State;
        _saveCfgIOService.SaveCfg.UserRPCCfg.LargeImageLabel = LargeImageLabel;
        _saveCfgIOService.SaveCfg.UserRPCCfg.SmallImageLabel = SmallImageLabel;
        _saveCfgIOService.SaveCfg.UserRPCCfg.UserButtons = Buttons.ToArray();
        _saveCfgIOService.SaveConfigData();
    }

    private void ResetPreview()
    {
        Details = DefaultDetails;
        State = DefaultState;
        LargeImageLabel = DefaultLargeImageLabel;
        SmallImageLabel = DefaultSmallImageLabel;
        Buttons = new ObservableCollection<RPCButton>(
            DefaultUserButtons.Select(button =>
                {
                    button.Action = UpdateCanSave;
                    return button;
                }
            )
        );
        SavePreview();
        UpdateButtonAddRemove();
    }

    private void RemoveButton()
    {
        Buttons.RemoveAt(Buttons.Count - 1);
        this.RaisePropertyChanged(nameof(CanSave));
        UpdateButtonAddRemove();
    }

    private void AddButton()
    {
        Buttons.Add(new RPCButton(UpdateCanSave));
        UpdateButtonAddRemove();
    }


    public void SetProperties()
    {
        UpdateButtonAddRemove();
        Details = _saveCfgIOService.SaveCfg.UserRPCCfg.Details;
        State = _saveCfgIOService.SaveCfg.UserRPCCfg.State;
        LargeImageLabel = _saveCfgIOService.SaveCfg.UserRPCCfg.LargeImageLabel;
        SmallImageLabel = _saveCfgIOService.SaveCfg.UserRPCCfg.SmallImageLabel;
        Buttons = new ObservableCollection<RPCButton>(
            _saveCfgIOService.SaveCfg.UserRPCCfg.UserButtons.Select(button =>
                {
                    button.Action = UpdateCanSave;
                    return button;
                }
            )
        );
    }

    private void UpdateCanSave()
    {
        this.RaisePropertyChanged(nameof(CanSave));
    }

    private void UpdateButtonAddRemove()
    {
        this.RaisePropertyChanged(nameof(CanRemove));
        this.RaisePropertyChanged(nameof(CanAdd));
    }
}
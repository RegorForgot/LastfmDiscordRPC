using System;
using LastfmDiscordRPC2.DataTypes;
using LastfmDiscordRPC2.Utilities;
using Newtonsoft.Json;
using ReactiveUI;

namespace LastfmDiscordRPC2.Models;

public record RPCButton : ReactiveRecord
{
    private string _url = SaveVars.DefaultButtonURL;
    private string _label = SaveVars.DefaultButtonLabel;
    private bool _isInvalidUrl;
    
    public Action? Action { get; set; }
    
    [JsonProperty]
    public string Label
    {
        get => _label;
        set => this.RaiseAndSetIfChanged(ref _label, value);
    }
    
    [JsonProperty]
    public string URL
    {
        get => _url;
        set
        {
            this.RaiseAndSetIfChanged(ref _url, value);
            if (Action is null)
            {
                return;
            }
            IsInvalidUrl = !DiscordClientExtensions.ValidatePlaceholderLink(value);
        }
    }

    public bool IsInvalidUrl
    {
        get => _isInvalidUrl;
        private set
        {
            if (_isInvalidUrl == value)
            {
                return;
            }
            this.RaiseAndSetIfChanged(ref _isInvalidUrl, value);
            Action?.Invoke();
        }
    }

    public RPCButton()
    {
        Action = null;
    }
    
    public RPCButton(Action action)
    {
        Action = action;
    }
    
    public RPCButton(RPCButton toClone, Action action)
    {
        URL = toClone.URL;
        Label = toClone.Label;
        Action = action;
    }
}

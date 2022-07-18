using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using LastfmDiscordRPC.Commands;

namespace LastfmDiscordRPC.ViewModels;

public partial class MainViewModel : INotifyDataErrorInfo
{
    private readonly Dictionary<string, List<string>> _errorsByPropertyName = new Dictionary<string, List<string>>();
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;
    public bool HasErrors => _errorsByPropertyName.Any();

    public bool PropertyHasError(string propertyName)
    {
        return _errorsByPropertyName.ContainsKey(propertyName);
    }

    public IEnumerable GetErrors(string? propertyName)
    {
        return _errorsByPropertyName.ContainsKey(propertyName) ? _errorsByPropertyName[propertyName] : null;
    }

    private void AddError(string propertyName, string error)
    {
        if (!_errorsByPropertyName.ContainsKey(propertyName))
            _errorsByPropertyName[propertyName] = new List<string>();

        if (_errorsByPropertyName[propertyName].Contains(error)) return;
        _errorsByPropertyName[propertyName].Add(error);
        OnErrorsChanged(propertyName);
    }

    private void ClearErrors(string propertyName)
    {
        if (!_errorsByPropertyName.ContainsKey(propertyName)) return;
        _errorsByPropertyName.Remove(propertyName);
        OnErrorsChanged(propertyName);
    }

    private void OnErrorsChanged(string? propertyName)
    {
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        ((CommandBase)SetPresenceCommand).RaiseCanExecuteChanged();
        ((CommandBase)SaveCommand).RaiseCanExecuteChanged();
    }

    private void ValidateUsername()
    {
        ClearErrors(nameof(Username));
        const string pattern = @"^[A-Za-z0-9-_]{1,15}$";

        if (Regex.IsMatch(Username, pattern)) return;
        AddError(nameof(Username), "Username is invalid.");
    }

    private void ValidateAPIKey()
    {
        ClearErrors(nameof(APIKey));
        const string pattern = @"^[0-9a-f]{32}$";

        if (Regex.IsMatch(APIKey, pattern)) return;
        AddError(nameof(APIKey), "Last.fm API key is invalid.");
    }

    private void ValidateAppKey()
    {
        ClearErrors(nameof(AppKey));
        const string pattern = @"^\d{14,19}$";

        if (Regex.IsMatch(AppKey, pattern)) return;
        AddError(nameof(AppKey), "Discord application key is invalid.");
    }
}
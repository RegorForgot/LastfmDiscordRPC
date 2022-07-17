using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace LastfmDiscordRPC.Models;

public class UsernameRule : ValidationRule
{
    private readonly string _pattern = @"^[A-Za-z0-9-_]{1,15}$";
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        return Regex.IsMatch((string)value, _pattern) ? ValidationResult.ValidResult : new ValidationResult(false, "Username is invalid.");
    }
}

public class APIKeyRule : ValidationRule
{
    private readonly string _pattern = @"^[0-9a-f]{32}$";
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        return Regex.IsMatch((string)value, _pattern) ? ValidationResult.ValidResult : new ValidationResult(false, "API Key is invalid.");
    }
}

public class ApplicationKeyRule : ValidationRule
{
    private readonly string _pattern = @"^\d{14,19}$";
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        return Regex.IsMatch((string)value, _pattern) ? ValidationResult.ValidResult : new ValidationResult(false, "Application key is invalid.");
    }
}
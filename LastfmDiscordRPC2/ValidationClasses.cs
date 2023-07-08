using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace LastfmDiscordRPC2;
public class ValidDiscordID : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        string appID = (string)value!;
        const string pattern = @"^\d{17,21}$";

       return Regex.IsMatch(appID, pattern)
            ? ValidationResult.Success
            : new ValidationResult("AppID is invalid.");
    }
}
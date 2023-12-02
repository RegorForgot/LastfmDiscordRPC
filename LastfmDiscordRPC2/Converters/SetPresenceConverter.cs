using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace LastfmDiscordRPC2.Converters;

public sealed class SetPresenceConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isSet)
        {
            return isSet ? "Disable presence" : "Set presence";
        }

        return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
    }
    
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
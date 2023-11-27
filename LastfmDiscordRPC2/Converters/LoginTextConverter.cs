using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace LastfmDiscordRPC2.Converters;

public class LoginTextConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isLogin)
        {
            return isLogin ? "Log into Last.fm" : "Log out";
        }

        return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
using System;
using System.Drawing;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media;
using LastfmDiscordRPC2.Assets;

namespace LastfmDiscordRPC2.Converters;

public sealed class LovedConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isLoved)
        {
            Thickness borderThickness = new Thickness(isLoved ? 14.2 : 1);
            SolidColorBrush color = isLoved ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.White);

            PathIcon icon = new PathIcon
            {
                BorderThickness = borderThickness,
                Foreground = color,
                Data = isLoved ? ParsedIcons.FilledHeart : ParsedIcons.RegularHeart
            };

            return icon;
        }

        return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
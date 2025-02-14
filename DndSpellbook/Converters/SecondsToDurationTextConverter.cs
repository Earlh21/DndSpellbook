using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace DndSpellbook.Converters;

public class SecondsToDurationTextConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not int seconds) return null;

        if(seconds % 3600 == 0) return $"{seconds / 3600} hours";
        if(seconds % 60 == 0) return $"{seconds / 60} minutes";
        
        return $"{seconds} seconds";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
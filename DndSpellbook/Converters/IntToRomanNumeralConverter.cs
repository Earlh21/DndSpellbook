using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace DndSpellbook.Converters;

public class IntToRomanNumeralConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if(value is not int number)
            return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);

        return number switch
        {
            1 => "\u2160",
            2 => "\u2161",
            3 => "\u2162",
            4 => "\u2163",
            5 => "\u2164",
            6 => "\u2165",
            7 => "\u2166",
            8 => "\u2167",
            9 => "\u2168",
            _ => ""
        }
        ;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
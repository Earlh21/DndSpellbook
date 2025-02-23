using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace DndSpellbook.Converters;

public class SpellDescToBrushConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string str) return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);

        return new SolidColorBrush(SpellDescToColorConverter.Convert(str));
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
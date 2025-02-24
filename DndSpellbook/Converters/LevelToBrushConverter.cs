using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace DndSpellbook.Converters;

public class LevelToBrushConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if(value is not int intLevel) return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
        
        byte level = Byte.Min(255, (byte)intLevel);
        
        byte red = (byte)(255 - level * 10);
        byte green = (byte)(255 - level * 30);
        byte blue = (byte)(255 - level * 30);
        var color = Color.FromRgb(red, green, blue);
        return new SolidColorBrush(color);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
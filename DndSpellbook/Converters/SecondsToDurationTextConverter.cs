using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace DndSpellbook.Converters;

public class SecondsToDurationTextConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not int seconds) return new BindingNotification(new InvalidCastException(), BindingErrorType.Error, "");

        return Convert(seconds);
    }
    
    public static string Convert(int seconds)
    {
        int minute = 60;
        int hour = minute * 60;
        int day = hour * 24;
        int week = day * 7;
        int month = day * 30;
        int year = day * 365;
        
        if (seconds % year == 0) return $"{seconds / year} years";
        if (seconds % month == 0) return $"{seconds / month} months";
        if (seconds % week == 0) return $"{seconds / week} weeks";
        if (seconds % day == 0) return $"{seconds / day} days";
        if (seconds % hour == 0) return $"{seconds / hour} hours";
        if (seconds % minute == 0) return $"{seconds / minute} minutes";
        
        return $"{seconds} seconds";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
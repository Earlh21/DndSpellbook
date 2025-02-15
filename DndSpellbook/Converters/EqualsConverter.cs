using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;

namespace DndSpellbook.Converters;

public class EqualsConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        for (int i = 1; i < values.Count; i++)
        {
            if(values[i] == null) return values[i - 1] == null;
            
            if (!values[i]!.Equals(values[i - 1]))
            {
                return false;
            }
        }
        
        return true;
    }
}
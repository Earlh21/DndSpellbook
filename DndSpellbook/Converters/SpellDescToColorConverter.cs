using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace DndSpellbook.Converters;

public class SpellDescToColorConverter : IValueConverter
{
    private static readonly Color DefaultColor = Color.FromRgb(224, 224, 224);
    private static readonly Color AcidColor = Color.FromRgb(0, 255, 0);
    private static readonly Color ColdColor = Color.FromRgb(0, 255, 255);
    private static readonly Color FireColor = Color.FromRgb(255, 0, 0);
    private static readonly Color ForceColor = Color.FromRgb(255, 255, 0);
    private static readonly Color LightningColor = Color.FromRgb(255, 128, 0);
    private static readonly Color NecroticColor = Color.FromRgb(128, 0, 128);
    private static readonly Color PoisonColor = Color.FromRgb(0, 128, 0);
    private static readonly Color PsychicColor = Color.FromRgb(255, 0, 255);
    private static readonly Color RadiantColor = Color.FromRgb(255, 255, 255);
    private static readonly Color ThunderColor = Color.FromRgb(128, 128, 128);
    
    public static Color Convert(string str)
    {
        if(str.Contains("Acid", StringComparison.OrdinalIgnoreCase)) return AcidColor;
        if(str.Contains("Cold", StringComparison.OrdinalIgnoreCase)) return ColdColor;
        if(str.Contains("Fire", StringComparison.OrdinalIgnoreCase)) return FireColor;
        if(str.Contains("Force", StringComparison.OrdinalIgnoreCase)) return ForceColor;
        if(str.Contains("Lightning", StringComparison.OrdinalIgnoreCase)) return LightningColor;
        if(str.Contains("Necrotic", StringComparison.OrdinalIgnoreCase)) return NecroticColor;
        if(str.Contains("Poison", StringComparison.OrdinalIgnoreCase)) return PoisonColor;
        if(str.Contains("Psychic", StringComparison.OrdinalIgnoreCase)) return PsychicColor;
        if(str.Contains("Radiant", StringComparison.OrdinalIgnoreCase)) return RadiantColor;
        if(str.Contains("Thunder", StringComparison.OrdinalIgnoreCase)) return ThunderColor;
        
        return DefaultColor;
    }
    
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string str) return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
        
        return Convert(str);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
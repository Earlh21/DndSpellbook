using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using DndSpellbook.Data.Models.Enums;

namespace DndSpellbook.Converters;

public class SpellSchoolBackgroundConverter : IValueConverter
{
    public static IImage? AbjurationBackground { get; private set;}
    public static IImage? ConjurationBackground { get; private set;}
    public static IImage? DivinationBackground { get; private set;}
    public static IImage? EnchantmentBackground { get; private set;}
    public static IImage? EvocationBackground { get; private set;}
    public static IImage? IllusionBackground { get; private set;}
    public static IImage? NecromancyBackground { get; private set;}
    public static IImage? TransmutationBackground { get; private set;}
    
    public static void LoadImages()
    {
        AbjurationBackground = GetImage("avares://DndSpellbook/Assets/CardBackgrounds/abjuration_background.jpg");
        ConjurationBackground = GetImage("avares://DndSpellbook/Assets/CardBackgrounds/conjuration_background.jpg");
        DivinationBackground = GetImage("avares://DndSpellbook/Assets/CardBackgrounds/divination_background.jpg");
        EnchantmentBackground = GetImage("avares://DndSpellbook/Assets/CardBackgrounds/enchantment_background.jpg");
        EvocationBackground = GetImage("avares://DndSpellbook/Assets/CardBackgrounds/evocation_background.jpg");
        IllusionBackground = GetImage("avares://DndSpellbook/Assets/CardBackgrounds/illusion_background.jpg");
        NecromancyBackground = GetImage("avares://DndSpellbook/Assets/CardBackgrounds/necromancy_background.jpg");
        TransmutationBackground = GetImage("avares://DndSpellbook/Assets/CardBackgrounds/transmutation_background.jpg");
    }
    
    private static IImage? GetImage(string path)
    {
        using var stream = AssetLoader.Open(new Uri(path));
        return new Bitmap(stream);
    }
    
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not SpellSchool school)
            return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);

        return school switch
        {
            SpellSchool.Abjuration => AbjurationBackground,
            SpellSchool.Conjuration => ConjurationBackground,
            SpellSchool.Divination => DivinationBackground,
            SpellSchool.Enchantment => EnchantmentBackground,
            SpellSchool.Evocation => EvocationBackground,
            SpellSchool.Illusion => IllusionBackground,
            SpellSchool.Necromancy => NecromancyBackground,
            SpellSchool.Transmutation => TransmutationBackground,
            _ => AbjurationBackground
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
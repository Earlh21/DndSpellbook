using System;
using System.Globalization;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using DndSpellbook.Data.Models.Enums;

namespace DndSpellbook.Converters;

public class SpellSchoolIconConverter : IValueConverter
{
    public static IImage? AbjurationIcon { get; private set;}
    public static IImage? ConjurationIcon { get; private set;}
    public static IImage? DivinationIcon { get; private set;}
    public static IImage? EnchantmentIcon { get; private set;}
    public static IImage? EvocationIcon { get; private set;}
    public static IImage? IllusionIcon { get; private set;}
    public static IImage? NecromancyIcon { get; private set;}
    public static IImage? TransmutationIcon { get; private set;}

    public static void LoadImages()
    {
        AbjurationIcon = GetImage("avares://DndSpellbook/Assets/Icons/abjuration.png");
        ConjurationIcon = GetImage("avares://DndSpellbook/Assets/Icons/conjuration.png");
        DivinationIcon = GetImage("avares://DndSpellbook/Assets/Icons/divination.png");
        EnchantmentIcon = GetImage("avares://DndSpellbook/Assets/Icons/enchantment.png");
        EvocationIcon = GetImage("avares://DndSpellbook/Assets/Icons/evocation.png");
        IllusionIcon = GetImage("avares://DndSpellbook/Assets/Icons/illusion.png");
        NecromancyIcon = GetImage("avares://DndSpellbook/Assets/Icons/necromancy.png");
        TransmutationIcon = GetImage("avares://DndSpellbook/Assets/Icons/transmutation.png");
    }
    
    private static IImage? GetImage(string path)
    {
        using var stream = AssetLoader.Open(new Uri(path));
        return new Bitmap(stream);
    }
    
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is SpellSchool school)
        {
            return school switch
            {
                SpellSchool.Abjuration => AbjurationIcon,
                SpellSchool.Conjuration => ConjurationIcon,
                SpellSchool.Divination => DivinationIcon,
                SpellSchool.Enchantment => EnchantmentIcon,
                SpellSchool.Evocation => EvocationIcon,
                SpellSchool.Illusion => IllusionIcon,
                SpellSchool.Necromancy => NecromancyIcon,
                SpellSchool.Transmutation => TransmutationIcon,
                _ => new BindingNotification(new InvalidCastException(), BindingErrorType.Error)
            };
        }

        return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
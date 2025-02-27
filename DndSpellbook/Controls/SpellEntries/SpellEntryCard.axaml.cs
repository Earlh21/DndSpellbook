using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DndSpellbook.Data.Models.Enums;

namespace DndSpellbook.Controls;

public partial class SpellEntryCard : UserControl
{
    public static RechargeType[] RechargeTypes { get; } = Enum.GetValues<RechargeType>();
    
    public SpellEntryCard()
    {
        InitializeComponent();
    }
}
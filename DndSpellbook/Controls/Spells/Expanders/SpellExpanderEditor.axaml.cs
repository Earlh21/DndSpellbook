using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using DndSpellbook.Data.Models.Enums;

namespace DndSpellbook.Controls;

public partial class SpellExpanderEditor : UserControl
{    
    public static readonly StyledProperty<SpellEditor?> SpellEditorProperty =
        AvaloniaProperty.Register<SpellCardEditor, SpellEditor?>(nameof(SpellEditor), defaultBindingMode: BindingMode.TwoWay);
    
    public static CastingTimeType[] CastingTimeTypes { get; } = Enum.GetValues<CastingTimeType>();
    public static RangeType[] RangeTypes { get; } = Enum.GetValues<RangeType>();
    public static SpellSchool[] Schools { get; } = Enum.GetValues<SpellSchool>();
    public static int[] Levels { get; } = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9];

    public SpellEditor? SpellEditor
    {
        get => GetValue(SpellEditorProperty);
        set => SetValue(SpellEditorProperty, value);
    }
    public SpellExpanderEditor()
    {
        InitializeComponent();
    }

    private void InputElement_OnTapped(object? sender, TappedEventArgs e)
    {
        int d = 3;
        SpellEditor.EditCopy.School = SpellEditor.EditCopy.School;
    }
}
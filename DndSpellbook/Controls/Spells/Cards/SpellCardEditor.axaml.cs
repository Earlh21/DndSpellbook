using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using DndSpellbook.Data.Models.Enums;

namespace DndSpellbook.Controls;

public partial class SpellCardEditor : UserControl
{
    public static readonly StyledProperty<SpellEditor> SpellEditorProperty =
        AvaloniaProperty.Register<SpellCardEditor, SpellEditor>(nameof(SpellEditor));

    public SpellEditor SpellEditor
    {
        get => GetValue(SpellEditorProperty);
        set => SetValue(SpellEditorProperty, value);
    }
    
    public static readonly StyledProperty<ICommand> SaveCommandProperty =
        AvaloniaProperty.Register<SpellCardEditor, ICommand>(nameof(SaveCommand));

    public ICommand SaveCommand
    {
        get => GetValue(SaveCommandProperty);
        set => SetValue(SaveCommandProperty, value);
    }
    
    public static readonly StyledProperty<ICommand> CancelCommandProperty =
        AvaloniaProperty.Register<SpellCardEditor, ICommand>(nameof(CancelCommand));

    public ICommand CancelCommand
    {
        get => GetValue(CancelCommandProperty);
        set => SetValue(CancelCommandProperty, value);
    }
    
    public static CastingTimeType[] CastingTimeTypes { get; } = Enum.GetValues<CastingTimeType>();
    public static RangeType[] RangeTypes { get; } = Enum.GetValues<RangeType>();
    public static SpellSchool[] Schools { get; } = Enum.GetValues<SpellSchool>();
    public static int[] Levels { get; } = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9];
    
    public SpellCardEditor()
    {
        InitializeComponent();
    }
}
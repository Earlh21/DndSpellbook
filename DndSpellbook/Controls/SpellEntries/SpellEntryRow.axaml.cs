using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DndSpellbook.Data.Models.Enums;

namespace DndSpellbook.Controls;

public partial class SpellEntryRow : UserControl
{
    public static RechargeType[] RechargeTypes { get; } = Enum.GetValues<RechargeType>();
    
    public static readonly StyledProperty<SpellEntryEditor> SpellEntryEditorProperty =
        AvaloniaProperty.Register<SpellEntryRow, SpellEntryEditor>(nameof(SpellEntryEditor));

    public SpellEntryEditor SpellEntryEditor
    {
        get => GetValue(SpellEntryEditorProperty);
        set => SetValue(SpellEntryEditorProperty, value);
    }
    
    public static readonly StyledProperty<ICommand> DeleteCommandProperty =
        AvaloniaProperty.Register<SpellEntryRow, ICommand>(nameof(DeleteCommand));

    public ICommand DeleteCommand
    {
        get => GetValue(DeleteCommandProperty);
        set => SetValue(DeleteCommandProperty, value);
    }
    
    public SpellEntryRow()
    {
        InitializeComponent();
    }
}
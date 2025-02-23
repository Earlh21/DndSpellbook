using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace DndSpellbook.Controls;

public partial class SpellEntryRow : UserControl
{
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
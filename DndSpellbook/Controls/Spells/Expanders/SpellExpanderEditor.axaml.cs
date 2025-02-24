using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace DndSpellbook.Controls;

public partial class SpellExpanderEditor : UserControl
{    
    public static readonly StyledProperty<SpellEditor> SpellEditorProperty =
        AvaloniaProperty.Register<SpellCardEditor, SpellEditor>(nameof(SpellEditor));

    public SpellEditor SpellEditor
    {
        get => GetValue(SpellEditorProperty);
        set => SetValue(SpellEditorProperty, value);
    }
    public SpellExpanderEditor()
    {
        InitializeComponent();
    }
}
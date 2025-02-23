using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace DndSpellbook.Controls.Spells;

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
    
    public SpellCardEditor()
    {
        InitializeComponent();
    }
}
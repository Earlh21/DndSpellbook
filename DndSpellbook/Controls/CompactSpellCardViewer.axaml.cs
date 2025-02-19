using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DndSpellbook.Data.Models;

namespace DndSpellbook.Controls;

public partial class CompactSpellCardViewer : UserControl
{
    public static readonly StyledProperty<Spell> SpellProperty =
        AvaloniaProperty.Register<SpellCardViewer, Spell>(nameof(Spell));

    public Spell Spell
    {
        get => GetValue(SpellProperty);
        set => SetValue(SpellProperty, value);
    }
    
    public static readonly StyledProperty<ICommand?> DeleteCommandProperty =
        AvaloniaProperty.Register<SpellCardViewer, ICommand?>(nameof(DeleteCommand));

    public ICommand? DeleteCommand
    {
        get => GetValue(DeleteCommandProperty);
        set => SetValue(DeleteCommandProperty, value);
    }
    
    public CompactSpellCardViewer()
    {
        InitializeComponent();
    }
}
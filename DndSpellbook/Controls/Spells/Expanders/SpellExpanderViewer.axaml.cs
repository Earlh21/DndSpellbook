using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DndSpellbook.Data.Models;

namespace DndSpellbook.Controls;

public partial class SpellExpanderViewer : UserControl
{
    public static readonly StyledProperty<Spell> SpellProperty =
        AvaloniaProperty.Register<SpellCardViewer, Spell>(nameof(Spell));
    
    public Spell Spell
    {
        get => GetValue(SpellProperty);
        set => SetValue(SpellProperty, value);
    }
    
    public SpellExpanderViewer()
    {
        InitializeComponent();
    }
}
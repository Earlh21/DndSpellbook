using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using DndSpellbook.Data.Models;
using ReactiveUI;

namespace DndSpellbook.Controls;

public partial class SpellCardViewer : UserControl
{
    public static readonly StyledProperty<Spell> SpellProperty =
        AvaloniaProperty.Register<SpellCardViewer, Spell>(nameof(Spell));

    public Spell Spell
    {
        get => GetValue(SpellProperty);
        set => SetValue(SpellProperty, value);
    }

    // IsSelector property
    public static readonly StyledProperty<bool> IsSelectorProperty =
        AvaloniaProperty.Register<SpellCardViewer, bool>(nameof(IsSelector));

    public bool IsSelector
    {
        get => GetValue(IsSelectorProperty);
        set => SetValue(IsSelectorProperty, value);
    }

    // IsSelected property
    public static readonly StyledProperty<bool> IsSelectedProperty =
        AvaloniaProperty.Register<SpellCardViewer, bool>(nameof(IsSelected), defaultBindingMode: BindingMode.TwoWay);

    public bool IsSelected
    {
        get => GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }
    
    public static readonly StyledProperty<ICommand?> EditCommandProperty =
        AvaloniaProperty.Register<SpellCardViewer, ICommand?>(nameof(EditCommand));

    public ICommand? EditCommand
    {
        get => GetValue(EditCommandProperty);
        set => SetValue(EditCommandProperty, value);
    }
    
    public static readonly StyledProperty<ICommand?> DeleteCommandProperty =
        AvaloniaProperty.Register<SpellCardViewer, ICommand?>(nameof(DeleteCommand));

    public ICommand? DeleteCommand
    {
        get => GetValue(DeleteCommandProperty);
        set => SetValue(DeleteCommandProperty, value);
    }
    
    public SpellCardViewer()
    {
        InitializeComponent();
    }
}
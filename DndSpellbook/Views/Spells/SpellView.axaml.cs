using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;

namespace DndSpellbook.Views;

public partial class SpellView : ReactiveUserControl<SpellViewModel>
{
    public SpellView()
    {
        InitializeComponent();
    }
}
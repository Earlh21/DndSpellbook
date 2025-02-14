using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;

namespace DndSpellbook.Views.SpellLists;

public partial class SpellListsView : ReactiveUserControl<SpellListsViewModel>
{
    public SpellListsView()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        
        if(DataContext is not SpellListsViewModel vm) return;

        vm.LoadDataAsync();
    }
}
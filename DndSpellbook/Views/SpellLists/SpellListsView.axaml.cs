using System;
using Avalonia.ReactiveUI;

namespace DndSpellbook.Views;

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
using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using DndSpellbook.Windows;

namespace DndSpellbook.Views;

public partial class SpellListView : ReactiveUserControl<SpellListViewModel>
{
    public SpellListView()
    {
        InitializeComponent();
    }
    
    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        
        if (DataContext is not SpellListViewModel vm) return;

        vm.AddSpellsInteraction.RegisterHandler(async ic =>
        {
            if (TopLevel.GetTopLevel(this) is not Window window) return;

            var dialog = new DialogWindow(ic.Input);
            
            var result = await dialog.ShowDialog<IEnumerable<int>?>(window);
            ic.SetOutput(result);
        });
        
        vm.LoadDataAsync();
    }
}
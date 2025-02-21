﻿using System;
using System.Threading.Tasks;
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

        Task.Run(vm.LoadDataAsync);
    }
}
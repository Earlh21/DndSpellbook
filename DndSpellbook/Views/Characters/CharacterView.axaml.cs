using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using DndSpellbook.Windows;
using ReactiveUI;

namespace DndSpellbook.Views;

public partial class CharacterView : ReactiveUserControl<CharacterViewModel> 
{
    public CharacterView()
    {
        this.WhenActivated(disposables => { });
        AvaloniaXamlLoader.Load(this);
    }
    
    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        
        if (DataContext is not CharacterViewModel vm) return;

        vm.AddSpellsInteraction.RegisterHandler(async ic =>
        {
            if (TopLevel.GetTopLevel(this) is not Window window) return;

            var dialog = new DialogWindow(ic.Input);
            
            var result = await dialog.ShowDialog<IEnumerable<int>?>(window);
            ic.SetOutput(result);
        });
        
        Task.Run(vm.LoadDataAsync);
    }
}
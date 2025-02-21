using System;
using System.Threading.Tasks;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;

namespace DndSpellbook.Views;

public partial class CharactersView : ReactiveUserControl<CharactersViewModel>
{
    public CharactersView()
    {
        this.WhenActivated(disposables => { });
        AvaloniaXamlLoader.Load(this);
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        
        if (DataContext is not CharactersViewModel vm) return;
        
        Task.Run(vm.LoadDataAsync);
    }
}
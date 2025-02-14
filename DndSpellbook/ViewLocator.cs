using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using DndSpellbook.Views;
using DndSpellbook.Views.SpellLists;
using ReactiveUI;
using Splat;
using CharactersView = DndSpellbook.Views.CharactersView;
using CharacterView = DndSpellbook.Views.CharacterView;

namespace DndSpellbook;

public class ViewLocator : IDataTemplate, IViewLocator
{
    public Control? Build(object? data)
    {
        if (data is null)
            return null;

        var name = data.GetType().FullName!.Replace("ViewModel", "View", StringComparison.Ordinal);
        var type = Type.GetType(name);

        if (type != null)
        {
            var control = (Control)Activator.CreateInstance(type)!;
            control.DataContext = data;
            return control;
        }

        return new TextBlock { Text = "Not Found: " + name };
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
    
    public IViewFor? ResolveView<T>(T? viewModel, string? contract = null)
    {
        return viewModel switch
        {
            SpellsViewModel => new SpellsView { DataContext = viewModel },
            SpellListsViewModel => new SpellListsView { DataContext = viewModel },
            SpellListViewModel => new SpellListView { DataContext = viewModel },
            CharactersViewModel => new CharactersView { DataContext = viewModel },
            CharacterViewModel => new CharacterView { DataContext = viewModel },
            _ => null
        };
    }
}
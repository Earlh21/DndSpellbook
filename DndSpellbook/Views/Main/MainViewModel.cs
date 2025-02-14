using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Interactivity;
using DndSpellbook.Navigation;
using ReactiveUI;

namespace DndSpellbook.Views;

public class MainViewModel : ReactiveObject, IScreen
{
    private readonly Navigator navigator;

    public RoutingState Router { get; } = new();
    
    public ReactiveCommand<string, Unit> NavigateCommand { get; }

    public MainViewModel(Navigator navigator)
    {
        this.navigator = navigator;
        
        NavigateCommand = ReactiveCommand.Create<string>(Navigate);
    }
    
    private void Navigate(string pageName)
    {
        switch (pageName)
        {
            case "spells":
                var spellsViewModel = navigator.BuildSpellsViewModel();
                Router.NavigateAndReset.Execute(spellsViewModel);
                break;
            case "characters":
                var charactersViewModel = navigator.BuildCharactersViewModel();
                Router.NavigateAndReset.Execute(charactersViewModel);
                break;
        }
    }
}
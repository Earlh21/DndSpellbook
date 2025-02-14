using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Interactivity;
using DndSpellbook.Navigation;
using ReactiveUI;

namespace DndSpellbook.Views;

public class MainViewModel : ReactiveObject, IScreen
{
    private readonly Navigator navigator;

    public RoutingState Router { get; }
    
    public ReactiveCommand<string, Unit> NavigateCommand { get; }

    public MainViewModel(RoutingState router, Navigator navigator)
    {
        Router = router;
        this.navigator = navigator;
        
        NavigateCommand = ReactiveCommand.Create<string>(Navigate);
    }
    
    private void Navigate(string pageName)
    {
        switch (pageName)
        {
            case "spells":
                var spellsViewModel = navigator.BuildSpellsViewModel();
                navigator.NavigateAndReset(spellsViewModel);
                break;
            case "characters":
                var charactersViewModel = navigator.BuildCharactersViewModel();
                navigator.NavigateAndReset(charactersViewModel);
                break;
            case "spelllists":
                var spellListsViewModel = navigator.BuildSpellListsViewModel();
                navigator.NavigateAndReset(spellListsViewModel);
                break;
        }
    }
}
using System;
using DndSpellbook.Data.Services;
using DndSpellbook.Views;
using DndSpellbook.Views.SpellLists;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace DndSpellbook.Navigation;

public class Navigator
{
    private RoutingState router;
    
    public IServiceProvider ServiceProvider { get; internal set; }

    public Navigator(RoutingState router)
    {
        this.router = router;
    }
    
    public event EventHandler<object?> NavigatedBack;

    public void Navigate(IRoutableViewModel viewModel)
    {
        router.Navigate.Execute(viewModel);
    }

    public void NavigateAndReset(IRoutableViewModel viewModel)
    {
        router.NavigateAndReset.Execute(viewModel);
    }

    public void NavigateBack(object? param)
    {
        router.NavigateBack.Execute();
        NavigatedBack?.Invoke(this, param);
    }
    
    public SpellsViewModel BuildSpellsViewModel(bool isSelector = false)
    {
        var scope = ServiceProvider.CreateScope();

        var spellService = scope.ServiceProvider.GetService<SpellService>()!;
        
        return new SpellsViewModel(spellService, isSelector);
    }
    
    public CharactersViewModel BuildCharactersViewModel()
    {
        var scope = ServiceProvider.CreateScope();

        var characterService = scope.ServiceProvider.GetService<CharacterService>()!;
        
        return new CharactersViewModel(this, characterService);
    }
    
    public CharacterViewModel BuildCharacterViewModel(int characterId)
    {
        var scope = ServiceProvider.CreateScope();

        var characterService = scope.ServiceProvider.GetService<CharacterService>()!;
        var spellService = scope.ServiceProvider.GetService<SpellService>()!;
        
        return new CharacterViewModel(this, characterService, spellService, characterId);
    }
    
    public SpellListsViewModel BuildSpellListsViewModel()
    {
        var scope = ServiceProvider.CreateScope();

        var spellListService = scope.ServiceProvider.GetService<SpellListService>()!;
        
        return new SpellListsViewModel(this, spellListService);
    }
    
    public SpellListViewModel BuildSpellListViewModel(int spellListId)
    {
        var scope = ServiceProvider.CreateScope();

        var spellListService = scope.ServiceProvider.GetService<SpellListService>()!;
        var spellService = scope.ServiceProvider.GetService<SpellService>()!;
        
        return new SpellListViewModel(this, spellService, spellListService, spellListId);
    }
}
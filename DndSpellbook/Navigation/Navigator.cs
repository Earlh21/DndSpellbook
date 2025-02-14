using System;
using DndSpellbook.Data.Services;
using DndSpellbook.Views;
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

        var screen = scope.ServiceProvider.GetService<IScreen>()!;
        var spellService = scope.ServiceProvider.GetService<SpellService>()!;
        
        return new SpellsViewModel(screen, spellService, isSelector);
    }
    
    public CharactersViewModel BuildCharactersViewModel()
    {
        var scope = ServiceProvider.CreateScope();

        var screen = scope.ServiceProvider.GetService<IScreen>()!;
        var characterService = scope.ServiceProvider.GetService<CharacterService>()!;
        
        return new CharactersViewModel(screen, this, characterService);
    }
    
    public CharacterViewModel BuildCharacterViewModel(int characterId)
    {
        var scope = ServiceProvider.CreateScope();

        var screen = scope.ServiceProvider.GetService<IScreen>()!;
        var characterService = scope.ServiceProvider.GetService<CharacterService>()!;
        var spellService = scope.ServiceProvider.GetService<SpellService>()!;
        
        return new CharacterViewModel(screen, this, characterService, spellService, characterId);
    }
}
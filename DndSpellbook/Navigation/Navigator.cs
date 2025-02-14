using System;
using DndSpellbook.Data.Services;
using DndSpellbook.Views;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace DndSpellbook.Navigation;

public class Navigator
{
    public IServiceProvider ServiceProvider { get; internal set; }
    public RoutingState Router { get; internal set; }
    
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
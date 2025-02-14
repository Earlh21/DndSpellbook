using System;
using DndSpellbook.Data.Services;
using DndSpellbook.Views;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace DndSpellbook.ViewModels;

public class ViewModelBuilder
{
    public IServiceProvider ServiceProvider { get; internal set; }
    
    public SpellsViewModel BuildSpellsViewModel()
    {
        var scope = ServiceProvider.CreateScope();

        var screen = scope.ServiceProvider.GetService<IScreen>()!;
        var spellService = scope.ServiceProvider.GetService<SpellService>()!;
        
        return new SpellsViewModel(screen, spellService);
    }
}
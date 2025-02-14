using DndSpellbook.Data.Services;
using DndSpellbook.ViewModels;
using ReactiveUI;
using Splat;

namespace DndSpellbook.Views;

public class SpellsViewModel : ViewModelBase
{
    private readonly SpellService spellService;
    
    public override string UrlPathSegment => "spells";
    public override IScreen HostScreen { get; }
    
    public SpellsViewModel(IScreen hostScreen, SpellService spellService)
    {
        this.spellService = spellService;
        
        HostScreen = hostScreen;
    }
}
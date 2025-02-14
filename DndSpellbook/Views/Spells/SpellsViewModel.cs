using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using DndSpellbook.Data.Models;
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
    
    private ObservableCollection<SpellCardViewModel> spells = new();
    public ObservableCollection<SpellCardViewModel> Spells
    {
        get => spells;
        set => this.RaiseAndSetIfChanged(ref spells, value);
    }
    
    public ReactiveCommand<Unit, Unit> NewSpellCommand { get; }
    public ReactiveCommand<SpellCardViewModel, Unit> DeleteSpellCommand { get; }
    
    public SpellsViewModel(IScreen hostScreen, SpellService spellService)
    {
        this.spellService = spellService;
        
        NewSpellCommand = ReactiveCommand.CreateFromTask(NewSpell);
        DeleteSpellCommand = ReactiveCommand.CreateFromTask<SpellCardViewModel>(DeleteSpell);
        
        HostScreen = hostScreen;
    }

    public async Task LoadDataAsync()
    {
        var spells = await spellService.GetAllAsync();
        
        Spells = new (spells.Select(s => new SpellCardViewModel(s, spellService, DeleteSpellCommand)));
    }
    
    private async Task DeleteSpell(SpellCardViewModel spell)
    {
        await spellService.DeleteAsync(spell.Spell);
        Spells.Remove(spell);
    }
    
    private async Task NewSpell()
    {
        var spell = new Spell("Name");
        var spellCard = new SpellCardViewModel(spell, spellService, DeleteSpellCommand);
        Spells.Add(spellCard);
        
        await spellService.AddAsync(spell);
    }
}
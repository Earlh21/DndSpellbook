using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using DndSpellbook.Data.Models;
using DndSpellbook.Data.Services;
using DndSpellbook.Navigation;
using ReactiveUI;
using SQLitePCL;

namespace DndSpellbook.Views.SpellLists;

public class SpellListViewModel : ViewModelBase
{
    private readonly Navigator navigator;
    private readonly SpellListService spellListService;
    private readonly SpellService spellService;
    private readonly int spellListId;
    
    public Interaction<SpellsViewModel, IEnumerable<int>?> AddSpellsInteraction { get; } = new();
    
    private SpellList? spellList;
    public SpellList? SpellList
    {
        get => spellList;
        set => this.RaiseAndSetIfChanged(ref spellList, value);
    }
    
    public ReactiveCommand<Spell, Task> RemoveSpellCommand { get; }
    public ReactiveCommand<Unit, Unit> AddSpellsCommand { get; }
    
    public SpellListViewModel(Navigator navigator, SpellService spellService, SpellListService spellListService, int spellListId)
    {
        this.navigator = navigator;
        this.spellListService = spellListService;
        this.spellService = spellService;
        
        this.spellListId = spellListId;
        
        RemoveSpellCommand = ReactiveCommand.Create<Spell, Task>(RemoveSpell);
        AddSpellsCommand = ReactiveCommand.CreateFromTask(AddSpells);
    }
    
    public async Task LoadDataAsync()
    {
        SpellList = await spellListService.GetByIdAsync(spellListId);

        if (SpellList == null) return;
        SpellList.PropertyChanged += async (_, _) => await spellListService.UpdateAsync(SpellList);
    }

    private async Task RemoveSpell(Spell spell)
    {
        if (SpellList == null) return;
        
        SpellList.Spells.Remove(spell);
        await spellListService.UpdateAsync(SpellList);
    }

    private async Task AddSpells()
    {
        if (SpellList == null) return;
        
        var vm = navigator.BuildSpellsViewModel(true);
        var result = await AddSpellsInteraction.Handle(vm).FirstAsync();

        if (result == null) return;
        
        var newSpells = result.Except(SpellList.Spells.Select(s => s.Id));
        var spells = await spellService.GetByIdsAsync(newSpells);
        
        foreach (var spell in spells)
        {
            SpellList.Spells.Add(spell);
        }
        
        await spellListService.UpdateAsync(SpellList);
    }
}
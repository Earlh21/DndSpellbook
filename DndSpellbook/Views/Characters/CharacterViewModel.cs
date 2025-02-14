using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using DndSpellbook.Data.Models;
using DndSpellbook.Data.Services;
using DndSpellbook.Navigation;
using DynamicData;
using ReactiveUI;

namespace DndSpellbook.Views;

public class CharacterViewModel : ViewModelBase
{
    private readonly Navigator navigator;
    private readonly CharacterService characterService;
    private readonly SpellService spellService;
    private readonly int characterId;
    
    private Character? character;
    public Character? Character
    {
        get => character;
        set => this.RaiseAndSetIfChanged(ref character, value);
    }
    
    public Interaction<SpellsViewModel, IEnumerable<int>?> AddSpellsInteraction { get; } = new();
    
    public ReactiveCommand<Unit, Unit> AddSpellsCommand { get; }
    public ReactiveCommand<SpellEntry, Unit> DeleteSpellCommand { get; }
    
    public CharacterViewModel(Navigator navigator, CharacterService characterService, SpellService spellService, int characterId)
    {
        this.navigator = navigator;
        this.characterId = characterId;
        this.characterService = characterService;
        this.spellService = spellService;
        
        AddSpellsCommand = ReactiveCommand.CreateFromTask(AddSpells);
        DeleteSpellCommand = ReactiveCommand.CreateFromTask<SpellEntry>(DeleteSpell);
    }
    
    public async Task LoadDataAsync()
    {
        Character = await characterService.GetByIdAsync(characterId);
        if (Character == null) return;
        
        Character.PropertyChanged += async (sender, args) =>
        {
            await characterService.UpdateAsync(Character);
        };
    }

    private async Task DeleteSpell(SpellEntry spellEntry)
    {
        if(Character == null) return;
        
        Character.Spells.Remove(spellEntry);
        await characterService.UpdateAsync(Character);
    }

    private async Task AddSpells()
    {
        if (Character == null) return;
        
        var vm = navigator.BuildSpellsViewModel(true);
        var result = await AddSpellsInteraction.Handle(vm).FirstAsync();

        if (result == null) return;

        var newSpells = result.Except(Character.Spells.Select(s => s.SpellId));
        var spells = await spellService.GetByIdsAsync(newSpells);
        
        Character.Spells.AddRange(spells.Select(spell => new SpellEntry(spell, Character)));
        await characterService.UpdateAsync(Character);
    }
}
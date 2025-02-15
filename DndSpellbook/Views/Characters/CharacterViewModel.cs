using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using DndSpellbook.Data.Models;
using DndSpellbook.Data.Models.Enums;
using DndSpellbook.Data.Services;
using DndSpellbook.Extensions;
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
    public ReactiveCommand<SpellEntry, Unit> RemoveEntryCommand { get; }
    
    public ObservableCollection<SpellEntryEditor> SpellEntries { get; } = new();
    
    public RechargeType[] RechargeTypes { get; } = Enum.GetValues<RechargeType>();
    
    public CharacterViewModel(Navigator navigator, CharacterService characterService, SpellService spellService, int characterId)
    {
        this.navigator = navigator;
        this.characterId = characterId;
        this.characterService = characterService;
        this.spellService = spellService;
        
        AddSpellsCommand = ReactiveCommand.CreateFromTask(AddSpells);
        RemoveEntryCommand = ReactiveCommand.CreateFromTask<SpellEntry>(RemoveEntry);
    }
    
    public async Task LoadDataAsync()
    {
        Character = await characterService.GetByIdAsync(characterId);
        if (Character == null) return;

        Character.SubscribeToAllChanges(() => characterService.UpdateAsync(Character));

        Character.Spells.CollectionChanged += (_, args) =>
        {
            if (args.NewItems == null) return;

            foreach (var newItem in args.NewItems)
            {
                if (newItem is not SpellEntry spellEntry) continue;

                spellEntry.SubscribeToAllChanges(() => characterService.UpdateAsync(Character));
            }
        };
        
        foreach (var spellEntry in Character.Spells)
        {
            spellEntry.SubscribeToAllChanges(() => characterService.UpdateAsync(Character));
        }
        
        SpellEntries.AddRange(Character.Spells.Select(spell => new SpellEntryEditor(spell)));
    }

    private async Task RemoveEntry(SpellEntry spellEntry)
    {
        if(Character == null) return;
        
        Character.Spells.Remove(spellEntry);
        SpellEntries.Remove(SpellEntries.First(s => s.Entry == spellEntry));
        await characterService.UpdateAsync(Character);
    }

    private async Task AddSpells()
    {
        if (Character == null) return;
        
        var vm = navigator.BuildSpellsViewModel(true);
        var result = await AddSpellsInteraction.Handle(vm).FirstAsync();

        if (result == null) return;

        var spells = await spellService.GetByIdsAsync(result);
        
        Character.Spells.AddRange(spells.Select(spell => new SpellEntry(spell, Character)));
        SpellEntries.AddRange(spells.Select(spell => new SpellEntryEditor(new SpellEntry(spell, Character))));
        await characterService.UpdateAsync(Character);
    }
}

public class SpellEntryEditor : ReactiveObject
{
    public SpellEntry Entry { get; }

    private bool rechargeIsChecked;
    public bool RechargeIsChecked
    {
        get => rechargeIsChecked;
        set => this.RaiseAndSetIfChanged(ref rechargeIsChecked, value);
    }

    public SpellEntryEditor(SpellEntry spellEntry)
    {
        Entry = spellEntry;
        RechargeIsChecked = spellEntry.Uses.Recharge != null;

        this.WhenAnyValue(x => x.RechargeIsChecked).Do(val =>
        {
            if(val && Entry.Uses.Recharge == null)
            {
                Entry.Uses.Recharge = new(RechargeType.ShortRest, 1);
            }
            else if(!val && Entry.Uses.Recharge != null)
            {
                Entry.Uses.Recharge = null;
            }
        }).Subscribe();
    }
}
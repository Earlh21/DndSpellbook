using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using DndSpellbook.Controls;
using DndSpellbook.Data.Models;
using DndSpellbook.Data.Models.Enums;
using DndSpellbook.Data.Services;
using DndSpellbook.Extensions;
using DndSpellbook.Navigation;
using DndSpellbook.Util;
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

    private bool showEntriesAsCards;
    public bool ShowEntriesAsCards
    {
        get => showEntriesAsCards;
        set => this.RaiseAndSetIfChanged(ref showEntriesAsCards, value);
    }
    
    public Interaction<SpellsViewModel, IEnumerable<int>?> AddSpellsInteraction { get; } = new();
    
    public ReactiveCommand<Unit, Unit> AddSpellsCommand { get; }
    public ReactiveCommand<SpellEntry, Unit> RemoveEntryCommand { get; }

    private FilteredCollection<SpellEntryEditor> spellEntries;
    public FilteredCollection<SpellEntryEditor> SpellEntries
    {
        get => spellEntries;
        set => this.RaiseAndSetIfChanged(ref spellEntries, value);
    }
    
    public RechargeType[] RechargeTypes { get; } = Enum.GetValues<RechargeType>();
    
    public CharacterViewModel(Navigator navigator, CharacterService characterService, SpellService spellService, int characterId)
    {
        this.navigator = navigator;
        this.characterId = characterId;
        this.characterService = characterService;
        this.spellService = spellService;
        
        AddSpellsCommand = ReactiveCommand.CreateFromTask(AddSpells);
        RemoveEntryCommand = ReactiveCommand.CreateFromTask<SpellEntry>(RemoveEntry);

        spellEntries = new(se => se.Entry.Id, new SpellEntryComparer(), null);
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

                spellEntry.SubscribeToAllChanges(() => EntryChanged(spellEntry));
            }
        };
        
        foreach (var spellEntry in Character.Spells)
        {
            spellEntry.SubscribeToAllChanges(() => EntryChanged(spellEntry));
        }

        SpellEntries.ReplaceAll(Character.Spells.Select(spell => new SpellEntryEditor(spell)));
    }

    private async Task EntryChanged(SpellEntry entry)
    {
        if (Character == null) return;
        
        var editor = SpellEntries.AllItems.FirstOrDefault(s => s.Entry == entry);
        if (editor == null) return;
        
        var saveTask = characterService.UpdateAsync(Character);
        
        //SpellEntries.AddOrUpdate(editor);
        
        await saveTask;
    }

    private async Task RemoveEntry(SpellEntry spellEntry)
    {
        if(Character == null) return;
        
        Character.Spells.Remove(spellEntry);
        SpellEntries.Remove(SpellEntries.AllItems.First(s => s.Entry == spellEntry));
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
        SpellEntries.AddOrUpdate(spells.Select(spell => new SpellEntryEditor(new SpellEntry(spell, Character))));
        await characterService.UpdateAsync(Character);
    }

    private class SpellEntryComparer : IComparer<SpellEntryEditor>, IComparer<SpellEntry>
    {
        public int Compare(SpellEntryEditor? x, SpellEntryEditor? y)
        {
            return Compare(x?.Entry, y?.Entry);
        }

        public int Compare(SpellEntry? x, SpellEntry? y)
        {
            //Sort by prepared, then level, then name
            if (x == null && y == null) return 0;
            if (x == null) return -1;
            if (y == null) return 1;
            
            if (x.Prepared && !y.Prepared) return -1;
            if (!x.Prepared && y.Prepared) return 1;
            
            if (x.Spell.Level != y.Spell.Level) return x.Spell.Level.CompareTo(y.Spell.Level);
            
            return string.Compare(x.Spell.Name, y.Spell.Name, StringComparison.Ordinal);
        }
    }
}
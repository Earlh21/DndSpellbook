using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using DndSpellbook.Controls;
using DndSpellbook.Data.Models;
using DndSpellbook.Data.Models.Enums;
using DndSpellbook.Data.Services;
using DndSpellbook.Navigation;
using DndSpellbook.Util;
using DndSpellbook.Util.Extensions;
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
    public ReactiveCommand<Unit, Unit> LongRestCommand { get; }

    private FilteredCollection<SpellEntryEditor> spellEntryEditors;
    public FilteredCollection<SpellEntryEditor> SpellEntryEditors
    {
        get => spellEntryEditors;
        set => this.RaiseAndSetIfChanged(ref spellEntryEditors, value);
    }
    
    public CharacterViewModel(Navigator navigator, CharacterService characterService, SpellService spellService, int characterId)
    {
        this.navigator = navigator;
        this.characterId = characterId;
        this.characterService = characterService;
        this.spellService = spellService;
        
        AddSpellsCommand = ReactiveCommand.CreateFromTask(AddSpells);
        RemoveEntryCommand = ReactiveCommand.CreateFromTask<SpellEntry>(RemoveEntry);
        LongRestCommand = ReactiveCommand.Create(LongRest);

        spellEntryEditors = new(se => se.Entry.Id, new SpellEntryComparer(), null);
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

        SpellEntryEditors.ReplaceAll(Character.Spells.Select(spell => new SpellEntryEditor(spell)));
    }

    private async Task EntryChanged(SpellEntry entry)
    {
        if (Character == null) return;

        var editor = SpellEntryEditors.Get(entry.Id);
        if (editor == null) return;
        
        var saveTask = characterService.UpdateAsync(Character);
        
        SpellEntryEditors.AddOrUpdate(editor);
        
        await saveTask;
    }

    private async Task RemoveEntry(SpellEntry spellEntry)
    {
        if(Character == null) return;
        
        Character.Spells.Remove(spellEntry);
        SpellEntryEditors.RemoveKey(spellEntry.Id);
        await characterService.UpdateAsync(Character);
    }

    private async Task AddSpells()
    {
        if (Character == null) return;
        
        var vm = navigator.BuildSpellsViewModel(true);
        var result = await AddSpellsInteraction.Handle(vm).FirstAsync();

        if (result == null) return;

        var spells = await spellService.GetByIdsAsync(result);
        var newEntries = spells.Select(spell => new SpellEntry(spell, Character)).ToArray();
        
        Character.Spells.AddRange(newEntries);
        await characterService.UpdateAsync(Character);
        
        SpellEntryEditors.AddOrUpdate(newEntries.Select(spellEntry => new SpellEntryEditor(spellEntry)));
    }

    private void LongRest()
    {
        if (Character == null) return;
        
        Character.SpellSlots.Level1Used = 0;
        Character.SpellSlots.Level2Used = 0;
        Character.SpellSlots.Level3Used = 0;
        Character.SpellSlots.Level4Used = 0;
        Character.SpellSlots.Level5Used = 0;
        Character.SpellSlots.Level6Used = 0;
        Character.SpellSlots.Level7Used = 0;
        Character.SpellSlots.Level8Used = 0;
        Character.SpellSlots.Level9Used = 0;
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
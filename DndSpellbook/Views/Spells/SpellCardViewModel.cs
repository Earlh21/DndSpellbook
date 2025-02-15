using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using DndSpellbook.Data.Models;
using DndSpellbook.Data.Models.Enums;
using DndSpellbook.Data.Services;
using ReactiveUI;

namespace DndSpellbook.Views;

public class SpellCardViewModel : ReactiveObject
{
    private readonly SpellService spellService;
    private readonly SpellList[] allSpellLists;

    private bool isSelected;

    public bool IsSelected
    {
        get => isSelected;
        set => this.RaiseAndSetIfChanged(ref isSelected, value);
    }

    private bool isSelector;

    public bool IsSelector
    {
        get => isSelector;
        set => this.RaiseAndSetIfChanged(ref isSelector, value);
    }

    private Spell spell;

    public Spell Spell
    {
        get => spell;
        set => this.RaiseAndSetIfChanged(ref spell, value);
    }

    private SpellEditor? spellEditor = null;

    public SpellEditor? SpellEditor
    {
        get => spellEditor;
        set => this.RaiseAndSetIfChanged(ref spellEditor, value);
    }

    readonly ObservableAsPropertyHelper<bool> isEditing;
    public bool IsEditing => isEditing.Value;

    public SpellSchool[] Schools { get; }
    public CastingTime[] CastingTimes { get; }

    public ReactiveCommand<SpellCardViewModel, Unit> DeleteCommand { get; }

    public ReactiveCommand<Unit, Unit> EditCommand { get; }
    public ReactiveCommand<Unit, Unit> SaveCommand { get; }
    public ReactiveCommand<Unit, Unit> CancelCommand { get; }

    public SpellCardViewModel(
        Spell spell,
        SpellList[] allSpellLists,
        SpellService spellService,
        bool asSelector,
        ReactiveCommand<SpellCardViewModel, Unit> deleteCommand)
    {
        this.spell = spell;
        this.allSpellLists = allSpellLists;

        this.spellService = spellService;
        IsSelector = asSelector;
        DeleteCommand = deleteCommand;

        isEditing = this.WhenAnyValue(x => x.SpellEditor)
            .Select(e => e != null)
            .ToProperty(this, x => x.IsEditing);

        Schools = Enum.GetValues<SpellSchool>();
        CastingTimes = Enum.GetValues<CastingTime>();

        EditCommand = ReactiveCommand.Create(
            Edit,
            this.WhenAnyValue(x => x.IsEditing, e => !e)
        );

        SaveCommand = ReactiveCommand.CreateFromTask(
            Save,
            this.WhenAnyValue(x => x.IsEditing)
        );

        CancelCommand = ReactiveCommand.Create(
            Cancel,
            this.WhenAnyValue(x => x.IsEditing)
        );
    }

    private void Edit()
    {
        SpellEditor = new SpellEditor(Spell.Clone(), allSpellLists);
    }

    private async Task Save()
    {
        if (SpellEditor == null) return;

        Spell.CopyFrom(SpellEditor.EditCopy);
        await spellService.UpdateAsync(Spell);
        SpellEditor = null;
    }

    private void Cancel()
    {
        if (SpellEditor == null) return;
        SpellEditor = null;
    }
}

public class SpellEditor : ReactiveObject
{
    public Spell EditCopy { get; }
    public SpellListEntry[] SpellListEntries { get; }
    //public SpellList[] OriginalSpellLists { get; }

    public SpellEditor(Spell originalSpell, IEnumerable<SpellList> spellLists)
    {
        EditCopy = originalSpell.Clone();
        //OriginalSpellLists = originalSpell.SpellLists.ToArray();

        SpellListEntries = spellLists.Select(sl => new SpellListEntry(sl, EditCopy.SpellLists.Contains(sl)))
            .ToArray();

        foreach (var entry in SpellListEntries)
        {
            entry.WhenAnyValue(x => x.IsSelected)
                .Subscribe(isSelected =>
                {
                    if (isSelected)
                    {
                        if (!EditCopy.SpellLists.Contains(entry.SpellList))
                        {
                            EditCopy.SpellLists.Add(entry.SpellList);
                        }
                    }
                    else
                    {
                        EditCopy.SpellLists.Remove(entry.SpellList);
                    }
                });
        }
    }
}

public class SpellListEntry(SpellList spellList, bool isSelected) : ReactiveObject
{
    public SpellList SpellList { get; } = spellList;

    private bool isSelected = isSelected;

    public bool IsSelected
    {
        get => isSelected;
        set => this.RaiseAndSetIfChanged(ref isSelected, value);
    }
}
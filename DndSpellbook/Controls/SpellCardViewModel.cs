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

namespace DndSpellbook.Controls;

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
    public CastingTimeType[] CastingTimeTypes { get; }
    public RangeType[] RangeTypes { get; }

    public ReactiveCommand<Unit, Unit> EditCommand { get; }
    public ReactiveCommand<Spell, Unit> DeleteCommand { get; }
    public ReactiveCommand<Unit, Unit> SaveCommand { get; }
    public ReactiveCommand<Unit, Unit> CancelCommand { get; }
    
    private ReactiveCommand<SpellCardViewModel, Unit> deleteCommand;

    public SpellCardViewModel(
        Spell spell,
        SpellList[] allSpellLists,
        SpellService spellService,
        bool asSelector,
        ReactiveCommand<SpellCardViewModel, Unit> deleteCommand)
    {
        Spell = spell;
        Spell.Range = Spell.Range;
        IsSelector = asSelector;
        
        this.allSpellLists = allSpellLists;
        this.spellService = spellService;
        this.deleteCommand = deleteCommand;

        isEditing = this.WhenAnyValue(x => x.SpellEditor)
            .Select(e => e != null)
            .ToProperty(this, x => x.IsEditing);

        Schools = Enum.GetValues<SpellSchool>();
        CastingTimeTypes = Enum.GetValues<CastingTimeType>();
        RangeTypes = Enum.GetValues<RangeType>();

        EditCommand = ReactiveCommand.Create(
            Edit,
            this.WhenAnyValue(x => x.IsEditing, e => !e)
        );
        
        DeleteCommand = ReactiveCommand.Create<Spell>(
            Delete,
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

    private void Delete(Spell _)
    {
        deleteCommand.Execute(this);
    }

    private void Edit()
    {
        SpellEditor = new(Spell.Clone(), allSpellLists);
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
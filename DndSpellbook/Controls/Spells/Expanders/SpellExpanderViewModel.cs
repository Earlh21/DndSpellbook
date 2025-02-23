using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using DndSpellbook.Data.Models;
using DndSpellbook.Data.Models.Enums;
using DndSpellbook.Data.Services;
using ReactiveUI;

namespace DndSpellbook.Controls;

public class SpellExpanderViewModel : ReactiveObject
{
    private readonly SpellService spellService;
    private readonly SpellList[] allSpellLists;
    
    public bool IsSelector { get; }

    private bool isSelected;

    public bool IsSelected
    {
        get => isSelected;
        set => this.RaiseAndSetIfChanged(ref isSelected, value);
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

    public ReactiveCommand<Unit, Unit> EditCommand { get; }
    public ReactiveCommand<Spell, Unit> DeleteCommand { get; }
    public ReactiveCommand<Unit, Unit> SaveCommand { get; }
    public ReactiveCommand<Unit, Unit> CancelCommand { get; }

    public SpellExpanderViewModel(
        Spell spell,
        SpellList[] allSpellLists,
        SpellService spellService,
        ReactiveCommand<Spell, Unit> deleteCommand,
        bool asSelector = false)
    {
        Spell = spell;
        Spell.Range = Spell.Range;
        IsSelector = asSelector;
        
        this.allSpellLists = allSpellLists;
        this.spellService = spellService;
        DeleteCommand = deleteCommand;

        isEditing = this.WhenAnyValue(x => x.SpellEditor)
            .Select(e => e != null)
            .ToProperty(this, x => x.IsEditing);

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
using System;
using System.Reactive;
using System.Threading.Tasks;
using DndSpellbook.Data.Models;
using DndSpellbook.Data.Models.Enums;
using DndSpellbook.Data.Services;
using ReactiveUI;

namespace DndSpellbook.Views;

public class SpellCardViewModel : ReactiveObject
{
    private readonly SpellService spellService;

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

    private Spell editCopy = new("...");

    public Spell EditCopy
    {
        get => editCopy;
        set => this.RaiseAndSetIfChanged(ref editCopy, value);
    }

    private bool isEditing;

    public bool IsEditing
    {
        get => isEditing;
        set => this.RaiseAndSetIfChanged(ref isEditing, value);
    }
    
    

    public SpellSchool[] Schools { get; }
    public CastingTime[] CastingTimes { get; }

    public ReactiveCommand<SpellCardViewModel, Unit> DeleteCommand { get; }

    public ReactiveCommand<Unit, Unit> EditCommand { get; }
    public ReactiveCommand<Unit, Unit> SaveCommand { get; }
    public ReactiveCommand<Unit, Unit> CancelCommand { get; }

    public SpellCardViewModel(Spell spell,
        SpellService spellService,
        bool asSelector,
        ReactiveCommand<SpellCardViewModel, Unit> deleteCommand)
    {
        this.spell = spell;
        this.spellService = spellService;
        IsSelector = asSelector;
        DeleteCommand = deleteCommand;

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
        EditCopy.CopyFrom(Spell);
        IsEditing = true;
    }

    private async Task Save()
    {
        Spell.CopyFrom(EditCopy);
        await spellService.UpdateAsync(Spell);
        IsEditing = false;
    }

    private void Cancel()
    {
        IsEditing = false;
    }
}
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
    public RangeType[] RangeTypes { get; }

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
        Spell = spell;
        Spell.Range = Spell.Range;
        IsSelector = asSelector;
        
        this.allSpellLists = allSpellLists;
        this.spellService = spellService;
        DeleteCommand = deleteCommand;

        isEditing = this.WhenAnyValue(x => x.SpellEditor)
            .Select(e => e != null)
            .ToProperty(this, x => x.IsEditing);

        Schools = Enum.GetValues<SpellSchool>();
        CastingTimes = Enum.GetValues<CastingTime>();
        RangeTypes = Enum.GetValues<RangeType>();

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

    private bool maxRangeChecked;

    public bool MaxRangeChecked
    {
        get => maxRangeChecked;
        set => this.RaiseAndSetIfChanged(ref maxRangeChecked, value);
    }

    private bool minRangeChecked;

    public bool MinRangeChecked
    {
        get => minRangeChecked;
        set => this.RaiseAndSetIfChanged(ref minRangeChecked, value);
    }

    private bool longRangeChecked;

    public bool LongRangeChecked
    {
        get => longRangeChecked;
        set => this.RaiseAndSetIfChanged(ref longRangeChecked, value);
    }

    private bool areaRadiusChecked;

    public bool AreaRadiusChecked
    {
        get => areaRadiusChecked;
        set => this.RaiseAndSetIfChanged(ref areaRadiusChecked, value);
    }

    private int minRange;

    public int MinRange
    {
        get => minRange;
        set => this.RaiseAndSetIfChanged(ref minRange, value);
    }

    private int maxRange;

    public int MaxRange
    {
        get => maxRange;
        set => this.RaiseAndSetIfChanged(ref maxRange, value);
    }

    private int longRange;

    public int LongRange
    {
        get => longRange;
        set => this.RaiseAndSetIfChanged(ref longRange, value);
    }

    private int areaRadius;

    public int AreaRadius
    {
        get => areaRadius;
        set => this.RaiseAndSetIfChanged(ref areaRadius, value);
    }

    public SpellEditor(Spell originalSpell, IEnumerable<SpellList> spellLists)
    {
        EditCopy = originalSpell.Clone();

        MinRange = EditCopy.Range.MinRange ?? 0;
        MaxRange = EditCopy.Range.MaxRange ?? 0;
        LongRange = EditCopy.Range.LongRange ?? 0;
        AreaRadius = EditCopy.Range.AreaRadius ?? 0;

        MinRangeChecked = EditCopy.Range.MinRange.HasValue;
        MaxRangeChecked = EditCopy.Range.MaxRange.HasValue;
        LongRangeChecked = EditCopy.Range.LongRange.HasValue;
        AreaRadiusChecked = EditCopy.Range.AreaRadius.HasValue;

        this.WhenAnyValue(x => x.MinRange, x => x.MinRangeChecked)
            .Subscribe(_ => EditCopy.Range.MinRange = MinRangeChecked ? MinRange : null);

        this.WhenAnyValue(x => x.MaxRange, x => x.MaxRangeChecked)
            .Subscribe(_ => EditCopy.Range.MaxRange = MaxRangeChecked ? MaxRange : null);

        this.WhenAnyValue(x => x.LongRange, x => x.LongRangeChecked)
            .Subscribe(_ => EditCopy.Range.LongRange = LongRangeChecked ? LongRange : null);

        this.WhenAnyValue(x => x.AreaRadius, x => x.AreaRadiusChecked)
            .Subscribe(_ => EditCopy.Range.AreaRadius = AreaRadiusChecked ? AreaRadius : null);

        EditCopy.Range.WhenAnyValue(r => r.Type).Subscribe(r =>
        {
            switch (r)
            {
                case RangeType.Fixed:
                    MaxRangeChecked = true;
                    break;
                case RangeType.Ranged:
                    MaxRangeChecked = true;
                    LongRangeChecked = true;
                    break;
            }
        });

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
using System;
using System.Collections.Generic;
using System.Linq;
using DndSpellbook.Data.Models;
using DndSpellbook.Data.Models.Enums;
using ReactiveUI;

namespace DndSpellbook.Controls;

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

    public bool CastingTimeTypeIsTime
    {
        get => EditCopy.CastingTime.Type == CastingTimeType.Time;
        set
        {
            EditCopy.CastingTime.Type = value ? CastingTimeType.Time : CastingTimeType.Action;
            this.RaisePropertyChanged();
        }
    }

    public CastingTimeType CastingTimeTypeWithoutTime
    {
        get => EditCopy.CastingTime.Type == CastingTimeType.Time ? CastingTimeType.Action : EditCopy.CastingTime.Type;
        set
        {
            EditCopy.CastingTime.Type = value;
        }
    }

    public static CastingTimeType[] CastingTimeTypesWithoutTime { get; } = Enum.GetValues<CastingTimeType>().Where(x => x != CastingTimeType.Time).ToArray();

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

        EditCopy.CastingTime.WhenAnyValue(c => c.Type).Subscribe(t =>
        {
            if (t != CastingTimeType.Time)
            {
                EditCopy.CastingTime.Time = null;
                return;
            }

            if (EditCopy.CastingTime.Time == null)
            {
                EditCopy.CastingTime.Time = 60;
            }
            
            this.RaisePropertyChanged(nameof(CastingTimeTypeIsTime));
            this.RaisePropertyChanged(nameof(CastingTimeTypeWithoutTime));
        });

        EditCopy.CastingTime.WhenAnyValue(c => c.Time).Subscribe(t =>
        {
            if (EditCopy.CastingTime.Type == CastingTimeType.Time && t == null)
            {
                EditCopy.CastingTime.Time = 60;
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
using System;
using System.ComponentModel.DataAnnotations;
using DndSpellbook.Data.Models.Enums;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;

namespace DndSpellbook.Data.Models;

[Owned]
public class Range : ReactiveObject
{
    private RangeType type;

    public RangeType Type
    {
        get => type;
        set => this.RaiseAndSetIfChanged(ref type, value);
    }

    private int? minRange;

    public int? MinRange
    {
        get => minRange;
        set => this.RaiseAndSetIfChanged(ref minRange, value);
    }

    private int? maxRange;

    public int? MaxRange
    {
        get => maxRange;
        set => this.RaiseAndSetIfChanged(ref maxRange, value);
    }

    private int? longRange;

    public int? LongRange
    {
        get => longRange;
        set => this.RaiseAndSetIfChanged(ref longRange, value);
    }

    private int? areaRadius;

    public int? AreaRadius
    {
        get => areaRadius;
        set => this.RaiseAndSetIfChanged(ref areaRadius, value);
    }

    public string DisplayString => ToString();

    private Range()
    {
        this.WhenAnyValue(x => x.Type, x => x.MinRange, x => x.MaxRange, x => x.LongRange, x => x.AreaRadius)
            .Subscribe(_ => this.RaisePropertyChanged(nameof(DisplayString)));
    }

    public Range(RangeType type,
        int? minRange = null,
        int? maxRange = null,
        int? longRange = null,
        int? areaRadius = null) : this()
    {
        Type = type;
        MinRange = minRange;
        MaxRange = maxRange;
        LongRange = longRange;
        AreaRadius = areaRadius;
    }

    public override string ToString()
    {
        if (Type == RangeType.Fixed)
        {
            string range = MinRange == null ? $"Range: {MaxRange ?? 0} ft." : $"Range: {MinRange}-{MaxRange ?? 0} ft.";

            if (AreaRadius != null)
            {
                range += $"{Environment.NewLine}Area: {AreaRadius} ft. radius";
            }

            return range;
        }

        if (Type == RangeType.Melee) return "Melee";

        if (Type == RangeType.Ranged)
        {
            string range = MinRange == null
                ? $"Range: {MaxRange ?? 0} ({LongRange} long) ft."
                : $"Range: {MinRange}-{MaxRange ?? 0} ({LongRange} long) ft.";

            if (AreaRadius != null)
            {
                range += $"{Environment.NewLine}Area: {AreaRadius} ft. radius";
            }

            return range;
        }

        if (Type == RangeType.Self) return "Self";

        if (Type == RangeType.Sight) return "Sight";

        if (Type == RangeType.Touch) return "Touch";

        if (Type == RangeType.Unlimited) return "Unlimited";

        return "";
    }
}
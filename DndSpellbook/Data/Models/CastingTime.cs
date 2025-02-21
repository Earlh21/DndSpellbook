using System;
using DndSpellbook.Converters;
using DndSpellbook.Data.Models.Enums;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;

namespace DndSpellbook.Data.Models;

[Owned]
public class CastingTime : ReactiveObject
{
    private CastingTimeType type;
    public CastingTimeType Type
    {
        get => type;
        set => this.RaiseAndSetIfChanged(ref type, value);
    }

    private int? time;
    public int? Time
    {
        get => time;
        set => this.RaiseAndSetIfChanged(ref time, value);
    }

    public string DisplayString => ToString();

    private CastingTime()
    {
        this.WhenAnyValue(thisViewModel => thisViewModel.Type, x => x.Time)
            .Subscribe(_ => this.RaisePropertyChanged(nameof(DisplayString)));
    }

    public CastingTime(CastingTimeType type, int? time = null)
    {
        Type = type;
        Time = time;
    }

    public override string ToString()
    {
        if (Type == CastingTimeType.BonusAction)
        {
            return "Bonus Action";
        }
        
        if (Type == CastingTimeType.Action)
        {
            return "Action";
        }

        if (Type == CastingTimeType.Time)
        {
            if (Time == null) return "";
            
            return SecondsToDurationTextConverter.Convert(Time.Value);
        }

        return "";
    }
}
using System;
using System.Linq;
using DndSpellbook.Data.Models;
using DndSpellbook.Data.Models.Enums;
using ReactiveUI;

namespace DndSpellbook.Controls;

public class CastingTimeEditor : ReactiveObject
{
    public CastingTime CastingTime { get; }
    
    public bool TypeIsTime
    {
        get => CastingTime.Type == CastingTimeType.Time;
        set
        {
            CastingTime.Type = value ? CastingTimeType.Time : CastingTimeType.Action;
            this.RaisePropertyChanged();
        }
    }

    public bool TypeIsReaction
    {
        get => CastingTime.Type == CastingTimeType.Reaction;
        set
        {
            CastingTime.Type = value ? CastingTimeType.Reaction : CastingTimeType.Action;
            this.RaisePropertyChanged();
        }
    }
    
    public bool TypeIsActionBonusAction
    {
        get => CastingTime.Type == CastingTimeType.Action || CastingTime.Type == CastingTimeType.BonusAction;
        set
        {
            CastingTime.Type = value ? CastingTimeType.Action : CastingTimeType.Time;
            this.RaisePropertyChanged();
        }
    }

    public CastingTimeType TypeWithoutTimeReaction
    {
        get
        {
            if(CastingTime.Type == CastingTimeType.Time || CastingTime.Type == CastingTimeType.Reaction)
            {
                return CastingTimeType.Action;
            }
            
            return CastingTime.Type;
        }
        set
        {
            CastingTime.Type = value;
            this.RaisePropertyChanged();
        }
    }

    public static CastingTimeType[] TypesWithoutTimeReaction { get; } = [CastingTimeType.Action, CastingTimeType.BonusAction];
    
    public CastingTimeEditor(CastingTime castingTime)
    {
        CastingTime = castingTime;
        
        CastingTime.WhenAnyValue(c => c.Type).Subscribe(t =>
        {
            this.RaisePropertyChanged(nameof(TypeIsTime));
            this.RaisePropertyChanged(nameof(TypeWithoutTimeReaction));
            this.RaisePropertyChanged(nameof(TypeIsReaction));
            this.RaisePropertyChanged(nameof(TypeIsActionBonusAction));
            
            if (t != CastingTimeType.Reaction)
            {
                CastingTime.ReactionText = "";
            }
            
            if (t != CastingTimeType.Time)
            {
                CastingTime.Time = null;
                return;
            }

            CastingTime.Time ??= 60;
        });

        CastingTime.WhenAnyValue(c => c.Time).Subscribe(t =>
        {
            if (CastingTime.Type == CastingTimeType.Time && t == null)
            {
                CastingTime.Time = 60;
            }
        });
    }
}
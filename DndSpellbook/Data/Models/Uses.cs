using System;
using DndSpellbook.Data.Models.Enums;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;

namespace DndSpellbook.Data.Models;

[Owned]
public class Uses : ReactiveObject
{
    private bool spellSlot;

    public bool SpellSlot
    {
        get => spellSlot;
        set => this.RaiseAndSetIfChanged(ref spellSlot, value);
    }

    private Recharge? recharge;

    public Recharge? Recharge
    {
        get => recharge;
        set => this.RaiseAndSetIfChanged(ref recharge, value);
    }

    public string DisplayString => ToString();

    private Uses()
    {
        this.WhenAnyValue(x => x.SpellSlot, x => x.Recharge.MaxCharges, x => x.Recharge.CurrentUses)
            .Subscribe(_ => this.RaisePropertyChanged(nameof(DisplayString)));
    }

    public Uses(bool spellSlot, Recharge? recharge)
    {
        SpellSlot = spellSlot;
        Recharge = recharge;
    }

    public override string ToString()
    {
        string result = SpellSlot ? "Spell Slot" : "At Will";
        if (Recharge != null)
        {
            result += $"{Environment.NewLine}Uses: {Recharge.MaxCharges - Recharge.CurrentUses}/{Recharge.MaxCharges}";
            result += Recharge.Type switch
            {
                RechargeType.LongRest => " (Long Rest)",
                RechargeType.ShortRest => " (Short Rest)",
                _ => ""
            };
        }

        return result;
    }
}
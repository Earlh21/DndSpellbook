using System;
using System.Reactive.Linq;
using DndSpellbook.Data.Models;
using DndSpellbook.Data.Models.Enums;
using ReactiveUI;

namespace DndSpellbook.Controls;

public class SpellEntryEditor : ReactiveObject
{
    public SpellEntry Entry { get; }
    public RechargeType[] RechargeTypes { get; } = Enum.GetValues<RechargeType>(); 

    private bool rechargeIsChecked;
    public bool RechargeIsChecked
    {
        get => rechargeIsChecked;
        set => this.RaiseAndSetIfChanged(ref rechargeIsChecked, value);
    }

    public SpellEntryEditor(SpellEntry spellEntry)
    {
        Entry = spellEntry;
        RechargeIsChecked = spellEntry.Uses.Recharge != null;

        this.WhenAnyValue(x => x.RechargeIsChecked).Do(val =>
        {
            if(val && Entry.Uses.Recharge == null)
            {
                Entry.Uses.Recharge = new(RechargeType.ShortRest, 1);
            }
            else if(!val && Entry.Uses.Recharge != null)
            {
                Entry.Uses.Recharge = null;
            }
        }).Subscribe();
    }
}
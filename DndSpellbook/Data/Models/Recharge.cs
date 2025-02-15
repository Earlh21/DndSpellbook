using DndSpellbook.Data.Models.Enums;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;

namespace DndSpellbook.Data.Models;

[Owned]
public class Recharge : ReactiveObject
{
    private RechargeType type;
    public RechargeType Type
    {
        get => type;
        set => this.RaiseAndSetIfChanged(ref type, value);
    }
    
    private int? maxCharges;
    public int? MaxCharges
    {
        get => maxCharges;
        set => this.RaiseAndSetIfChanged(ref maxCharges, value);
    }
    
    private int currentUses = 0;
    public int CurrentUses
    {
        get => currentUses;
        set => this.RaiseAndSetIfChanged(ref currentUses, value);
    }
    
    private Recharge()
    {
        
    }
    
    public Recharge(RechargeType type, int maxCharges)
    {
        Type = type;
        MaxCharges = maxCharges;
    }
}
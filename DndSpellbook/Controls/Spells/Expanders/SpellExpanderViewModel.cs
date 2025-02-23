using DndSpellbook.Data.Models;
using ReactiveUI;

namespace DndSpellbook.Controls;

public class SpellExpanderViewModel : ReactiveObject
{
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
    
    public SpellExpanderViewModel(Spell spell, bool isSelector = false)
    {
        Spell = spell;
        IsSelector = isSelector;
    }
}
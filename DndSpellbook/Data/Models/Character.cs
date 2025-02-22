using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using ReactiveUI;

namespace DndSpellbook.Data.Models;

public partial class Character : ReactiveObject
{
    [Key]
    public int Id { get; set; }

    private string name = "";
    public string Name
    {
        get => name;
        set => this.RaiseAndSetIfChanged(ref name, value);
    }
    
    private SpellSlots spellSlots = new();
    public SpellSlots SpellSlots
    {
        get => spellSlots;
        set => this.RaiseAndSetIfChanged(ref spellSlots, value);
    }

    public ObservableCollection<SpellEntry> Spells { get; } = new();

    private Character() { }

    public Character(string name)
    {
        Name = name;
    }
}
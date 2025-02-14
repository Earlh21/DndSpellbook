using System.ComponentModel.DataAnnotations.Schema;
using ReactiveUI;

namespace DndSpellbook.Data.Models;

public class SpellEntry : ReactiveObject
{
    public int Id { get; set; }

    private bool prepared;
    public bool Prepared
    {
        get => prepared;
        set => this.RaiseAndSetIfChanged(ref prepared, value);
    }

    [ForeignKey(nameof(Spell))]
    public int SpellId { get; set; }

    private Spell spell;
    public Spell Spell
    {
        get => spell;
        set => this.RaiseAndSetIfChanged(ref spell, value);
    }

    [ForeignKey(nameof(Character))]
    public int CharacterId { get; set; }

    private Character character;
    public Character Character
    {
        get => character;
        set => this.RaiseAndSetIfChanged(ref character, value);
    }

    private SpellEntry() { }

    public SpellEntry(int spellId, int characterId)
    {
        SpellId = spellId;
        CharacterId = characterId;
    }

    public SpellEntry(Spell spell, Character character)
    {
        Spell = spell;
        Character = character;
    }
}
using System.Text.Json.Serialization;

namespace DndSpellbook.Data.Models.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SpellSchool
{
    Evocation,
    Divination,
    Illusion,
    Enchantment,
    Necromancy,
    Abjuration,
    Conjuration,
    Transmutation
}
using System.Text.Json.Serialization;

namespace DndSpellbook.Data.Models.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RangeType
{
    Fixed = 0,
    Melee = 1,
    Ranged = 2,
    Touch = 3,
    Self = 4,
    Sight = 5,
    Unlimited = 6,
}
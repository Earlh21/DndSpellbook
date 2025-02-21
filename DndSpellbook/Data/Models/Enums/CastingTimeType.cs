using System.Text.Json.Serialization;

namespace DndSpellbook.Data.Models.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CastingTimeType
{
    Action,
    BonusAction,
    Reaction,
    Time
}
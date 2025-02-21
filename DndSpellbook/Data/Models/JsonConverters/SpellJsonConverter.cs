using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using DndSpellbook.Data.Models;
using DndSpellbook.Data.Models.Enums;
using Range = DndSpellbook.Data.Models.Range;

public class SpellJsonConverter : JsonConverter<Spell>
{
    public override Spell Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Use JsonDocument to parse and then create a new Spell instance manually.
        using (var document = JsonDocument.ParseValue(ref reader))
        {
            var root = document.RootElement;

            // Create Spell with required property 'name'
            var name = root.GetProperty("name").GetString();
            var spell = new Spell(name);

            // Set properties if they exist
            if (root.TryGetProperty("level", out var levelProp))
                spell.Level = levelProp.GetInt32();

            if (root.TryGetProperty("school", out var schoolProp))
            {
                // Parse enum from string
                var schoolStr = schoolProp.GetString();
                if (Enum.TryParse<SpellSchool>(schoolStr, out var school))
                {
                    spell.School = school;
                }
            }

            if (root.TryGetProperty("castingTime", out var castingTimeProp))
            {
                spell.CastingTime = JsonSerializer.Deserialize<CastingTime>(castingTimeProp.GetRawText(), options);
            }

            if (root.TryGetProperty("range", out var rangeProp))
            {
                spell.Range = JsonSerializer.Deserialize<Range>(rangeProp.GetRawText(), options);
            }

            if (root.TryGetProperty("duration", out var durationProp))
                spell.Duration = durationProp.GetInt32();

            if (root.TryGetProperty("verbal", out var verbalProp))
                spell.Verbal = verbalProp.GetBoolean();

            if (root.TryGetProperty("somatic", out var somaticProp))
                spell.Somatic = somaticProp.GetBoolean();

            if (root.TryGetProperty("material", out var materialProp))
                spell.Material = materialProp.GetBoolean();

            if (root.TryGetProperty("description", out var descriptionProp))
                spell.Description = descriptionProp.GetString();

            return spell;
        }
    }

    public override void Write(Utf8JsonWriter writer, Spell value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString("name", value.Name);
        writer.WriteNumber("level", value.Level);
        writer.WriteString("school", value.School.ToString());
        writer.WritePropertyName("castingTime");
        JsonSerializer.Serialize(writer, value.CastingTime, options);
        writer.WritePropertyName("range");
        JsonSerializer.Serialize(writer, value.Range, options);
        writer.WriteNumber("duration", value.Duration);
        writer.WriteBoolean("verbal", value.Verbal);
        writer.WriteBoolean("somatic", value.Somatic);
        writer.WriteBoolean("material", value.Material);
        writer.WriteString("description", value.Description);
        writer.WriteEndObject();
    }
}

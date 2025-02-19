using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using DndSpellbook.Data.Models;
using DndSpellbook.Data.Services;

namespace DndSpellbook.Import;

public record ImportResult(Spell[] NewSpells, SpellList[] NewSpellLists, string[] Issues);

public class SpellImporter
{
    private readonly SpellService _spellService;
    private readonly SpellListService _spellListService;

    public SpellImporter(SpellService spellService, SpellListService spellListService)
    {
        _spellService = spellService;
        _spellListService = spellListService;
    }

    public async Task<ImportResult> ImportSpellsAsync(string json)
    {
        var newSpells = new List<Spell>();
        var newSpellLists = new List<SpellList>();
        var issues = new List<string>();

        JsonDocument document;
        try
        {
            document = JsonDocument.Parse(json);
        }
        catch (Exception ex)
        {
            issues.Add($"Failed to parse JSON: {ex.Message}");
            return new ImportResult(newSpells.ToArray(), newSpellLists.ToArray(), issues.ToArray());
        }

        if (document.RootElement.ValueKind != JsonValueKind.Array)
        {
            issues.Add("JSON root element is not an array.");
            return new ImportResult(newSpells.ToArray(), newSpellLists.ToArray(), issues.ToArray());
        }

        // First pass: extract all spell names and spell list names from the JSON.
        var (spellNames, spellListNames) = ExtractNames(document.RootElement, issues);

        // Do our expensive database calls only once each.
        var existingSpells = await _spellService.GetByNamesAsync(spellNames);
        var existingSpellNames = new HashSet<string>(existingSpells.Select(s => s.Name), StringComparer.OrdinalIgnoreCase);

        var existingSpellLists = await _spellListService.GetByNamesAsync(spellListNames);
        var existingSpellListDict = existingSpellLists.ToDictionary(sl => sl.Name, StringComparer.OrdinalIgnoreCase);

        // A dictionary to track newly created spell lists (to avoid duplicates).
        var newSpellListDict = new Dictionary<string, SpellList>(StringComparer.OrdinalIgnoreCase);

        // Process each spell entry.
        foreach (var element in document.RootElement.EnumerateArray())
        {
            ProcessSpellElement(
                element,
                existingSpellNames,
                existingSpellListDict,
                newSpellListDict,
                newSpells,
                newSpellLists,
                issues);
        }

        return new ImportResult(newSpells.ToArray(), newSpellLists.ToArray(), issues.ToArray());
    }

    /// <summary>
    /// Extracts the names of spells and spell lists from the JSON array.
    /// </summary>
    private (HashSet<string> spellNames, HashSet<string> spellListNames) ExtractNames(JsonElement root, List<string> issues)
    {
        var spellNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var spellListNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var element in root.EnumerateArray())
        {
            if (element.TryGetProperty("name", out var nameProp))
            {
                var name = nameProp.GetString();
                if (!string.IsNullOrWhiteSpace(name))
                {
                    spellNames.Add(name);
                }
                else
                {
                    issues.Add("Spell entry with missing or empty 'name' property.");
                }
            }
            else
            {
                issues.Add("Spell entry missing 'name' property.");
            }

            if (element.TryGetProperty("spell_lists", out var listsProp))
            {
                try
                {
                    foreach (var listElement in listsProp.EnumerateArray())
                    {
                        var listName = listElement.GetString();
                        if (!string.IsNullOrWhiteSpace(listName))
                        {
                            spellListNames.Add(listName);
                        }
                    }
                }
                catch (Exception ex)
                {
                    issues.Add($"Failed to process 'spell_lists' property: {ex.Message}");
                }
            }
        }

        return (spellNames, spellListNames);
    }

    /// <summary>
    /// Processes a single spell JSON element. Deserializes the spell (if it isn’t already in the DB),
    /// then processes its spell_lists property, associating any existing or newly created SpellLists.
    /// </summary>
    private void ProcessSpellElement(
        JsonElement element,
        HashSet<string> existingSpellNames,
        Dictionary<string, SpellList> existingSpellListDict,
        Dictionary<string, SpellList> newSpellListDict,
        List<Spell> newSpells,
        List<SpellList> newSpellLists,
        List<string> issues)
    {
        try
        {
            if (!element.TryGetProperty("name", out var nameProp))
            {
                issues.Add("Spell entry missing 'name' property.");
                return;
            }
            var spellName = nameProp.GetString();
            if (string.IsNullOrWhiteSpace(spellName))
            {
                issues.Add("Spell entry with empty 'name' property.");
                return;
            }
            if (existingSpellNames.Contains(spellName))
            {
                // Spell already exists in the DB—skip it.
                return;
            }

            Spell spell;
            try
            {
                spell = JsonSerializer.Deserialize<Spell>(element.GetRawText());
            }
            catch (Exception ex)
            {
                issues.Add($"Failed to deserialize spell '{spellName}': {ex.Message}");
                return;
            }

            // Process the spell_lists property (if any) for this spell.
            if (element.TryGetProperty("spell_lists", out var listsProp))
            {
                string[] listNames;
                try
                {
                    listNames = JsonSerializer.Deserialize<string[]>(listsProp.GetRawText()) ?? Array.Empty<string>();
                }
                catch (Exception ex)
                {
                    issues.Add($"Failed to deserialize spell_lists for spell '{spellName}': {ex.Message}");
                    listNames = Array.Empty<string>();
                }

                foreach (var listName in listNames)
                {
                    if (string.IsNullOrWhiteSpace(listName))
                        continue;

                    SpellList spellList;
                    if (existingSpellListDict.TryGetValue(listName, out spellList))
                    {
                        // Use existing SpellList from DB.
                    }
                    else if (newSpellListDict.TryGetValue(listName, out spellList))
                    {
                        // Already created a new SpellList during this import.
                    }
                    else
                    {
                        // Create a new SpellList.
                        spellList = new SpellList(listName);
                        newSpellListDict[listName] = spellList;
                        newSpellLists.Add(spellList);
                    }

                    // Associate the spell with this SpellList (both directions).
                    if (!spell.SpellLists.Contains(spellList))
                        spell.SpellLists.Add(spellList);
                    if (!spellList.Spells.Contains(spell))
                        spellList.Spells.Add(spell);
                }
            }

            newSpells.Add(spell);
        }
        catch (Exception ex)
        {
            issues.Add($"Error processing a spell entry: {ex.Message}");
        }
    }
}
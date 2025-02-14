using System.Collections.Generic;
using System.Threading.Tasks;
using DndSpellbook.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace DndSpellbook.Data.Services;

public class CharacterService(SpellbookContext context)
{
    public async Task<List<Character>> GetAllAsync()
    {
        return await context.Characters.ToListAsync();
    }

    public async Task LoadAsync(Character character)
    {
        await context.Entry(character).Collection(c => c.Spells).LoadAsync();
    }
    
    public async Task AddAsync(Character character)
    {
        character.Id = default;
        
        context.Characters.Add(character);
        await context.SaveChangesAsync();
    }
    
    public async Task UpdateAsync(Character character)
    {
        context.Characters.Update(character);
        await context.SaveChangesAsync();
    }
    
    public async Task DeleteAsync(Character character)
    {
        context.Characters.Remove(character);
        await context.SaveChangesAsync();
    }
}
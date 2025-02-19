﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DndSpellbook.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace DndSpellbook.Data.Services;

public class SpellService(SpellbookContext context)
{
    public async Task<List<Spell>> GetAllAsync()
    {
        return await context.Spells
            .Include(s => s.SpellLists)
            .ToListAsync();
    }

    public async Task<List<Spell>> GetByIdsAsync(IEnumerable<int> ids)
    {
        return await context.Spells.Where(s => ids.Contains(s.Id)).ToListAsync();
    }

    public async Task<List<Spell>> GetByNamesAsync(IEnumerable<string> names)
    {
        return await context.Spells.Where(s => names.Contains(s.Name)).ToListAsync();
    }

    public async Task AddAsync(Spell spell)
    {
        context.Spells.Add(spell);
        await context.SaveChangesAsync();
    }
    
    public async Task AddAsync(IEnumerable<Spell> spells)
    {
        context.Spells.AddRange(spells);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Spell spell)
    {
        context.Spells.Update(spell);
        await context.SaveChangesAsync();
    }
    
    public async Task DeleteAsync(Spell spell)
    {
        context.Spells.Remove(spell);
        await context.SaveChangesAsync();
    }
    
    public async Task ClearAsync()
    {
        await context.Database.ExecuteSqlRawAsync("DELETE FROM Spells");
    }
}
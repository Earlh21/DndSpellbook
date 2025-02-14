using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DndSpellbook.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace DndSpellbook.Data.Services;

public class SpellService(SpellbookContext context)
{
    public async Task<List<Spell>> GetAllAsync()
    {
        return await context.Spells.ToListAsync();
    }

    public async Task<List<Spell>> GetByIdsAsync(IEnumerable<int> ids)
    {
        return await context.Spells.Where(s => ids.Contains(s.Id)).ToListAsync();
    }

    public async Task AddAsync(Spell spell)
    {
        context.Spells.Add(spell);
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
}
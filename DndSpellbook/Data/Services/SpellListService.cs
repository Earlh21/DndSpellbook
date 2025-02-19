using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DndSpellbook.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace DndSpellbook.Data.Services;

public class SpellListService(SpellbookContext context)
{
    public async Task<List<SpellList>> GetAllAsync()
    {
        return await context.SpellLists.ToListAsync();
    }
    
    public async Task<SpellList?> GetByIdAsync(int id)
    {
        return await context.SpellLists.Include(s => s.Spells).FirstOrDefaultAsync(s => s.Id == id);
    }
    
    public async Task<List<SpellList>> GetByNamesAsync(IEnumerable<string> names)
    {
        return await context.SpellLists.Where(s => names.Contains(s.Name)).ToListAsync();
    }
    
    public async Task AddAsync(SpellList spellList)
    {
        context.SpellLists.Add(spellList);
        await context.SaveChangesAsync();
    }
    
    public async Task AddAsync(IEnumerable<SpellList> spellLists)
    {
        context.SpellLists.AddRange(spellLists);
        await context.SaveChangesAsync();
    }
    
    public async Task UpdateAsync(SpellList spellList)
    {
        context.SpellLists.Update(spellList);
        await context.SaveChangesAsync();
    }
    
    public async Task DeleteAsync(SpellList spellList)
    {
        context.SpellLists.Remove(spellList);
        await context.SaveChangesAsync();
    }
    
    public async Task ClearAsync()
    {
        await context.Database.ExecuteSqlRawAsync("DELETE FROM SpellLists");
    }
}
using DndSpellbook.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace DndSpellbook.Data;

public class SpellbookContext : DbContext
{
    public DbSet<Spell> Spells { get; set; }
    public DbSet<Character> Characters { get; set; }

    public SpellbookContext(DbContextOptions<SpellbookContext> options) : base(options)
    {
    }
}
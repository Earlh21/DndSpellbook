using System.Linq;
using DndSpellbook.Data.Models;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using ReactiveUI.Validation.Helpers;

namespace DndSpellbook.Data;

public class SpellbookContext : DbContext
{
    public DbSet<Spell> Spells { get; set; }
    public DbSet<Character> Characters { get; set; }

    public SpellbookContext(DbContextOptions<SpellbookContext> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        IgnoreReactiveObjectProperties(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }
    
    private static void IgnoreReactiveObjectProperties(ModelBuilder modelBuilder)
    {
        var reactiveObjects = modelBuilder.Model.GetEntityTypes()
            .Select(e => e.ClrType)
            .Where(t => t.IsClass && !t.IsAbstract && typeof(ReactiveObject).IsAssignableFrom(t));

        foreach (var entityType in reactiveObjects)
        {
            modelBuilder.Entity(entityType).Ignore("Changed");
            modelBuilder.Entity(entityType).Ignore("Changing");
            modelBuilder.Entity(entityType).Ignore("ThrowExceptions");
        }
        
        var reactiveValidationObjects = modelBuilder.Model.GetEntityTypes()
            .Select(e => e.ClrType)
            .Where(t => t.IsClass && !t.IsAbstract && typeof(ReactiveValidationObject).IsAssignableFrom(t));

        foreach (var entityType in reactiveValidationObjects)
        {
            modelBuilder.Entity(entityType).Ignore("HasErrors");
        }
    }
}
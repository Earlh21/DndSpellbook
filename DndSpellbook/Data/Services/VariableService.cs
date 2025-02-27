using System.Linq;
using System.Threading.Tasks;
using DndSpellbook.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace DndSpellbook.Data.Services;

public class VariableService(SpellbookContext context)
{
    public async Task<string?> GetStringAsync(string name)
    {
        return (await context.StringVariables.FirstOrDefaultAsync(x => x.Name == name))?.Value;
    }
    
    public async Task SetStringAsync(string name, string value)
    {
        var variable = await context.StringVariables.FirstOrDefaultAsync(x => x.Name == name);
        if (variable is null)
        {
            variable = new StringVariable( name, value);
            context.StringVariables.Add(variable);
        }
        else
        {
            variable.Value = value;
        }

        await context.SaveChangesAsync();
    }
    
    public async Task<bool?> GetBoolAsync(string name)
    {
        var value = await context.StringVariables.FirstOrDefaultAsync(x => x.Name == name);
        return value is null ? null : bool.Parse(value.Value);
    }
    
    public async Task SetBoolAsync(string name, bool value)
    {
        var variable = await context.StringVariables.FirstOrDefaultAsync(x => x.Name == name);
        if (variable is null)
        {
            variable = new StringVariable(name, value.ToString());
            context.StringVariables.Add(variable);
        }
        else
        {
            variable.Value = value.ToString();
        }

        await context.SaveChangesAsync();
    }
}
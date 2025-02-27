using Microsoft.EntityFrameworkCore;

namespace DndSpellbook.Data.Models;

[PrimaryKey("Name")]
public class StringVariable
{
    public string Name { get; set; }
    public string Value { get; set; }

    public StringVariable(string name, string value)
    {
        Name = name;
        Value = value;
    }
}
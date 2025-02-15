using System.Collections.ObjectModel;
using ReactiveUI;

namespace DndSpellbook.Data.Models;

public partial class Character : ReactiveObject
{
    public int Id { get; set; }

    private string name = "";
    public string Name
    {
        get => name;
        set => this.RaiseAndSetIfChanged(ref name, value);
    }

    public ObservableCollection<SpellEntry> Spells { get; } = new();

    private Character() { }

    public Character(string name)
    {
        Name = name;
    }
}
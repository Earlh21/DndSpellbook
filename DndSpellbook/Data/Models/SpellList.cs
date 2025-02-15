﻿using System.Collections.ObjectModel;
using ReactiveUI;

namespace DndSpellbook.Data.Models;

public class SpellList : ReactiveObject
{
    public int Id { get; set; }
    
    private string name = "";
    public string Name
    {
        get => name;
        set => this.RaiseAndSetIfChanged(ref name, value);
    }

    public ObservableCollection<Spell> Spells { get; } = new();
    
    private SpellList() { }

    public SpellList(string name)
    {
        this.name = name;
    }
}
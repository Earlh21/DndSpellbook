using Microsoft.EntityFrameworkCore;
using ReactiveUI;

namespace DndSpellbook.Data.Models;

[Owned]
public class SpellSlots : ReactiveObject
{
    private int level1Max;
    public int Level1Max
    {
        get => level1Max;
        set => this.RaiseAndSetIfChanged(ref level1Max, value);
    }

    private int level1Used;
    public int Level1Used
    {
        get => level1Used;
        set => this.RaiseAndSetIfChanged(ref level1Used, value);
    }
    
    private int level2Max;
    public int Level2Max
    {
        get => level2Max;
        set => this.RaiseAndSetIfChanged(ref level2Max, value);
    }
    
    private int level2Used;
    public int Level2Used
    {
        get => level2Used;
        set => this.RaiseAndSetIfChanged(ref level2Used, value);
    }
    
    private int level3Max;
    public int Level3Max
    {
        get => level3Max;
        set => this.RaiseAndSetIfChanged(ref level3Max, value);
    }
    
    private int level3Used;
    public int Level3Used
    {
        get => level3Used;
        set => this.RaiseAndSetIfChanged(ref level3Used, value);
    }
    
    private int level4Max;
    public int Level4Max
    {
        get => level4Max;
        set => this.RaiseAndSetIfChanged(ref level4Max, value);
    }
    
    private int level4Used;
    public int Level4Used
    {
        get => level4Used;
        set => this.RaiseAndSetIfChanged(ref level4Used, value);
    }
    
    private int level5Max;
    public int Level5Max
    {
        get => level5Max;
        set => this.RaiseAndSetIfChanged(ref level5Max, value);
    }
    
    private int level5Used;
    public int Level5Used
    {
        get => level5Used;
        set => this.RaiseAndSetIfChanged(ref level5Used, value);
    }
    
    private int level6Max;
    public int Level6Max
    {
        get => level6Max;
        set => this.RaiseAndSetIfChanged(ref level6Max, value);
    }
    
    private int level6Used;
    public int Level6Used
    {
        get => level6Used;
        set => this.RaiseAndSetIfChanged(ref level6Used, value);
    }
    
    private int level7Max;
    public int Level7Max
    {
        get => level7Max;
        set => this.RaiseAndSetIfChanged(ref level7Max, value);
    }
    
    private int level7Used;
    public int Level7Used
    {
        get => level7Used;
        set => this.RaiseAndSetIfChanged(ref level7Used, value);
    }
    
    private int level8Max;
    public int Level8Max
    {
        get => level8Max;
        set => this.RaiseAndSetIfChanged(ref level8Max, value);
    }
    
    private int level8Used;
    public int Level8Used
    {
        get => level8Used;
        set => this.RaiseAndSetIfChanged(ref level8Used, value);
    }
    
    private int level9Max;
    public int Level9Max
    {
        get => level9Max;
        set => this.RaiseAndSetIfChanged(ref level9Max, value);
    }
    
    private int level9Used;
    public int Level9Used
    {
        get => level9Used;
        set => this.RaiseAndSetIfChanged(ref level9Used, value);
    }

    public SpellSlots()
    {
        
    }
}
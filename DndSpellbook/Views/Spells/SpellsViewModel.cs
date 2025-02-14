using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using DndSpellbook.Data.Models;
using DndSpellbook.Data.Services;
using DndSpellbook.ViewModels;
using ReactiveUI;
using Splat;

namespace DndSpellbook.Views;

public class SpellsViewModel : ViewModelBase
{
    private readonly SpellService spellService;
    
    public override string UrlPathSegment => "spells";
    public override IScreen HostScreen { get; }
    
    private ObservableCollection<Spell> spells = new();
    public ObservableCollection<Spell> Spells
    {
        get => spells;
        set => this.RaiseAndSetIfChanged(ref spells, value);
    }
    
    public ReactiveCommand<Unit, Unit> NewSpellCommand { get; }
    
    public SpellsViewModel(IScreen hostScreen, SpellService spellService)
    {
        this.spellService = spellService;
        
        NewSpellCommand = ReactiveCommand.Create(NewSpell);
        
        HostScreen = hostScreen;
    }

    public async Task LoadDataAsync()
    {
        var spells = await spellService.GetAllAsync();
        
        Spells = new (spells);
    }
    
    private void NewSpell()
    {
        Spells.Add(new("Name"));
    }
}
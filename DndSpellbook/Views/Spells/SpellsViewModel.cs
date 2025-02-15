using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using DndSpellbook.Data.Models;
using DndSpellbook.Data.Services;
using DndSpellbook.Navigation;
using ReactiveUI;
using Splat;

namespace DndSpellbook.Views;

public class SpellsViewModel : ViewModelBase, IDialog
{
    private readonly SpellService spellService;
    private readonly SpellListService spellListService;

    private bool isSelector;

    public bool IsSelector
    {
        get => isSelector;
        set => this.RaiseAndSetIfChanged(ref isSelector, value);
    }

    private ObservableCollection<SpellCardViewModel> spells = new();
    public ObservableCollection<SpellCardViewModel> Spells
    {
        get => spells;
        set => this.RaiseAndSetIfChanged(ref spells, value);
    }
    
    private ObservableCollection<SpellList> spellLists = new();
    public ObservableCollection<SpellList> SpellLists
    {
        get => spellLists;
        set => this.RaiseAndSetIfChanged(ref spellLists, value);
    }

    public ReactiveCommand<Unit, Unit> NewSpellCommand { get; }
    public ReactiveCommand<SpellCardViewModel, Unit> DeleteSpellCommand { get; }
    public ReactiveCommand<Unit, Unit> SaveCommand { get; }
    public ReactiveCommand<Unit, Unit> CancelCommand { get; }

    public SpellsViewModel(SpellService spellService, SpellListService spellListService, bool asSelector = false)
    {
        this.spellService = spellService;
        this.spellListService = spellListService;
        IsSelector = asSelector;

        NewSpellCommand = ReactiveCommand.CreateFromTask(NewSpell);
        DeleteSpellCommand = ReactiveCommand.CreateFromTask<SpellCardViewModel>(DeleteSpell);
        SaveCommand = ReactiveCommand.Create(Save);
        CancelCommand = ReactiveCommand.Create(Cancel);
    }

    public async Task LoadDataAsync()
    {
        var fetchedSpells = await spellService.GetAllAsync();
        var fetchedSpellLists = await spellListService.GetAllAsync();

        var spellListArray = fetchedSpellLists.ToArray();

        Spells = new(fetchedSpells.Select(s =>
            new SpellCardViewModel(s, spellListArray, spellService, IsSelector, DeleteSpellCommand)));
        
        SpellLists = new(fetchedSpellLists);
    }

    private async Task DeleteSpell(SpellCardViewModel spell)
    {
        await spellService.DeleteAsync(spell.Spell);
        Spells.Remove(spell);
    }

    private async Task NewSpell()
    {
        try
        {
            var spell = new Spell("Name");
            var spellCard = new SpellCardViewModel(spell, spellLists.ToArray(), spellService, IsSelector,
                DeleteSpellCommand);
            Spells.Add(spellCard);

            await spellService.AddAsync(spell);
        }
        catch (Exception e)
        {
            int d = 3;
        }
    }

    private void Save()
    {
        Closed?.Invoke(this, Spells.Where(s => s.IsSelected).Select(s => s.Spell.Id));
    }

    private void Cancel()
    {
        Cancelled?.Invoke(this, EventArgs.Empty);
    }

    public event EventHandler<object>? Closed;
    public event EventHandler? Cancelled;
}
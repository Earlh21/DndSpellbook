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

    public ReactiveCommand<Unit, Unit> NewSpellCommand { get; }
    public ReactiveCommand<SpellCardViewModel, Unit> DeleteSpellCommand { get; }
    public ReactiveCommand<Unit, Unit> SaveCommand { get; }
    public ReactiveCommand<Unit, Unit> CancelCommand { get; }

    public SpellsViewModel(SpellService spellService, bool asSelector = false)
    {
        this.spellService = spellService;
        IsSelector = asSelector;

        NewSpellCommand = ReactiveCommand.CreateFromTask(NewSpell);
        DeleteSpellCommand = ReactiveCommand.CreateFromTask<SpellCardViewModel>(DeleteSpell);
        SaveCommand = ReactiveCommand.Create(Save);
        CancelCommand = ReactiveCommand.Create(Cancel);
    }

    public async Task LoadDataAsync()
    {
        var fetchedSpells = await spellService.GetAllAsync();

        Spells = new(fetchedSpells.Select(s =>
            new SpellCardViewModel(s, spellService, IsSelector, DeleteSpellCommand)));
    }

    private async Task DeleteSpell(SpellCardViewModel spell)
    {
        await spellService.DeleteAsync(spell.Spell);
        Spells.Remove(spell);
    }

    private async Task NewSpell()
    {
        var spell = new Spell("Name");
        var spellCard = new SpellCardViewModel(spell, spellService, IsSelector, DeleteSpellCommand);
        Spells.Add(spellCard);

        await spellService.AddAsync(spell);
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
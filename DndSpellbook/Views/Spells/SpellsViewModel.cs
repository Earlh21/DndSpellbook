using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using DndSpellbook.Controls;
using DndSpellbook.Data.Models;
using DndSpellbook.Data.Models.Enums;
using DndSpellbook.Data.Services;
using DndSpellbook.Import;
using DndSpellbook.Navigation;
using DndSpellbook.Util;
using DynamicData.Binding;
using ReactiveUI;
using Splat;

namespace DndSpellbook.Views;

public class SpellsViewModel : ViewModelBase, IDialog
{
    private readonly SpellService spellService;
    private readonly SpellListService spellListService;

    private int? filterMinLevel;

    public int? FilterMinLevel
    {
        get => filterMinLevel;
        set => this.RaiseAndSetIfChanged(ref filterMinLevel, value);
    }

    private int? filterMaxLevel;

    public int? FilterMaxLevel
    {
        get => filterMaxLevel;
        set => this.RaiseAndSetIfChanged(ref filterMaxLevel, value);
    }

    private SpellSchool? filterSchool;

    public SpellSchool? FilterSchool
    {
        get => filterSchool;
        set => this.RaiseAndSetIfChanged(ref filterSchool, value);
    }

    private SpellList? filterSpellList;

    public SpellList? FilterSpellList
    {
        get => filterSpellList;
        set => this.RaiseAndSetIfChanged(ref filterSpellList, value);
    }

    private string filterText = "";

    public string FilterText
    {
        get => filterText;
        set => this.RaiseAndSetIfChanged(ref filterText, value);
    }

    private bool isSelector;

    public bool IsSelector
    {
        get => isSelector;
        set => this.RaiseAndSetIfChanged(ref isSelector, value);
    }

    private bool isCardView;

    public bool IsCardView
    {
        get => isCardView;
        set => this.RaiseAndSetIfChanged(ref isCardView, value);
    }

    private FilteredCollection<SpellCardViewModel> spells;
    public FilteredCollection<SpellCardViewModel> Spells
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
    
    private ObservableCollection<SpellList?> spellListsWithNull = new();
    public ObservableCollection<SpellList?> SpellListsWithNull
    {
        get => spellListsWithNull;
        set => this.RaiseAndSetIfChanged(ref spellListsWithNull, value);
    }

    public PageRequest PageRequest { get; } = new(1, 25);
    public int[] PageSizes { get; } = [10, 25, 50, 100];
    public int MaxPage => (int)Math.Ceiling((double)Spells.AllItems.Count / PageRequest.Size);
    public SpellSchool?[] SpellSchools => Enum.GetValues<SpellSchool>().Select(x => (SpellSchool?)x).Prepend(null).ToArray();

    public ReactiveCommand<Unit, Unit> NewSpellCommand { get; }
    public ReactiveCommand<SpellCardViewModel, Unit> DeleteSpellCommand { get; }
    public ReactiveCommand<Unit, Unit> SaveCommand { get; }
    public ReactiveCommand<Unit, Unit> CancelCommand { get; }
    public ReactiveCommand<Unit, Unit> ImportSpellsCommand { get; }
    public ReactiveCommand<Unit, Unit> ClearSpellsCommand { get; }

    public Interaction<Unit, string?> OpenImportSpellsFile { get; } = new();

    public SpellsViewModel(SpellService spellService, SpellListService spellListService, bool asSelector = false,
        bool asCardView = false)
    {
        this.spellService = spellService;
        this.spellListService = spellListService;
        IsSelector = asSelector;
        IsCardView = asCardView;

        NewSpellCommand = ReactiveCommand.CreateFromTask(NewSpell);
        DeleteSpellCommand = ReactiveCommand.CreateFromTask<SpellCardViewModel>(DeleteSpell);
        SaveCommand = ReactiveCommand.Create(Save);
        CancelCommand = ReactiveCommand.Create(Cancel);
        ImportSpellsCommand = ReactiveCommand.CreateFromTask(ImportSpells);
        ClearSpellsCommand = ReactiveCommand.CreateFromTask(ClearSpells);
        
        Func<SpellCardViewModel, bool> levelFilter = spell =>
        {
            if (FilterMinLevel != null && spell.Spell.Level < FilterMinLevel) return false;
            if (FilterMaxLevel != null && spell.Spell.Level > FilterMaxLevel) return false;
            return true;
        };

        Func<SpellCardViewModel, bool> schoolFilter = spell =>
        {
            if (FilterSchool != null && spell.Spell.School != FilterSchool) return false;
            return true;
        };

        Func<SpellCardViewModel, bool> listFilter = spell =>
        {
            if (FilterSpellList != null && !spell.Spell.SpellLists.Contains(FilterSpellList)) return false;
            return true;
        };

        Func<SpellCardViewModel, bool> textFilter = spell =>
        {
            if (String.IsNullOrWhiteSpace(FilterText)) return true;
            
            return spell.Spell.Name.Contains(FilterText, StringComparison.OrdinalIgnoreCase) ||
                   spell.Spell.Description.Contains(FilterText, StringComparison.OrdinalIgnoreCase) ||
                   spell.Spell.School.ToString().Contains(FilterText, StringComparison.OrdinalIgnoreCase) ||
                   spell.Spell.SpellLists.Any(l => l.Name.Contains(FilterText, StringComparison.OrdinalIgnoreCase));
        };

        Spells = new FilteredCollection<SpellCardViewModel>(x => x.Spell.Id,
            null,
            PageRequest.AsObservable(),
            this.WhenValueChanged(vm => vm.FilterMinLevel).Select(_ => levelFilter),
            this.WhenValueChanged(vm => vm.FilterMaxLevel).Select(_ => levelFilter),
            this.WhenValueChanged(vm => vm.FilterSchool).Select(_ => schoolFilter),
            this.WhenValueChanged(vm => vm.FilterSpellList).Select(_ => listFilter),
            this.WhenValueChanged(vm => vm.FilterText).Select(_ => textFilter)
        );
    }

    public async Task LoadDataAsync()
    {
        var fetchedSpells = await spellService.GetAllAsync();
        var fetchedSpellLists = await spellListService.GetAllAsync();

        var spellListArray = fetchedSpellLists.ToArray();

        Spells.ReplaceAll(fetchedSpells.Select(s =>
            new SpellCardViewModel(s, spellListArray, spellService, IsSelector, DeleteSpellCommand)));

        SpellLists = new(fetchedSpellLists);
        SpellListsWithNull = new(fetchedSpellLists.Prepend(null));
    }

    private async Task DeleteSpell(SpellCardViewModel spell)
    {
        await spellService.DeleteAsync(spell.Spell);
        Spells.Remove(spell);
    }

    private async Task NewSpell()
    {
        var spell = new Spell("Name");
        var spellCard = new SpellCardViewModel(spell, spellLists.ToArray(), spellService, IsSelector,
            DeleteSpellCommand);
        Spells.AddOrUpdate(spellCard);

        await spellService.AddAsync(spell);
    }

    private async Task ImportSpells()
    {
        var path = await OpenImportSpellsFile.Handle(Unit.Default);
        if (path == null) return;

        var importer = new SpellImporter(spellService, spellListService);
        var data = await importer.ImportSpellsAsync(await File.ReadAllTextAsync(path));

        await spellService.AddAsync(data.NewSpells);
    }

    private async Task ClearSpells()
    {
        await spellService.ClearAsync();
        await LoadDataAsync();
    }

    private void Save()
    {
        Closed?.Invoke(this, Spells.AllItems.Where(s => s.IsSelected).Select(s => s.Spell.Id));
    }

    private void Cancel()
    {
        Cancelled?.Invoke(this, EventArgs.Empty);
    }

    public event EventHandler<object>? Closed;
    public event EventHandler? Cancelled;
}
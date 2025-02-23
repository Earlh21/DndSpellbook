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
using DndSpellbook.Navigation;
using DndSpellbook.Util;
using DynamicData;
using DynamicData.Binding;
using DynamicData.PLinq;
using ReactiveUI;
using Splat;
using PageRequest = DndSpellbook.Util.PageRequest;

namespace DndSpellbook.Views;

public class SpellsViewModel : ViewModelBase, IDialog
{
    private readonly SpellService spellService;
    private readonly SpellListService spellListService;

    private bool isPaneOpen = true;

    public bool IsPaneOpen
    {
        get => isPaneOpen;
        set => this.RaiseAndSetIfChanged(ref isPaneOpen, value);
    }

    private int filterMinLevel = 0;

    public int FilterMinLevel
    {
        get => filterMinLevel;
        set => this.RaiseAndSetIfChanged(ref filterMinLevel, value);
    }

    private int filterMaxLevel = 9;

    public int FilterMaxLevel
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
    
    private DamageType? filterDamageType;
    public DamageType? FilterDamageType
    {
        get => filterDamageType;
        set => this.RaiseAndSetIfChanged(ref filterDamageType, value);
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

    private SourceCache<SpellExpanderViewModel, int> Expanders { get; } = new(x => x.Spell.Id);
    public ReadOnlyObservableCollection<SpellExpanderViewModel> ExpandersView { get; }

    private SourceCache<SpellCardViewModel, int> Cards { get; } = new(x => x.Spell.Id);
    public ReadOnlyObservableCollection<SpellCardViewModel> CardsView { get; }

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

    public PageRequest PageRequest { get; } = new(1, 100);
    public static int[] PageSizes { get; } = [50, 200, 1000];
    public static int[] Levels { get; } = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9];
    public int MaxPage => (int)Math.Ceiling((double)Cards.Items.Count / PageRequest.Size);

    public SpellSchool?[] SpellSchools =>
        Enum.GetValues<SpellSchool>().Select(x => (SpellSchool?)x).Prepend(null).ToArray();
    public DamageType?[] DamageTypes =>
        Enum.GetValues<DamageType>().Select(x => (DamageType?)x).Prepend(null).ToArray();

    public ReactiveCommand<Unit, Unit> NewSpellCommand { get; }
    public ReactiveCommand<Unit, Unit> TogglePaneCommand { get; }
    public ReactiveCommand<Spell, Unit> DeleteSpellCommand { get; }
    public ReactiveCommand<Unit, Unit> ReturnSpellsCommand { get; }
    public ReactiveCommand<Unit, Unit> ReturnNoSpellsCommand { get; }
    public ReactiveCommand<Unit, Unit> ImportSpellsCommand { get; }
    public ReactiveCommand<Unit, Unit> ClearSpellsCommand { get; }

    public Interaction<Unit, string?> OpenImportSpellsFile { get; } = new();
    public Interaction<Unit, bool> DeleteSpellsConfirmation { get; } = new();

    public SpellsViewModel(SpellService spellService, SpellListService spellListService, bool asSelector = false,
        bool asCardView = false)
    {
        this.spellService = spellService;
        this.spellListService = spellListService;
        IsSelector = asSelector;
        IsCardView = asCardView;

        NewSpellCommand = ReactiveCommand.CreateFromTask(NewSpell);
        TogglePaneCommand = ReactiveCommand.Create(TogglePaneOpen);
        DeleteSpellCommand = ReactiveCommand.CreateFromTask<Spell>(DeleteSpell);
        ReturnSpellsCommand = ReactiveCommand.Create(ReturnSpells);
        ReturnNoSpellsCommand = ReactiveCommand.Create(ReturnNoSpells);
        ImportSpellsCommand = ReactiveCommand.CreateFromTask(ImportSpells);
        ClearSpellsCommand = ReactiveCommand.CreateFromTask(ClearSpells);


        Func<SpellCardViewModel, bool> cardLevelFilter = card => LevelFilter(card.Spell);
        Func<SpellExpanderViewModel, bool> expanderLevelFilter = expander => LevelFilter(expander.Spell);

        Func<SpellCardViewModel, bool> cardSchoolFilter = card => SchoolFilter(card.Spell);
        Func<SpellExpanderViewModel, bool> expanderSchoolFilter = expander => SchoolFilter(expander.Spell);

        Func<SpellCardViewModel, bool> cardListFilter = card => ListFilter(card.Spell);
        Func<SpellExpanderViewModel, bool> expanderListFilter = expander => ListFilter(expander.Spell);

        Func<SpellCardViewModel, bool> damageTypeFilter = card => DamageTypeFilter(card.Spell);
        Func<SpellExpanderViewModel, bool> damageTypeFilterExpander = expander => DamageTypeFilter(expander.Spell);
        
        Func<SpellCardViewModel, bool> cardTextFilter = card => TextFilter(card.Spell);
        Func<SpellExpanderViewModel, bool> expanderTextFilter = expander => TextFilter(expander.Spell);

        Expanders.Connect()
            .Filter(this.WhenPropertyChanged(vm => vm.FilterText).Select(_ => expanderTextFilter))
            .Filter(this.WhenPropertyChanged(vm => vm.FilterMinLevel).Select(_ => expanderLevelFilter))
            .Filter(this.WhenPropertyChanged(vm => vm.FilterMaxLevel).Select(_ => expanderLevelFilter))
            .Filter(this.WhenPropertyChanged(vm => vm.FilterSchool).Select(_ => expanderSchoolFilter))
            .Filter(this.WhenPropertyChanged(vm => vm.FilterDamageType).Select(_ => damageTypeFilterExpander))
            .Filter(this.WhenPropertyChanged(vm => vm.FilterSpellList).Select(_ => expanderListFilter))
            .SortAndPage(new SpellComparer(), PageRequest.AsObservable())
            .Bind(out var expandersView)
            .Subscribe();

        Cards.Connect()
            .Filter(this.WhenPropertyChanged(vm => vm.FilterText).Select(_ => cardTextFilter))
            .Filter(this.WhenPropertyChanged(vm => vm.FilterMinLevel).Select(_ => cardLevelFilter))
            .Filter(this.WhenPropertyChanged(vm => vm.FilterMaxLevel).Select(_ => cardLevelFilter))
            .Filter(this.WhenPropertyChanged(vm => vm.FilterSchool).Select(_ => cardSchoolFilter))
            .Filter(this.WhenPropertyChanged(vm => vm.FilterDamageType).Select(_ => damageTypeFilter))
            .Filter(this.WhenPropertyChanged(vm => vm.FilterSpellList).Select(_ => cardListFilter))
            .SortAndPage(new SpellComparer(), PageRequest.AsObservable())
            .Bind(out var cardsView)
            .Subscribe();

        ExpandersView = expandersView;
        CardsView = cardsView;
        return;

        bool TextFilter(Spell spell)
        {
            if (String.IsNullOrWhiteSpace(FilterText)) return true;

            return spell.Name.Contains(FilterText, StringComparison.OrdinalIgnoreCase) ||
                   spell.Description.Contains(FilterText, StringComparison.OrdinalIgnoreCase) ||
                   spell.School.ToString().Contains(FilterText, StringComparison.OrdinalIgnoreCase) ||
                   spell.SpellLists.Any(l => l.Name.Contains(FilterText, StringComparison.OrdinalIgnoreCase));
        }

        bool LevelFilter(Spell spell) => spell.Level >= FilterMinLevel && spell.Level <= FilterMaxLevel;

        bool SchoolFilter(Spell spell)
        {
            if (FilterSchool != null && spell.School != FilterSchool) return false;
            return true;
        }

        bool ListFilter(Spell spell)
        {
            if (FilterSpellList != null && !spell.SpellLists.Contains(FilterSpellList)) return false;
            return true;
        }
        
        bool DamageTypeFilter(Spell spell)
        {
            if (FilterDamageType == null) return true;
            
            return spell.Description.Contains(FilterDamageType.ToString()!, StringComparison.OrdinalIgnoreCase);
        }
    }

    public async Task LoadDataAsync()
    {
        var fetchedSpells = await spellService.GetAllAsync();
        var fetchedSpellLists = await spellListService.GetAllAsync();

        var spellListArray = fetchedSpellLists.ToArray();

        Cards.Clear();
        Cards.AddOrUpdate(fetchedSpells.Select(s =>
            new SpellCardViewModel(s, spellListArray, spellService, DeleteSpellCommand, IsSelector)));

        Expanders.Clear();
        Expanders.AddOrUpdate(fetchedSpells.Select(s =>
            new SpellExpanderViewModel(s, spellListArray, spellService, DeleteSpellCommand, IsSelector)));

        SpellLists = new(fetchedSpellLists);
        SpellListsWithNull = new(fetchedSpellLists.Prepend(null));

        PageRequest.Size = 200;
    }

    private void TogglePaneOpen()
    {
        IsPaneOpen = !IsPaneOpen;
    }

    private async Task DeleteSpell(Spell spell)
    {
        await spellService.DeleteAsync(spell);
        Cards.RemoveKey(spell.Id);
        Expanders.RemoveKey(spell.Id);
    }

    private async Task NewSpell()
    {
        var spell = new Spell("Name");
        await spellService.AddAsync(spell);

        var card = new SpellCardViewModel(spell, spellLists.ToArray(), spellService, DeleteSpellCommand, IsSelector);
        Cards.AddOrUpdate(card);

        var expander = new SpellExpanderViewModel(spell, spellLists.ToArray(), spellService, DeleteSpellCommand,
            IsSelector);
        Expanders.AddOrUpdate(expander);
    }

    private async Task ImportSpells()
    {
        var path = await OpenImportSpellsFile.Handle(Unit.Default);
        if (path == null) return;

        var importer = new SpellImporter(spellService, spellListService);
        var data = await importer.ImportSpellsAsync(await File.ReadAllTextAsync(path));

        await spellService.AddAsync(data.NewSpells);

        Cards.AddOrUpdate(data.NewSpells.Select(s =>
            new SpellCardViewModel(s, spellLists.ToArray(), spellService, DeleteSpellCommand, IsSelector)));
        Expanders.AddOrUpdate(data.NewSpells.Select(s => new SpellExpanderViewModel(s, spellLists.ToArray(),
            spellService,
            DeleteSpellCommand, IsSelector)));
    }

    private async Task ClearSpells()
    {
        if(!await DeleteSpellsConfirmation.Handle(Unit.Default)) return;
        
        await spellService.ClearAsync();
        await LoadDataAsync();
    }

    private void ReturnSpells()
    {
        if (IsCardView)
        {
            Closed?.Invoke(this, Cards.Items.Where(s => s.IsSelected).Select(s => s.Spell.Id));
        }
        else
        {
            Closed?.Invoke(this, Expanders.Items.Where(s => s.IsSelected).Select(s => s.Spell.Id));
        }
    }

    private void ReturnNoSpells()
    {
        Cancelled?.Invoke(this, EventArgs.Empty);
    }

    public event EventHandler<object>? Closed;
    public event EventHandler? Cancelled;

    private class SpellComparer : IComparer<SpellCardViewModel>, IComparer<SpellExpanderViewModel>, IComparer<Spell>
    {
        public int Compare(Spell? x, Spell? y)
        {
            if (x == null && y == null) return 0;
            if (x == null) return -1;
            if (y == null) return 1;

            if (x.Level != y.Level) return x.Level.CompareTo(y.Level);
            //if(x.School != y.School) return x.School.CompareTo(y.School);

            return String.Compare(x.Name, y.Name, StringComparison.Ordinal);
        }

        public int Compare(SpellCardViewModel? x, SpellCardViewModel? y)
        {
            return Compare(x?.Spell, y?.Spell);
        }

        public int Compare(SpellExpanderViewModel? x, SpellExpanderViewModel? y)
        {
            return Compare(x?.Spell, y?.Spell);
        }
    }
}
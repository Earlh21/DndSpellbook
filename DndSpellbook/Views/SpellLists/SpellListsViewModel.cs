using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using DndSpellbook.Data.Models;
using DndSpellbook.Data.Services;
using DndSpellbook.Navigation;
using ReactiveUI;

namespace DndSpellbook.Views.SpellLists;

public class SpellListsViewModel : ViewModelBase
{
    private readonly Navigator navigator;
    private readonly SpellListService spellListService;
    
    public ReactiveCommand<SpellList, Unit> DeleteCommand { get; }
    public ReactiveCommand<SpellList, Unit> NavigateCommand { get; }
    public ReactiveCommand<Unit, Unit> NewSpellListCommand { get; }
    
    private ObservableCollection<SpellList> spellLists = new();
    public ObservableCollection<SpellList> SpellLists
    {
        get => spellLists;
        set => this.RaiseAndSetIfChanged(ref spellLists, value);
    }
    
    public SpellListsViewModel(Navigator navigator, SpellListService spellListService)
    {
        this.navigator = navigator;
        this.spellListService = spellListService;
        
        DeleteCommand = ReactiveCommand.CreateFromTask<SpellList>(DeleteSpellList);
        NavigateCommand = ReactiveCommand.Create<SpellList>(Navigate);
        NewSpellListCommand = ReactiveCommand.CreateFromTask(NewSpellList);
    }

    public async Task LoadDataAsync()
    {
        SpellLists = new ObservableCollection<SpellList>(await spellListService.GetAllAsync());
    }
    
    private async Task NewSpellList()
    {
        var spellList = new SpellList("new");
        await spellListService.AddAsync(spellList);
        SpellLists.Add(spellList);
        
        Navigate(spellList);
    }
    
    private async Task DeleteSpellList(SpellList spellList)
    {
        await spellListService.DeleteAsync(spellList);
        SpellLists.Remove(spellList);
    }

    private void Navigate(SpellList spellList)
    {
        var vm = navigator.BuildSpellListViewModel(spellList.Id);
        navigator.Navigate(vm);
    }
}
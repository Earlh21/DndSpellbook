using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using DndSpellbook.Data.Models;
using DndSpellbook.Data.Services;
using DndSpellbook.Navigation;
using ReactiveUI;

namespace DndSpellbook.Views;

public class CharactersViewModel : ViewModelBase
{
    private readonly Navigator navigator;
    private readonly CharacterService characterService;
    
    public override string? UrlPathSegment => "characters";
    public override IScreen HostScreen { get; }
    
    private ObservableCollection<Character> characters = new();
    public ObservableCollection<Character> Characters
    {
        get => characters;
        set => this.RaiseAndSetIfChanged(ref characters, value);
    }
    
    public ReactiveCommand<Unit, Unit> NewCharacterCommand { get; }
    public ReactiveCommand<Character, Unit> DeleteCharacterCommand { get; }
    public ReactiveCommand<Character, Unit> EditCharacterCommand { get; }

    public CharactersViewModel(IScreen screen, Navigator navigator, CharacterService characterService)
    {
        HostScreen = screen;
        this.navigator = navigator;
        
        this.characterService = characterService;
        
        NewCharacterCommand = ReactiveCommand.CreateFromTask(NewCharacter);
        DeleteCharacterCommand = ReactiveCommand.CreateFromTask<Character>(DeleteCharacter);
        EditCharacterCommand = ReactiveCommand.CreateFromTask<Character>(EditCharacter);
    }

    public async Task LoadDataAsync()
    {
        Characters = new ObservableCollection<Character>(await characterService.GetAllAsync());
    }

    private async Task NewCharacter()
    {
        var character = new Character("");
        await characterService.AddAsync(character);
        
        Characters.Add(character);

        var vm = navigator.BuildCharacterViewModel(character.Id);
        navigator.Router.NavigateAndReset.Execute(vm);
    }
    
    private async Task DeleteCharacter(Character character)
    {
        await characterService.DeleteAsync(character);
        Characters.Remove(character);
    }
    
    private async Task EditCharacter(Character character)
    {
        var vm = navigator.BuildCharacterViewModel(character.Id);
        navigator.Router.NavigateAndReset.Execute(vm);
    }
}
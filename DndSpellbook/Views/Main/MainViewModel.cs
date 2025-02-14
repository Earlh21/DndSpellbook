using System.Reactive;
using DndSpellbook.ViewModels;
using ReactiveUI;

namespace DndSpellbook.Views;

public class MainViewModel : ReactiveObject, IScreen
{
    private readonly ViewModelBuilder vmBuilder;
    
    public RoutingState Router { get; }

    public MainViewModel(ViewModelBuilder vmBuilder)
    {
        this.vmBuilder = vmBuilder;
        
        Router = new RoutingState();
    }
    
    public ReactiveCommand<string, Unit> NavigateCommand { get; } = ReactiveCommand.Create<string>(pageName =>
    {
        switch (pageName)
        {
            case "spells":
                var vm = vmBuilder.BuildSpellsViewModel();
        }
    });
}
using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Interactivity;
using DndSpellbook.ViewModels;
using ReactiveUI;

namespace DndSpellbook.Views;

public class MainViewModel : ReactiveObject, IScreen
{
    private readonly ViewModelBuilder vmBuilder;

    public RoutingState Router { get; } = new();
    
    public ReactiveCommand<string, Unit> NavigateCommand { get; }

    public MainViewModel(ViewModelBuilder vmBuilder)
    {
        this.vmBuilder = vmBuilder;
        
        NavigateCommand = ReactiveCommand.Create<string>(Navigate);
    }
    
    private void Navigate(string pageName)
    {
        switch (pageName)
        {
            case "spells":
                var vm = vmBuilder.BuildSpellsViewModel();
                Router.Navigate.Execute(vm);
                break;
        }
    }
}
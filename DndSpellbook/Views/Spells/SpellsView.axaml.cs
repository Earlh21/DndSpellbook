using System;

using System.Linq;

using System.Reactive.Linq;

using System.Threading.Tasks;

using Avalonia;

using Avalonia.Controls;

using Avalonia.Interactivity;

using Avalonia.Markup.Xaml;

using Avalonia.Platform.Storage;

using Avalonia.ReactiveUI;

using DynamicData.Binding;

using MsBox.Avalonia;

using MsBox.Avalonia.Enums;

using ReactiveUI;
namespace DndSpellbook.Views;
public partial class SpellsView : ReactiveUserControl<SpellsViewModel>
{
    private SpellExpandersView? expandersView;
    private SpellCardsView? cardsView;

    public SpellsView()
    {
        this.WhenActivated(disposables => { });
        AvaloniaXamlLoader.Load(this);
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is not SpellsViewModel vm) return;
        
        vm.OpenImportSpellsFile.RegisterHandler(async interaction =>
        {
            var path = await OpenImportSpellsFile();
            interaction.SetOutput(path);
        });
        
        vm.DeleteSpellsConfirmation.RegisterHandler(async interaction =>
        {
            if (TopLevel.GetTopLevel(this) is not Window window)
            {
                interaction.SetOutput(false);
                return;
            }
            
            var result = await MessageBoxManager
                .GetMessageBoxStandard(
                    "Delete All Spells",
                    "Are you sure want to delete all spells?",
                    ButtonEnum.YesNo
                ).ShowWindowDialogAsync(window) == ButtonResult.Yes;
            interaction.SetOutput(result);
        });
        
        vm.WhenPropertyChanged(x => x.IsCardView).Subscribe(_ =>
        {
            if (vm.IsCardView)
            {
                SetCardsView();
            }
            else
            {
                SetExpandersView();
            }
        });

        Task.Run(() => vm.LoadDataAsync());
    }

    private async Task<string?> OpenImportSpellsFile()
    {
        var storage = TopLevel.GetTopLevel(this)?.StorageProvider;
        if (storage == null) return null;
        
        var options = new FilePickerOpenOptions
        {
            AllowMultiple = false,
            FileTypeFilter = [new FilePickerFileType("JSON") { Patterns = ["*.json"] }]
        };

        var files = await storage.OpenFilePickerAsync(options);
        return files.FirstOrDefault()?.TryGetLocalPath();
    }

    private void SetCardsView()
    {
        if (SpellsViewContainer.Content is SpellCardsView) return;

        if (SpellsViewContainer.Content is SpellExpandersView expandersView)
        {
            this.expandersView = expandersView;
        }

        SpellsViewContainer.Content = cardsView ?? new SpellCardsView
        {
            DataContext = DataContext,
            Margin = new(0, 30, 0, 0)
        };
    }
    
    private void SetExpandersView()
    {
        if (SpellsViewContainer.Content is SpellExpandersView) return;

        if (SpellsViewContainer.Content is SpellCardsView cardsView)
        {
            this.cardsView = cardsView;
        }

        SpellsViewContainer.Content = expandersView ?? new SpellExpandersView
        {
            DataContext = DataContext,
            Margin = new(5, 30, 5, 0)
        };
    }
}
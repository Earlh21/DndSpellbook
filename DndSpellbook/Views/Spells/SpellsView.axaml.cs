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
using ReactiveUI;

namespace DndSpellbook.Views;

public partial class SpellsView : ReactiveUserControl<SpellsViewModel>
{
    public SpellsView()
    {
        this.WhenActivated(disposables => { });
        AvaloniaXamlLoader.Load(this);
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

        Task.Run(vm.LoadDataAsync);
    }
    
    private async Task<string?> OpenImportSpellsFile()
    {
        var storage = TopLevel.GetTopLevel(this)?.StorageProvider;
        if (storage == null) return null;
        
        var options = new FilePickerOpenOptions
        {
            AllowMultiple = false,
            FileTypeFilter = [new FilePickerFileType("JSON") { Patterns = ["*.json"]}]
        };

        var files = await storage.OpenFilePickerAsync(options);
        return files.FirstOrDefault()?.TryGetLocalPath();
    }
}
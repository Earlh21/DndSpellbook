using System;
using System.IO;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using DndSpellbook.Data;
using DndSpellbook.Data.Services;
using DndSpellbook.ViewModels;
using DndSpellbook.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace DndSpellbook;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DndSpellbook", "data.db");
        
        var serviceProvider = ConfigureServices(dbPath);
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow()
            {
                DataContext = serviceProvider.GetRequiredService<MainViewModel>()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
    
    private IServiceProvider ConfigureServices(string dbPath)
    {
        var vmBuilder = new ViewModelBuilder();
        var mainViewModel = new MainViewModel(vmBuilder);
        
        var services = new ServiceCollection();

        services.AddSingleton(vmBuilder);
        services.AddSingleton<MainViewModel>(mainViewModel);
        services.AddSingleton<IScreen>(mainViewModel);
        
        services.AddDbContext<SpellbookContext>(options =>
        {
            options.UseSqlite($"Data Source={dbPath}");
        });

        services.Scan(scan => scan
            .FromEntryAssembly()
            .AddClasses(sel => sel.InExactNamespaceOf(typeof(SpellService)))
            .AsSelf()
            .WithScopedLifetime());
        
        var serviceProvider = services.BuildServiceProvider();
        
        vmBuilder.ServiceProvider = serviceProvider;
        
        return serviceProvider;
    }
}
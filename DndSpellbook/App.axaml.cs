using System;
using System.IO;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using DndSpellbook.Data;
using DndSpellbook.Data.Services;
using DndSpellbook.Navigation;
using DndSpellbook.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using MainWindow = DndSpellbook.Windows.MainWindow;

namespace DndSpellbook;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var appdataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DndSpellbook");
        Directory.CreateDirectory(appdataPath);
        
        var dbPath = Path.Combine(appdataPath, "data.db");
        
        var serviceProvider = ConfigureServices(dbPath);
        
        var dbContext = serviceProvider.GetRequiredService<SpellbookContext>();
        dbContext.Database.EnsureCreated();
        
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
        RoutingState router = new();
        var navigator = new Navigator(router);
        var mainViewModel = new MainViewModel(router, navigator);
        
        var services = new ServiceCollection();

        services.AddSingleton(navigator);
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
        
        navigator.ServiceProvider = serviceProvider;
        
        return serviceProvider;
    }
}
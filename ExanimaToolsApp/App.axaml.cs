using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ExanimaTools.Models;
using ExanimaToolsApp;
using System;
using System.IO;

namespace ExanimaTools;

public partial class App : Application
{
    public static ILoggingService? LoggingServiceInstance { get; private set; }
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Set up logging service
        var logDir = Path.Combine(Environment.CurrentDirectory, "logs");
        LoggingServiceInstance = new FileLoggingService(logDir);
        // Seed equipment if DB is empty
        var dbPath = DbManager.GetDbPath();
        bool needsSeeding = false;
        if (!File.Exists(dbPath) || new FileInfo(dbPath).Length < 1024)
        {
            needsSeeding = true;
        }
        else
        {
            try
            {
                var repo = new ExanimaTools.Persistence.EquipmentRepository($"Data Source={dbPath}", LoggingServiceInstance!);
                var all = repo.GetAllAsync().GetAwaiter().GetResult();
                if (all.Count == 0)
                    needsSeeding = true;
            }
            catch { needsSeeding = true; }
        }
        if (needsSeeding)
        {
            try { SeedEquipment.SeedAsync(dbPath, LoggingServiceInstance).GetAwaiter().GetResult(); } catch { /* ignore */ }
        }
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow(LoggingServiceInstance);
        }
        base.OnFrameworkInitializationCompleted();
    }
}
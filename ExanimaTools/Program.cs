// No work should be done in this file without understanding the development practices.
// See: project-management/development_practices.md

using Avalonia;
using System;
using System.Threading.Tasks;
using ExanimaTools;
using ExanimaTools.Services;
using static ExanimaTools.DbManager;
using static ExanimaTools.DumpEquipmentDb;
using static ExanimaTools.SeedEquipment;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ExanimaTools;

class Program
{
    private static ExanimaTools.Models.ILoggingService? _logger;
    private static IServiceProvider? _serviceProvider;

    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        // Set up dependency injection container
        var services = new ServiceCollection();
        services.AddExanimaToolsServices();
        services.AddSingleton<IServiceLocator, ServiceLocator>();
        _serviceProvider = services.BuildServiceProvider();

        // Get logger from DI container
        _logger = _serviceProvider.GetRequiredService<ExanimaTools.Models.ILoggingService>();

        // Global exception handlers
        AppDomain.CurrentDomain.UnhandledException += (s, e) =>
        {
            LogFatalException(e.ExceptionObject as Exception, "AppDomain.CurrentDomain.UnhandledException");
        };
        TaskScheduler.UnobservedTaskException += (s, e) =>
        {
            LogFatalException(e.Exception, "TaskScheduler.UnobservedTaskException");
            e.SetObserved();
        };
        try
        {
            LogEnvironmentInfo();
            var arg0 = args.Length > 0 ? args[0].ToLowerInvariant() : string.Empty;
            if (arg0 == "dump" || arg0 == "dumpequipmentdb")
            {
                var dbPath = args.Length > 1 ? args[1] : DbManager.GetDbPath();
                var outPath = args.Length > 2 ? args[2] : "equipment_dump.txt";
                DumpEquipmentDb.DumpAsync(dbPath, outPath).GetAwaiter().GetResult();
                _logger?.Log($"Equipment dump written to {outPath}");
                return;
            }
            if (arg0 == "seed" || arg0 == "seedequipment")
            {
                var dbPath = args.Length > 1 ? args[1] : DbManager.GetDbPath();
                SeedEquipment.SeedAsync(dbPath, null).GetAwaiter().GetResult();
                _logger?.Log($"Equipment database reseeded at {dbPath}");
                return;
            }
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            LogFatalException(ex, "Main");
            Console.Error.WriteLine($"Fatal error: {ex}");
            Environment.Exit(1);
        }
        finally
        {
            // Dispose service provider
            if (_serviceProvider is IDisposable disposableServiceProvider)
            {
                disposableServiceProvider.Dispose();
            }
        }
    }

    public static IServiceProvider ServiceProvider => _serviceProvider ?? throw new InvalidOperationException("Service provider not initialized");

    // ...existing code...

    private static void LogEnvironmentInfo()
    {
        try
        {
            var os = Environment.OSVersion;
            var is64 = Environment.Is64BitProcess;
            var clr = Environment.Version;
            var exe = Assembly.GetEntryAssembly()?.Location ?? "?";
            var cwd = Environment.CurrentDirectory;
            var user = Environment.UserName;
            var machine = Environment.MachineName;
            var now = DateTime.Now;
            var args = string.Join(" ", Environment.GetCommandLineArgs());
            _logger?.Log($"[INFO] ExanimaTools starting at {now:yyyy-MM-dd HH:mm:ss}");
            _logger?.Log($"[INFO] OS: {os}, 64bit: {is64}, .NET: {clr}");
            _logger?.Log($"[INFO] Executable: {exe}");
            _logger?.Log($"[INFO] Working dir: {cwd}");
            _logger?.Log($"[INFO] User: {user}@{machine}");
            _logger?.Log($"[INFO] Args: {args}");
        }
        catch (Exception ex)
        {
            _logger?.LogError("[ERROR] Failed to log environment info", ex);
        }
    }

    private static void LogFatalException(Exception? ex, string context)
    {
        try
        {
            var msg = $"[FATAL] Unhandled exception in {context}: {ex}";
            _logger?.LogError(msg, ex);
            try
            {
                System.IO.File.AppendAllText("fatal.log", $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {msg}\n");
            }
            catch { /* ignore file errors */ }
        }
        catch { /* ignore logging errors */ }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}

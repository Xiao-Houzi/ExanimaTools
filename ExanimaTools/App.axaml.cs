// No work should be done in this file without understanding the development practices.
// See: project-management/development_practices.md

using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ExanimaTools.Models;
using ExanimaTools.Services;
using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;

namespace ExanimaTools;

public partial class App : Application
{
    [Obsolete("Use dependency injection instead. This property is deprecated and will be removed in a future version.")]
    public static ILoggingService? LoggingServiceInstance { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        ILoggingService? logger = null;
        try
        {
            var serviceProvider = Program.ServiceProvider;
            logger = serviceProvider.GetRequiredService<ILoggingService>();

            logger.LogOperation("App.OnFrameworkInitializationCompleted", "Starting UI initialization");

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                logger.LogOperation("App.OnFrameworkInitializationCompleted", "Creating MainWindow");
                
                try
                {
                    desktop.MainWindow = serviceProvider.GetRequiredService<MainWindow>();
                    logger.LogOperation("App.OnFrameworkInitializationCompleted", "MainWindow created successfully");
                }
                catch (Exception mainWindowEx)
                {
                    logger.LogError("Failed to create MainWindow", mainWindowEx);
                    Console.Error.WriteLine($"Critical error creating MainWindow: {mainWindowEx}");
                    throw;
                }
            }
            
            logger.LogOperation("App.OnFrameworkInitializationCompleted", "UI initialization completed");
            base.OnFrameworkInitializationCompleted();
        }
        catch (Exception ex)
        {
            // Log using both logger (if available) and console
            var errorMsg = $"Fatal error during UI initialization: {ex}";
            
            if (logger != null)
            {
                try
                {
                    logger.LogError(errorMsg, ex);
                }
                catch
                {
                    // Ignore logging errors during critical failure
                }
            }
            
            Console.Error.WriteLine(errorMsg);
            
            // Also write to a fallback log file
            try
            {
                var fallbackLog = Path.Combine(Environment.CurrentDirectory, "logs", "ui_startup_error.log");
                Directory.CreateDirectory(Path.GetDirectoryName(fallbackLog)!);
                File.AppendAllText(fallbackLog, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {errorMsg}\n");
            }
            catch
            {
                // Ignore file write errors during critical failure
            }
            
            throw;
        }
    }
}
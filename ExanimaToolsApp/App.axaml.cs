using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ExanimaTools.Models;
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
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow(LoggingServiceInstance);
        }
        base.OnFrameworkInitializationCompleted();
    }
}
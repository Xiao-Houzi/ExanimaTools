using Avalonia;
using System;
using System.Threading.Tasks;
using ExanimaToolsApp;

namespace ExanimaTools;

class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        if (args.Length > 0 && args[0] == "dump")
        {
            var dbPath = DbManager.GetDbPath();
            var outPath = "equipment_dump.txt";
            DumpEquipmentDb.DumpAsync(dbPath, outPath).GetAwaiter().GetResult();
            Console.WriteLine($"Equipment dump written to {outPath}");
            return;
        }
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}

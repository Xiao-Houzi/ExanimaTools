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
        var arg0 = args.Length > 0 ? args[0].ToLowerInvariant() : string.Empty;
        if (arg0 == "dump" || arg0 == "dumpequipmentdb")
        {
            var dbPath = args.Length > 1 ? args[1] : DbManager.GetDbPath();
            var outPath = args.Length > 2 ? args[2] : "equipment_dump.txt";
            DumpEquipmentDb.DumpAsync(dbPath, outPath).GetAwaiter().GetResult();
            Console.WriteLine($"Equipment dump written to {outPath}");
            return;
        }
        if (arg0 == "seed" || arg0 == "seedequipment")
        {
            var dbPath = args.Length > 1 ? args[1] : DbManager.GetDbPath();
            SeedEquipment.SeedAsync(dbPath, null).GetAwaiter().GetResult();
            Console.WriteLine($"Equipment database reseeded at {dbPath}");
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

// No work should be done in this file without understanding the development practices.
// See: project-management/development_practices.md

using System;
using System.IO;
using System.Threading.Tasks;
using ExanimaTools.Persistence;
using ExanimaTools.Models;

namespace ExanimaTools;

public static class DumpEquipmentDb
{
    public static async Task DumpAsync(string? dbPath, string outPath, ILoggingService? logger = null, bool useDevDb = true)
    {
        // If dbPath is not provided, select based on mode
        string resolvedDbPath;
        if (!string.IsNullOrWhiteSpace(dbPath))
        {
            resolvedDbPath = dbPath;
        }
        else
        {
            resolvedDbPath = DbManager.GetDbPath();
        }
        logger ??= new FileLoggingService("logs");
        logger.LogOperation("DumpEquipmentDb", $"Using DB path: {resolvedDbPath}");
        var repo = DbManager.GetEquipmentRepository(logger);
        var all = await repo.GetAllAsync();
        logger.LogOperation("DumpEquipmentDb", $"Found {all.Count} equipment items in database.");
        using var writer = new StreamWriter(outPath, false);
        writer.WriteLine($"Equipment DB Dump ({DateTime.Now})");
        writer.WriteLine($"Count: {all.Count}");
        foreach (var eq in all)
        {
            var eqSummary = $"- Id: {eq.Id}, Name: {eq.Name}, Type: {eq.Type}, Category: {eq.Category}, Subcategory: {eq.Subcategory}, Rank: {eq.Rank}, Points: {eq.Points}, Weight: {eq.Weight}, Desc: {eq.Description}";
            writer.WriteLine(eqSummary);
            logger.LogOperation("DumpEquipmentDb.Equipment", eqSummary);
            if (eq.Stats != null && eq.Stats.Count > 0)
            {
                writer.WriteLine($"  Stats:");
                logger.LogOperation("DumpEquipmentDb.EquipmentStats", $"{eq.Name} stats:");
                foreach (var stat in eq.Stats)
                {
                    var statLine = $"    {stat.Key}: {stat.Value}";
                    writer.WriteLine(statLine);
                    logger.LogOperation("DumpEquipmentDb.EquipmentStat", statLine);
                }
            }
            else
            {
                writer.WriteLine("  Stats: (none)");
                logger.LogOperation("DumpEquipmentDb.EquipmentStats", $"{eq.Name} has no stats.");
            }
        }
    }
}

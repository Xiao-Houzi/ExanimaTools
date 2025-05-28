using System;
using System.IO;
using System.Threading.Tasks;
using ExanimaTools.Persistence;
using ExanimaTools.Models;
using ExanimaToolsApp;

namespace ExanimaTools;

public static class DumpEquipmentDb
{
    public static async Task DumpAsync(string? dbPath, string outPath)
    {
        dbPath ??= DbManager.GetDbPath();
        var repo = new EquipmentRepository($"Data Source={dbPath}");
        var all = await repo.GetAllAsync();
        using var writer = new StreamWriter(outPath, false);
        writer.WriteLine($"Equipment DB Dump ({DateTime.Now})");
        writer.WriteLine($"Count: {all.Count}");
        foreach (var eq in all)
        {
            writer.WriteLine($"- Id: {eq.Id}, Name: {eq.Name}, Type: {eq.Type}, Category: {eq.Category}, Subcategory: {eq.Subcategory}, Rank: {eq.Rank}, Points: {eq.Points}, Weight: {eq.Weight}, Desc: {eq.Description}");
        }
    }
}

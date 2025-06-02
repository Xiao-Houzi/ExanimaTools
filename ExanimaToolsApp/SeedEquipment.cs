using System.Threading.Tasks;
using ExanimaTools.Models;
using ExanimaTools.Persistence;
using System;
using System.Collections.Generic;
using ExanimaToolsApp;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;

public static class SeedEquipment
{
    public static async Task SeedAsync(string? dbPath = null, ExanimaTools.Models.ILoggingService? logger = null)
    {
        dbPath ??= DbManager.GetDbPath();
        logger?.LogOperation("SeedEquipment", $"Seeding equipment to DB: {dbPath}");
        var repo = new EquipmentRepository($"Data Source={dbPath}", logger ?? new FileLoggingService("logs"));
        try
        {
            // Load equipment from JSON file
            var jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "project-management", "equipment_seed.json");
            if (!File.Exists(jsonPath))
            {
                logger?.LogError($"SeedEquipment: equipment_seed.json not found at {jsonPath}");
                return;
            }
            var json = await File.ReadAllTextAsync(jsonPath);
            var items = JsonSerializer.Deserialize<List<JsonEquipmentSeed>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (items == null)
            {
                logger?.LogError("SeedEquipment: Failed to parse equipment_seed.json");
                return;
            }
            logger?.LogOperation("SeedEquipment", $"Loaded {items.Count} equipment items from JSON");
            await repo.DeleteAllAsync();
            foreach (var item in items)
            {
                var eq = item.ToEquipmentPiece();
                logger?.LogOperation("SeedEquipment", $"Adding: {eq.Name}");
                await repo.AddAsync(eq);
            }
            logger?.LogOperation("SeedEquipment", "Seeding complete");
            var all = await repo.GetAllAsync();
            logger?.LogOperation("SeedEquipment", $"Row count after seeding: {all.Count}");
            System.IO.File.AppendAllText("repo_debug.log", $"[DEBUG][SeedEquipment] Row count after seeding: {all.Count}\n");
        }
        catch (Exception ex)
        {
            logger?.LogError($"[SeedEquipment] Error: {ex.Message}", ex);
            System.Diagnostics.Debug.WriteLine($"[SeedEquipment] Error: {ex.Message}\n{ex.StackTrace}");
            File.AppendAllText("seed_error.log", $"[{DateTime.Now}] {ex.Message}\n{ex.StackTrace}\n");
        }
    }

    // Helper class for JSON mapping
    private class JsonEquipmentSeed
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int MinRank { get; set; }
        public string? Quality { get; set; }
        public string? Condition { get; set; }
        public Dictionary<string, float> Stats { get; set; } = new();
        public EquipmentPiece ToEquipmentPiece()
        {
            var eq = new EquipmentPiece(null!)
            {
                Name = Name,
                Type = ParseType(Type),
                Rank = (Rank)MinRank,
                Quality = ParseQuality(Quality),
                Condition = ParseCondition(Condition),
                Stats = new Dictionary<StatType, float>(),
                // Optionally map Category/Subcategory/Description if present in JSON
            };
            foreach (var kv in Stats)
            {
                if (Enum.TryParse<StatType>(kv.Key, true, out var statType))
                    eq.Stats[statType] = kv.Value;
            }
            return eq;
        }
        private EquipmentQuality ParseQuality(string? quality)
        {
            if (!string.IsNullOrWhiteSpace(quality) && Enum.TryParse<EquipmentQuality>(quality.Replace(" ", ""), true, out var result))
                return result;
            return EquipmentQuality.Common;
        }
        private EquipmentCondition ParseCondition(string? condition)
        {
            if (!string.IsNullOrWhiteSpace(condition) && Enum.TryParse<EquipmentCondition>(condition.Replace(" ", ""), true, out var result))
                return result;
            return EquipmentCondition.Good;
        }
        private EquipmentType ParseType(string type)
        {
            if (type.Contains("Weapon", StringComparison.OrdinalIgnoreCase)) return EquipmentType.Weapon;
            if (type.Contains("Armour", StringComparison.OrdinalIgnoreCase) || type.Contains("Armor", StringComparison.OrdinalIgnoreCase)) return EquipmentType.Armour;
            if (type.Contains("Shield", StringComparison.OrdinalIgnoreCase)) return EquipmentType.Shield;
            // Fallback: try to parse
            if (Enum.TryParse<EquipmentType>(type, true, out var result)) return result;
            return EquipmentType.Weapon;
        }
    }
}

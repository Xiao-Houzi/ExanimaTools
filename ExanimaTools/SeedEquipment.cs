// SeedEquipment.cs
// Provides logic to seed the equipment database from a JSON file.
// No work should be done in this file without understanding the development practices.
// See: project-management/development_practices.md

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using ExanimaTools.Models;
using ExanimaTools.Persistence;

namespace ExanimaTools
{
    /// <summary>
    /// Provides methods to seed the equipment database from a JSON seed file.
    /// </summary>
    public static class SeedEquipment
    {
        /// <summary>
        /// Seeds the equipment database from equipment_seed.json. Overwrites all existing equipment.
        /// </summary>
        /// <param name="dbPath">Optional path to the SQLite database file. Defaults to exanima_tools.db in the output directory.</param>
        /// <param name="logger">Optional logger for diagnostics.</param>
        public static async Task SeedAsync(string? dbPath = null, ExanimaTools.Models.ILoggingService? logger = null)
        {
            logger ??= new FileLoggingService("logs");
            logger.LogOperation("SeedEquipment", "Seeding process started.");
            var repo = DbManager.GetEquipmentRepository(logger);
            var dbPathUsed = DbManager.GetDbPath();
            logger.LogOperation("SeedEquipment", $"[DEBUG] Using DB path: {dbPathUsed}");
            try
            {
                var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "DevAssets", "equipment_seed.json");
                logger.LogOperation("SeedEquipment", $"[DEBUG] Using JSON seed path: {jsonPath}");
                if (!File.Exists(jsonPath))
                {
                    logger.LogError($"SeedEquipment: equipment_seed.json not found at {jsonPath}");
                    return;
                }
                string json;
                try {
                    json = await File.ReadAllTextAsync(jsonPath);
                } catch (Exception ex) {
                    logger.LogError($"SeedEquipment: Failed to read equipment_seed.json: {ex.Message}", ex);
                    return;
                }
                List<JsonEquipmentSeed>? items = null;
                try {
                    items = JsonSerializer.Deserialize<List<JsonEquipmentSeed>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                } catch (Exception ex) {
                    logger.LogError($"SeedEquipment: Failed to parse equipment_seed.json: {ex.Message}", ex);
                    return;
                }
                if (items == null)
                {
                    logger.LogError("SeedEquipment: Failed to parse equipment_seed.json (null result)");
                    return;
                }
                logger.LogOperation("SeedEquipment", $"Loaded {items.Count} equipment items from JSON");
                
                // Deduplicate items by name - take the first occurrence of each name
                var uniqueItems = items.GroupBy(item => item.Name)
                                      .Select(group => group.First())
                                      .ToList();
                logger.LogOperation("SeedEquipment", $"Deduplicated to {uniqueItems.Count} unique equipment items");
                
                // Generate rank variants to ensure all ranks have equipment available
                var expandedItems = new List<JsonEquipmentSeed>();
                foreach (var item in uniqueItems)
                {
                    // Always create an Inept (rank 0) version for basic equipment access
                    var ineptVersion = CloneItemForRank(item, 0, "Crude");
                    expandedItems.Add(ineptVersion);
                    
                    // Add the original item at its intended rank
                    if (item.MinRank > 0)
                    {
                        expandedItems.Add(item);
                    }
                    
                    // For weapons and shields, create additional quality variants at higher ranks
                    if (item.TopType?.Contains("Weapon") == true || item.TopType?.Contains("Shield") == true)
                    {
                        if (item.MinRank < 3) // Add a better version for higher ranks
                        {
                            var betterVersion = CloneItemForRank(item, Math.Max(item.MinRank + 2, 3), "Well Made");
                            expandedItems.Add(betterVersion);
                        }
                    }
                }
                
                logger.LogOperation("SeedEquipment", $"Generated {expandedItems.Count} rank-variant equipment items");
                
                await repo.DeleteAllAsync();
                int addCount = 0;
                int skipCount = 0;
                foreach (var item in expandedItems)
                {
                    EquipmentPiece? eq = null;
                    try {
                        eq = item.ToEquipmentPiece(logger);
                    } catch (Exception ex) {
                        logger.LogError($"SeedEquipment: Exception converting item '{item.Name}': {ex.Message}", ex);
                        skipCount++;
                        continue;
                    }
                    if (eq == null)
                    {
                        logger.LogError($"SeedEquipment: Skipped item with missing or invalid name: {item.Name}");
                        skipCount++;
                        continue;
                    }
                    try {
                        logger.LogOperation("SeedEquipment", $"Adding: {eq.Name}");
                        await repo.AddAsync(eq);
                        addCount++;
                    } catch (Exception ex) {
                        logger.LogError($"SeedEquipment: Failed to add '{eq.Name}': {ex.Message}", ex);
                        skipCount++;
                    }
                }
                logger.LogOperation("SeedEquipment", $"Seeding complete. Added {addCount} items, skipped {skipCount}.");
                var all = await repo.GetAllAsync();
                logger.LogOperation("SeedEquipment", $"Row count after seeding: {all.Count}");
            }
            catch (Exception ex)
            {
                logger.LogError($"[SeedEquipment] Error: {ex.Message}", ex);
                System.Diagnostics.Debug.WriteLine($"[SeedEquipment] Error: {ex.Message}\n{ex.StackTrace}");
                File.AppendAllText("seed_error.log", $"[{DateTime.Now}] {ex.Message}\n{ex.StackTrace}\n");
            }
        }

        /// <summary>
        /// Creates a clone of an equipment item for a specific rank with adjusted quality
        /// </summary>
        private static JsonEquipmentSeed CloneItemForRank(JsonEquipmentSeed original, int rank, string quality)
        {
            var clone = new JsonEquipmentSeed
            {
                Name = rank == 0 ? $"{original.Name} (Basic)" : 
                       rank > original.MinRank ? $"{original.Name} (Fine)" : original.Name,
                Type = original.Type,
                TopType = original.TopType,
                MinRank = rank,
                Quality = quality,
                Condition = original.Condition,
                Category = original.Category,
                Subcategory = original.Subcategory,
                Description = original.Description,
                Stats = new Dictionary<string, float>(original.Stats)
            };
            
            // Adjust stats based on rank/quality
            if (rank == 0) // Inept versions have reduced stats
            {
                foreach (var key in clone.Stats.Keys.ToList())
                {
                    if (key != "Weight") // Don't reduce weight
                    {
                        clone.Stats[key] *= 0.8f; // 20% reduction for basic versions
                    }
                }
            }
            else if (rank > original.MinRank) // Better versions have improved stats
            {
                foreach (var key in clone.Stats.Keys.ToList())
                {
                    if (key != "Weight") // Don't increase weight
                    {
                        clone.Stats[key] *= 1.2f; // 20% improvement for fine versions
                    }
                }
            }
            
            return clone;
        }

        // Helper class for JSON mapping
        /// <summary>
        /// Represents a single equipment item as loaded from the JSON seed file.
        /// </summary>
        private class JsonEquipmentSeed
        {
            public string Name { get; set; } = string.Empty;
            public string? Type { get; set; } // Category or subtype
            public string? TopType { get; set; } // Weapon/Armour/Shield (from table, if present)
            public int MinRank { get; set; }
            public string? Quality { get; set; }
            public string? Condition { get; set; }
            public string? Category { get; set; } // Not used in seed, but allow for future
            public string? Subcategory { get; set; }
            public string? Description { get; set; }
            public Dictionary<string, float> Stats { get; set; } = new();

            /// <summary>
            /// Converts this JSON seed object to an EquipmentPiece. Returns null if invalid.
            /// </summary>
            public EquipmentPiece? ToEquipmentPiece(ILoggingService logger)
            {
                if (string.IsNullOrWhiteSpace(Name))
                {
                    logger.LogError("SeedEquipment: Skipped item with missing or invalid name in JSON.");
                    return null;
                }
                // Split Type into Category/Subcategory if possible
                string cat = string.Empty, subcat = string.Empty;
                if (!string.IsNullOrWhiteSpace(Type) && Type.Contains("/")) {
                    var parts = Type.Split('/', 2);
                    cat = parts[0].Trim();
                    subcat = parts[1].Trim();
                } else {
                    cat = Type?.Trim() ?? string.Empty;
                    subcat = string.Empty;
                }
                var parsedType = ParseType(TopType, logger);
                logger.LogOperation("SEED_DEBUG", $"Name={Name}, TopType={TopType}, ParsedType={parsedType}");
                logger.LogOperation("SeedEquipment: ToEquipmentPiece", $"Name={Name}, TopType={TopType}, ParsedType={parsedType}");
                var eq = new EquipmentPiece(logger)
                {
                    Name = Name.Trim(),
                    Type = parsedType, // Always use TopType for EquipmentType
                    Rank = (Rank)MinRank,
                    Quality = ParseQuality(Quality, logger),
                    Condition = ParseCondition(Condition, logger),
                    Stats = new Dictionary<StatType, float>(),
                    Category = cat,
                    Subcategory = subcat,
                    Description = Description?.Trim() ?? string.Empty
                };
                foreach (var kv in Stats)
                {
                    if (Enum.TryParse<StatType>(kv.Key, true, out var statType))
                        eq.Stats[statType] = kv.Value;
                    else
                        logger.LogError($"SeedEquipment: Unknown stat type '{kv.Key}' for item '{Name}' in JSON.");
                }
                return eq;
            }
            private EquipmentQuality ParseQuality(string? quality, ILoggingService logger)
            {
                if (string.IsNullOrWhiteSpace(quality))
                {
                    logger.LogError($"SeedEquipment: Missing quality for item '{Name}' in JSON. Defaulting to Common.");
                    return EquipmentQuality.Common;
                }

                // Map known alternate quality names
                var normalizedQuality = quality.Replace(" ", "").ToLowerInvariant();
                switch (normalizedQuality)
                {
                    case "crude":
                        return EquipmentQuality.Poor;
                    case "decent":
                        return EquipmentQuality.Common;
                    case "wellmade":
                    case "well-made":
                        return EquipmentQuality.WellMade;
                    default:
                        // Try normal enum parsing
                        if (Enum.TryParse<EquipmentQuality>(quality.Replace(" ", ""), true, out var result))
                            return result;
                        
                        logger.LogError($"SeedEquipment: Unknown quality '{quality}' for item '{Name}' in JSON. Defaulting to Common.");
                        return EquipmentQuality.Common;
                }
            }
            private EquipmentCondition ParseCondition(string? condition, ILoggingService logger)
            {
                if (!string.IsNullOrWhiteSpace(condition) && Enum.TryParse<EquipmentCondition>(condition.Replace(" ", ""), true, out var result))
                    return result;
                logger.LogError($"SeedEquipment: Unknown or missing condition '{condition}' for item '{Name}' in JSON. Defaulting to Good.");
                return EquipmentCondition.Good;
            }
            private EquipmentType ParseType(string? type, ILoggingService logger)
            {
                if (string.IsNullOrWhiteSpace(type)) {
                    logger.LogError($"SeedEquipment: Missing TopType for item '{Name}' in JSON. Defaulting to Weapon.");
                    return EquipmentType.Weapon;
                }
                if (type.Contains("Weapon", StringComparison.OrdinalIgnoreCase)) return EquipmentType.Weapon;
                if (type.Contains("Armour", StringComparison.OrdinalIgnoreCase)) return EquipmentType.Armour;
                if (type.Contains("Shield", StringComparison.OrdinalIgnoreCase)) return EquipmentType.Shield;
                if (Enum.TryParse<EquipmentType>(type, true, out var result)) return result;
                logger.LogError($"SeedEquipment: Unknown TopType '{type}' for item '{Name}' in JSON. Defaulting to Weapon.");
                return EquipmentType.Weapon;
            }
        }
    }
}

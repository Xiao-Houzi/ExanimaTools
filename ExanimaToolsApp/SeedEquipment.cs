using System.Threading.Tasks;
using ExanimaTools.Models;
using ExanimaTools.Persistence;
using System;
using System.Collections.Generic;
using ExanimaToolsApp;
using System.IO;

public static class SeedEquipment
{
    public static async Task SeedAsync(string? dbPath = null, ExanimaTools.Models.ILoggingService? logger = null)
    {
        dbPath ??= DbManager.GetDbPath();
        logger?.LogOperation("SeedEquipment", $"Seeding equipment to DB: {dbPath}");
        var repo = new EquipmentRepository($"Data Source={dbPath}");
        try
        {
            logger?.LogOperation("SeedEquipment", "Starting weapon seeding");
            var weapons = new List<EquipmentPiece>
            {
                new EquipmentPiece { Name = "Rustedlongsword", Type = EquipmentType.Weapon, Category = "Sword", Subcategory = "Longsword", Rank = Rank.Inept, Points = 0, Weight = 0.2f, Description = "" },
                new EquipmentPiece { Name = "battered longsword", Type = EquipmentType.Weapon, Category = "Sword", Subcategory = "Longsword", Rank = Rank.Inept, Points = 0, Weight = 0f, Description = "" },
                new EquipmentPiece { Name = "worn hatchet", Type = EquipmentType.Weapon, Category = "Axe", Subcategory = "Handaxe", Rank = Rank.Inept, Points = 0, Weight = 0f, Description = "" },
                new EquipmentPiece { Name = "Rusty Sword", Type = EquipmentType.Weapon, Category = "Sword", Subcategory = "Arming Sword", Rank = Rank.Inept, Points = 5, Weight = 0.8f, Description = "A dull, rusty sword." },
                new EquipmentPiece { Name = "Old Axe", Type = EquipmentType.Weapon, Category = "Axe", Subcategory = "Battleaxe", Rank = Rank.Inept, Points = 6, Weight = 1.0f, Description = "A heavy, old axe." },
                new EquipmentPiece { Name = "Wooden Club", Type = EquipmentType.Weapon, Category = "Bludgeon", Subcategory = "Club", Rank = Rank.Inept, Points = 4, Weight = 0.7f, Description = "A simple wooden club." },
                new EquipmentPiece { Name = "Iron Dagger", Type = EquipmentType.Weapon, Category = "Dagger", Subcategory = "Stiletto", Rank = Rank.Inept, Points = 3, Weight = 0.3f, Description = "A short iron dagger." },
                new EquipmentPiece { Name = "Training Spear", Type = EquipmentType.Weapon, Category = "Polearm", Subcategory = "Spear", Rank = Rank.Inept, Points = 5, Weight = 1.2f, Description = "A blunt training spear." },
            };
            logger?.LogOperation("SeedEquipment", $"Weapons to seed: {weapons.Count}");
            var armours = new List<EquipmentPiece>
            {
                new EquipmentPiece { Name = "Old Leather Tunic", Type = EquipmentType.Armour, Category = "Body", Subcategory = "Shirt", Rank = Rank.Inept, Points = 2, Weight = 0.5f, Description = "Worn leather tunic." },
                new EquipmentPiece { Name = "Padded Cap", Type = EquipmentType.Armour, Category = "Head", Subcategory = "Padded Cap", Rank = Rank.Inept, Points = 1, Weight = 0.2f, Description = "Simple padded cap." },
                new EquipmentPiece { Name = "Ragged Trousers", Type = EquipmentType.Armour, Category = "Legs", Subcategory = "Trousers", Rank = Rank.Inept, Points = 1, Weight = 0.3f, Description = "Torn cloth trousers." },
                new EquipmentPiece { Name = "Worn Boots", Type = EquipmentType.Armour, Category = "Feet", Subcategory = "Boots", Rank = Rank.Inept, Points = 1, Weight = 0.4f, Description = "Old leather boots." },
                new EquipmentPiece { Name = "Cloth Gloves", Type = EquipmentType.Armour, Category = "Hands", Subcategory = "Gloves", Rank = Rank.Inept, Points = 1, Weight = 0.1f, Description = "Basic cloth gloves." },
            };
            logger?.LogOperation("SeedEquipment", $"Armours to seed: {armours.Count}");
            var aspirantWeapons = new List<EquipmentPiece>
            {
                new EquipmentPiece { Name = "Aspirant Sword", Type = EquipmentType.Weapon, Category = "Sword", Subcategory = "Arming Sword", Rank = Rank.Aspirant, Points = 8, Weight = 0.9f, Description = "A sharper sword for aspirants." },
                new EquipmentPiece { Name = "Aspirant Axe", Type = EquipmentType.Weapon, Category = "Axe", Subcategory = "Battleaxe", Rank = Rank.Aspirant, Points = 9, Weight = 1.1f, Description = "A better axe for aspirants." },
                new EquipmentPiece { Name = "Aspirant Club", Type = EquipmentType.Weapon, Category = "Bludgeon", Subcategory = "Club", Rank = Rank.Aspirant, Points = 7, Weight = 0.8f, Description = "A heavier club for aspirants." },
                new EquipmentPiece { Name = "Aspirant Dagger", Type = EquipmentType.Weapon, Category = "Dagger", Subcategory = "Stiletto", Rank = Rank.Aspirant, Points = 6, Weight = 0.4f, Description = "A sharper dagger for aspirants." },
                new EquipmentPiece { Name = "Aspirant Spear", Type = EquipmentType.Weapon, Category = "Polearm", Subcategory = "Spear", Rank = Rank.Aspirant, Points = 8, Weight = 1.3f, Description = "A pointed spear for aspirants." },
            };
            logger?.LogOperation("SeedEquipment", $"Aspirant weapons to seed: {aspirantWeapons.Count}");
            var aspirantArmours = new List<EquipmentPiece>
            {
                new EquipmentPiece { Name = "Aspirant Leather Vest", Type = EquipmentType.Armour, Category = "Body", Subcategory = "Shirt", Rank = Rank.Aspirant, Points = 4, Weight = 0.6f, Description = "Sturdier leather vest." },
                new EquipmentPiece { Name = "Aspirant Cap", Type = EquipmentType.Armour, Category = "Head", Subcategory = "Padded Cap", Rank = Rank.Aspirant, Points = 2, Weight = 0.25f, Description = "Better padded cap." },
                new EquipmentPiece { Name = "Aspirant Trousers", Type = EquipmentType.Armour, Category = "Legs", Subcategory = "Trousers", Rank = Rank.Aspirant, Points = 2, Weight = 0.35f, Description = "Reinforced trousers." },
                new EquipmentPiece { Name = "Aspirant Boots", Type = EquipmentType.Armour, Category = "Feet", Subcategory = "Boots", Rank = Rank.Aspirant, Points = 2, Weight = 0.45f, Description = "Sturdier boots." },
                new EquipmentPiece { Name = "Aspirant Gloves", Type = EquipmentType.Armour, Category = "Hands", Subcategory = "Gloves", Rank = Rank.Aspirant, Points = 2, Weight = 0.15f, Description = "Reinforced gloves." },
            };
            logger?.LogOperation("SeedEquipment", $"Aspirant armours to seed: {aspirantArmours.Count}");
            foreach (var eq in weapons) { logger?.LogOperation("SeedEquipment", $"Adding: {eq.Name}"); await repo.AddAsync(eq); }
            foreach (var eq in armours) { logger?.LogOperation("SeedEquipment", $"Adding: {eq.Name}"); await repo.AddAsync(eq); }
            foreach (var eq in aspirantWeapons) { logger?.LogOperation("SeedEquipment", $"Adding: {eq.Name}"); await repo.AddAsync(eq); }
            foreach (var eq in aspirantArmours) { logger?.LogOperation("SeedEquipment", $"Adding: {eq.Name}"); await repo.AddAsync(eq); }
            logger?.LogOperation("SeedEquipment", "Seeding complete");
        }
        catch (Exception ex)
        {
            logger?.LogError($"[SeedEquipment] Error: {ex.Message}", ex);
            System.Diagnostics.Debug.WriteLine($"[SeedEquipment] Error: {ex.Message}\n{ex.StackTrace}");
            File.AppendAllText("seed_error.log", $"[{DateTime.Now}] {ex.Message}\n{ex.StackTrace}\n");
        }
    }
}

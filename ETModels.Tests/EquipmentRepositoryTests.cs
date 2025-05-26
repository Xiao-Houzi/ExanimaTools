using ExanimaTools.Models;
using ExanimaTools.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ETModels.Tests;

[TestClass]
public class EquipmentRepositoryTests
{
    private string _dbPath = "TestEquipment.db";
    private string _connectionString => $"Data Source={_dbPath}";

    [TestInitialize]
    public void Init()
    {
        // Only try to delete the test DB in the current directory (test runner output)
        if (File.Exists(_dbPath))
        {
            try { File.Delete(_dbPath); } catch { /* ignore if locked */ }
        }
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = @"CREATE TABLE IF NOT EXISTS Equipment (
            Name TEXT PRIMARY KEY,
            Type INTEGER,
            Description TEXT
        )";
        cmd.ExecuteNonQuery();
    }

    [TestMethod]
    public async Task AddAndGetAllAsync_Works()
    {
        var repo = new EquipmentRepository(_connectionString);
        var eq = new EquipmentPiece { Name = "Sword", Type = EquipmentType.Weapon, Description = "Sharp" };
        await repo.AddAsync(eq);
        List<EquipmentPiece> all = await repo.GetAllAsync();
        Assert.AreEqual(1, all.Count);
        Assert.AreEqual("Sword", all[0].Name);
        Assert.AreEqual(EquipmentType.Weapon, all[0].Type);
        Assert.AreEqual("Sharp", all[0].Description);
    }

    [TestMethod]
    public async Task UpdateAndDeleteAsync_Works()
    {
        var repo = new EquipmentRepository(_connectionString);
        var eq = new EquipmentPiece { Name = "Axe", Type = EquipmentType.Weapon, Description = "Heavy" };
        await repo.AddAsync(eq);
        // Update
        eq.Description = "Very Heavy";
        await repo.UpdateAsync(eq);
        var updated = await repo.GetByIdAsync(eq.Name);
        Assert.IsNotNull(updated);
        Assert.AreEqual("Very Heavy", updated.Description);
        // Delete
        await repo.DeleteAsync(eq.Name);
        var all = await repo.GetAllAsync();
        Assert.AreEqual(0, all.Count);
    }

    [TestMethod]
    public async Task GetByIdAsync_Works()
    {
        var repo = new EquipmentRepository(_connectionString);
        var eq = new EquipmentPiece { Name = "Mace", Type = EquipmentType.Weapon, Description = "Blunt" };
        await repo.AddAsync(eq);
        var found = await repo.GetByIdAsync("Mace");
        Assert.IsNotNull(found);
        Assert.AreEqual("Mace", found.Name);
        Assert.AreEqual(EquipmentType.Weapon, found.Type);
    }
}

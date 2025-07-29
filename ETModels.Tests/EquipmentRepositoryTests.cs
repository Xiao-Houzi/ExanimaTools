using ExanimaTools;
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
    private class DummyLogger : ILoggingService
    {
        public void Log(string message) { }
        public void LogOperation(string operation, string? details = null) { }
        public void LogError(string message, System.Exception? ex = null) { }
        public void LogInformation(string message) { }
    }

    private string _dbPath = $"TestEquipment_{System.Guid.NewGuid()}.db";
    private string _connectionString => $"Data Source={_dbPath}";
    private SqliteConnection? _connection;

    [TestInitialize]
    public void Init()
    {
        if (File.Exists(_dbPath))
        {
            try { File.Delete(_dbPath); } catch { /* ignore if locked */ }
        }
        DbManager.SetDbPath(_dbPath);
        _connection = new SqliteConnection(_connectionString);
        _connection.Open();
    }

    [TestCleanup]
    public void Cleanup()
    {
        _connection?.Dispose();
        if (File.Exists(_dbPath))
        {
            try { File.Delete(_dbPath); } catch { /* ignore if locked */ }
        }
    }

    private EquipmentRepository GetTestEquipmentRepository()
    {
        // Use DummyLogger to avoid file conflicts during testing
        return DbManager.GetEquipmentRepository(new DummyLogger(), _connection);
    }

    [TestMethod]
    public async Task AddAndGetAllAsync_Works()
    {
        var repo = GetTestEquipmentRepository();
        var eq = new EquipmentPiece { Name = "Sword", Type = EquipmentType.Weapon, Description = "Sharp", Category = "Weapon", Subcategory = "Swords" };
        await repo.AddAsync(eq);
        List<EquipmentPiece> all = await repo.GetAllAsync();
        Assert.AreEqual(1, all.Count);
        Assert.AreEqual("Sword", all[0].Name);
        Assert.AreEqual(EquipmentType.Weapon, all[0].Type);
        Assert.AreEqual("Sharp", all[0].Description);
        Assert.AreEqual("Weapon", all[0].Category);
        Assert.AreEqual("Swords", all[0].Subcategory);
        // New: check by Id
        var byId = await repo.GetByIdAsync(eq.Id);
        Assert.IsNotNull(byId);
        Assert.AreEqual("Sword", byId.Name);
    }

    [TestMethod]
    public async Task UpdateAndDeleteAsync_Works()
    {
        var repo = GetTestEquipmentRepository();
        var eq = new EquipmentPiece { Name = "Axe", Type = EquipmentType.Weapon, Description = "Heavy", Category = "Weapon", Subcategory = "Axes" };
        await repo.AddAsync(eq);
        // Update
        eq.Description = "Very Heavy";
        eq.Category = "Weapon";
        eq.Subcategory = "Axes";
        await repo.UpdateAsync(eq);
        var updated = await repo.GetByIdAsync(eq.Id);
        Assert.IsNotNull(updated);
        Assert.AreEqual("Very Heavy", updated.Description);
        Assert.AreEqual("Weapon", updated.Category);
        Assert.AreEqual("Axes", updated.Subcategory);
        // Delete
        await repo.DeleteAsync(eq.Id);
        var all = await repo.GetAllAsync();
        Assert.AreEqual(0, all.Count);
    }

    [TestMethod]
    public async Task GetByIdAsync_Works()
    {
        var repo = GetTestEquipmentRepository();
        var eq = new EquipmentPiece { Name = "Mace", Type = EquipmentType.Weapon, Description = "Blunt", Category = "Weapon", Subcategory = "Bludgeons" };
        await repo.AddAsync(eq);
        var found = await repo.GetByIdAsync(eq.Id);
        Assert.IsNotNull(found);
        Assert.AreEqual("Mace", found.Name);
        Assert.AreEqual(EquipmentType.Weapon, found.Type);
        Assert.AreEqual("Weapon", found.Category);
        Assert.AreEqual("Bludgeons", found.Subcategory);
    }
}

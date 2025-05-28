using ExanimaTools.Models;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace ExanimaTools.Persistence;

public class ArsenalRepository
{
    private readonly string _connectionString;
    public ArsenalRepository(string connectionString)
    {
        _connectionString = connectionString;
        EnsureTableExists();
    }

    private void EnsureTableExists()
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        // Change: Add Id as PK, EquipmentId is just a value (allows duplicates)
        cmd.CommandText = @"CREATE TABLE IF NOT EXISTS Arsenal (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            EquipmentId INTEGER NOT NULL
        )";
        cmd.ExecuteNonQuery();
    }

    public async Task<Arsenal> GetArsenalAsync(EquipmentRepository equipmentRepo)
    {
        var arsenal = new Arsenal();
        using var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT EquipmentId FROM Arsenal";
        using var reader = await cmd.ExecuteReaderAsync();
        var ids = new List<int>();
        while (await reader.ReadAsync())
            ids.Add(reader.GetInt32(0));
        // Load full equipment objects
        var allEquipment = await equipmentRepo.GetAllAsync();
        foreach (var id in ids)
        {
            var eq = allEquipment.FirstOrDefault(e => e.Id == id);
            if (eq != null)
                arsenal.AddEquipment(eq); // AddEquipment will be updated to allow duplicates
        }
        return arsenal;
    }

    public async Task AddToArsenalAsync(int equipmentId)
    {
        using var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync();
        var cmd = conn.CreateCommand();
        // Change: No longer ignore duplicates
        cmd.CommandText = "INSERT INTO Arsenal (EquipmentId) VALUES ($id)";
        cmd.Parameters.AddWithValue("$id", equipmentId);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task RemoveFromArsenalAsync(int equipmentId)
    {
        using var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync();
        var cmd = conn.CreateCommand();
        // Change: Only remove one instance (the lowest Id)
        cmd.CommandText = "DELETE FROM Arsenal WHERE Id = (SELECT Id FROM Arsenal WHERE EquipmentId = $id LIMIT 1)";
        cmd.Parameters.AddWithValue("$id", equipmentId);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task SetArsenalAsync(IEnumerable<int> equipmentIds)
    {
        using var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync();
        using var tx = conn.BeginTransaction();
        var clearCmd = conn.CreateCommand();
        clearCmd.CommandText = "DELETE FROM Arsenal";
        await clearCmd.ExecuteNonQueryAsync();
        foreach (var id in equipmentIds)
        {
            var cmd = conn.CreateCommand();
            // Remove OR IGNORE to allow duplicates
            cmd.CommandText = "INSERT INTO Arsenal (EquipmentId) VALUES ($id)";
            cmd.Parameters.AddWithValue("$id", id);
            await cmd.ExecuteNonQueryAsync();
        }
        tx.Commit();
    }
}

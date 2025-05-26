using ExanimaTools.Models;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExanimaTools.Persistence;

public class EquipmentRepository
{
    private readonly string _connectionString;

    public EquipmentRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<List<EquipmentPiece>> GetAllAsync()
    {
        var result = new List<EquipmentPiece>();
        using var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT Name, Type, Description, Category, Subcategory FROM Equipment";
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            result.Add(new EquipmentPiece
            {
                Name = reader.GetString(0),
                Type = (EquipmentType)reader.GetInt32(1),
                Description = reader.GetString(2),
                Category = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                Subcategory = reader.IsDBNull(4) ? string.Empty : reader.GetString(4)
            });
        }
        return result;
    }

    public async Task AddAsync(EquipmentPiece equipment)
    {
        using var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "INSERT INTO Equipment (Name, Type, Description, Category, Subcategory) VALUES ($name, $type, $desc, $cat, $subcat) ON CONFLICT(Name) DO UPDATE SET Type = excluded.Type, Description = excluded.Description, Category = excluded.Category, Subcategory = excluded.Subcategory";
        cmd.Parameters.AddWithValue("$name", equipment.Name);
        cmd.Parameters.AddWithValue("$type", (int)equipment.Type);
        cmd.Parameters.AddWithValue("$desc", equipment.Description);
        cmd.Parameters.AddWithValue("$cat", equipment.Category);
        cmd.Parameters.AddWithValue("$subcat", equipment.Subcategory);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task UpdateAsync(EquipmentPiece equipment)
    {
        using var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "UPDATE Equipment SET Type = $type, Description = $desc, Category = $cat, Subcategory = $subcat WHERE Name = $name";
        cmd.Parameters.AddWithValue("$name", equipment.Name);
        cmd.Parameters.AddWithValue("$type", (int)equipment.Type);
        cmd.Parameters.AddWithValue("$desc", equipment.Description);
        cmd.Parameters.AddWithValue("$cat", equipment.Category);
        cmd.Parameters.AddWithValue("$subcat", equipment.Subcategory);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task DeleteAsync(string name)
    {
        using var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "DELETE FROM Equipment WHERE Name = $name";
        cmd.Parameters.AddWithValue("$name", name);
        await cmd.ExecuteNonQueryAsync();
        // VACUUM to ensure deleted rows are removed from the file
        var vacuumCmd = conn.CreateCommand();
        vacuumCmd.CommandText = "VACUUM";
        await vacuumCmd.ExecuteNonQueryAsync();
    }

    public async Task<EquipmentPiece?> GetByIdAsync(string name)
    {
        using var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT Name, Type, Description, Category, Subcategory FROM Equipment WHERE Name = $name";
        cmd.Parameters.AddWithValue("$name", name);
        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new EquipmentPiece
            {
                Name = reader.GetString(0),
                Type = (EquipmentType)reader.GetInt32(1),
                Description = reader.GetString(2),
                Category = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                Subcategory = reader.IsDBNull(4) ? string.Empty : reader.GetString(4)
            };
        }
        return null;
    }
}

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
        EnsureTableExists();
    }

    private void EnsureTableExists()
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = @"CREATE TABLE IF NOT EXISTS Equipment (
            Name TEXT PRIMARY KEY,
            Type INTEGER,
            Description TEXT,
            Category TEXT,
            Subcategory TEXT,
            Rank INTEGER DEFAULT 0,
            Points INTEGER DEFAULT 0,
            Weight REAL DEFAULT 0
        )";
        cmd.ExecuteNonQuery();
    }

    public async Task<List<EquipmentPiece>> GetAllAsync()
    {
        var result = new List<EquipmentPiece>();
        using var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT Name, Type, Description, Category, Subcategory, Rank, Points, Weight FROM Equipment";
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var eq = new EquipmentPiece
            {
                Name = reader.GetString(0),
                Type = (EquipmentType)reader.GetInt32(1),
                Description = reader.GetString(2),
                Category = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                Subcategory = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                Rank = (Rank)(reader.IsDBNull(5) ? 0 : reader.GetInt32(5)),
                Points = reader.IsDBNull(6) ? 0 : reader.GetInt32(6),
                Weight = reader.IsDBNull(7) ? 0f : (float)reader.GetDouble(7)
            };
            result.Add(eq);
        }
        return result;
    }

    public async Task AddAsync(EquipmentPiece equipment)
    {
        using var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "INSERT INTO Equipment (Name, Type, Description, Category, Subcategory, Rank, Points, Weight) VALUES ($name, $type, $desc, $cat, $subcat, $rank, $points, $weight) ON CONFLICT(Name) DO UPDATE SET Type = excluded.Type, Description = excluded.Description, Category = excluded.Category, Subcategory = excluded.Subcategory, Rank = excluded.Rank, Points = excluded.Points, Weight = excluded.Weight";
        cmd.Parameters.AddWithValue("$name", equipment.Name);
        cmd.Parameters.AddWithValue("$type", (int)equipment.Type);
        cmd.Parameters.AddWithValue("$desc", equipment.Description);
        cmd.Parameters.AddWithValue("$cat", equipment.Category);
        cmd.Parameters.AddWithValue("$subcat", equipment.Subcategory);
        cmd.Parameters.AddWithValue("$rank", (int)equipment.Rank);
        cmd.Parameters.AddWithValue("$points", equipment.Points);
        cmd.Parameters.AddWithValue("$weight", equipment.Weight);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task UpdateAsync(EquipmentPiece equipment)
    {
        using var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "UPDATE Equipment SET Type = $type, Description = $desc, Category = $cat, Subcategory = $subcat, Rank = $rank, Points = $points, Weight = $weight WHERE Name = $name";
        cmd.Parameters.AddWithValue("$name", equipment.Name);
        cmd.Parameters.AddWithValue("$type", (int)equipment.Type);
        cmd.Parameters.AddWithValue("$desc", equipment.Description);
        cmd.Parameters.AddWithValue("$cat", equipment.Category);
        cmd.Parameters.AddWithValue("$subcat", equipment.Subcategory);
        cmd.Parameters.AddWithValue("$rank", (int)equipment.Rank);
        cmd.Parameters.AddWithValue("$points", equipment.Points);
        cmd.Parameters.AddWithValue("$weight", equipment.Weight);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<EquipmentPiece?> GetByIdAsync(string name)
    {
        using var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT Name, Type, Description, Category, Subcategory, Rank, Points, Weight FROM Equipment WHERE Name = $name";
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
                Subcategory = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                Rank = (Rank)(reader.IsDBNull(5) ? 0 : reader.GetInt32(5)),
                Points = reader.IsDBNull(6) ? 0 : reader.GetInt32(6),
                Weight = reader.IsDBNull(7) ? 0f : (float)reader.GetDouble(7)
            };
        }
        return null;
    }

    public async Task DeleteAsync(string name)
    {
        using var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "DELETE FROM Equipment WHERE Name = $name";
        cmd.Parameters.AddWithValue("$name", name);
        await cmd.ExecuteNonQueryAsync();
    }
}

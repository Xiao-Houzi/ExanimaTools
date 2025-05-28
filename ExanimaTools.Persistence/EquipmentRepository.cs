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
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Name TEXT UNIQUE,
            Type INTEGER,
            Description TEXT,
            Category TEXT,
            Subcategory TEXT,
            Rank INTEGER DEFAULT 0,
            Points INTEGER DEFAULT 0,
            Weight REAL DEFAULT 0
        )";
        cmd.ExecuteNonQuery();

        // Migration: Add missing columns if they do not exist
        var columns = new HashSet<string>();
        using (var pragmaCmd = conn.CreateCommand())
        {
            pragmaCmd.CommandText = "PRAGMA table_info(Equipment)";
            using var reader = pragmaCmd.ExecuteReader();
            while (reader.Read())
            {
                columns.Add(reader.GetString(1)); // column name
            }
        }
        if (!columns.Contains("Rank"))
        {
            using var alter = conn.CreateCommand();
            alter.CommandText = "ALTER TABLE Equipment ADD COLUMN Rank INTEGER DEFAULT 0";
            alter.ExecuteNonQuery();
        }
        if (!columns.Contains("Points"))
        {
            using var alter = conn.CreateCommand();
            alter.CommandText = "ALTER TABLE Equipment ADD COLUMN Points INTEGER DEFAULT 0";
            alter.ExecuteNonQuery();
        }
        if (!columns.Contains("Weight"))
        {
            using var alter = conn.CreateCommand();
            alter.CommandText = "ALTER TABLE Equipment ADD COLUMN Weight REAL DEFAULT 0";
            alter.ExecuteNonQuery();
        }
    }

    public async Task<List<EquipmentPiece>> GetAllAsync()
    {
        var result = new List<EquipmentPiece>();
        using var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT Id, Name, Type, Description, Category, Subcategory, Rank, Points, Weight FROM Equipment";
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var eq = new EquipmentPiece
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Type = (EquipmentType)reader.GetInt32(2),
                Description = reader.GetString(3),
                Category = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                Subcategory = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                Rank = (Rank)(reader.IsDBNull(6) ? 0 : reader.GetInt32(6)),
                Points = reader.IsDBNull(7) ? 0 : reader.GetInt32(7),
                Weight = reader.IsDBNull(8) ? 0f : (float)reader.GetDouble(8)
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
        cmd.CommandText = "INSERT INTO Equipment (Name, Type, Description, Category, Subcategory, Rank, Points, Weight) VALUES ($name, $type, $desc, $cat, $subcat, $rank, $points, $weight); SELECT last_insert_rowid();";
        cmd.Parameters.AddWithValue("$name", equipment.Name);
        cmd.Parameters.AddWithValue("$type", (int)equipment.Type);
        cmd.Parameters.AddWithValue("$desc", equipment.Description);
        cmd.Parameters.AddWithValue("$cat", equipment.Category);
        cmd.Parameters.AddWithValue("$subcat", equipment.Subcategory);
        cmd.Parameters.AddWithValue("$rank", (int)equipment.Rank);
        cmd.Parameters.AddWithValue("$points", equipment.Points);
        cmd.Parameters.AddWithValue("$weight", equipment.Weight);
        var id = (long)await cmd.ExecuteScalarAsync();
        equipment.Id = (int)id;
    }

    public async Task UpdateAsync(EquipmentPiece equipment)
    {
        using var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "UPDATE Equipment SET Name = $name, Type = $type, Description = $desc, Category = $cat, Subcategory = $subcat, Rank = $rank, Points = $points, Weight = $weight WHERE Id = $id";
        cmd.Parameters.AddWithValue("$id", equipment.Id);
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

    public async Task<EquipmentPiece?> GetByIdAsync(int id)
    {
        using var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT Id, Name, Type, Description, Category, Subcategory, Rank, Points, Weight FROM Equipment WHERE Id = $id";
        cmd.Parameters.AddWithValue("$id", id);
        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new EquipmentPiece
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Type = (EquipmentType)reader.GetInt32(2),
                Description = reader.GetString(3),
                Category = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                Subcategory = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                Rank = (Rank)(reader.IsDBNull(6) ? 0 : reader.GetInt32(6)),
                Points = reader.IsDBNull(7) ? 0 : reader.GetInt32(7),
                Weight = reader.IsDBNull(8) ? 0f : (float)reader.GetDouble(8)
            };
        }
        return null;
    }

    public async Task DeleteAsync(int id)
    {
        using var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "DELETE FROM Equipment WHERE Id = $id";
        cmd.Parameters.AddWithValue("$id", id);
        await cmd.ExecuteNonQueryAsync();
    }
}

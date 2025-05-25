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
        cmd.CommandText = "SELECT Name, Type, Description FROM Equipment";
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            result.Add(new EquipmentPiece
            {
                Name = reader.GetString(0),
                Type = (EquipmentType)reader.GetInt32(1),
                Description = reader.GetString(2)
            });
        }
        return result;
    }

    public async Task AddAsync(EquipmentPiece equipment)
    {
        using var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "INSERT INTO Equipment (Name, Type, Description) VALUES ($name, $type, $desc)";
        cmd.Parameters.AddWithValue("$name", equipment.Name);
        cmd.Parameters.AddWithValue("$type", (int)equipment.Type);
        cmd.Parameters.AddWithValue("$desc", equipment.Description);
        await cmd.ExecuteNonQueryAsync();
    }

    // Add UpdateAsync, DeleteAsync, and GetByIdAsync as needed
}

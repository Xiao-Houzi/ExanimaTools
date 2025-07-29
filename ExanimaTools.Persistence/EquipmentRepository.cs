// No work should be done in this file without understanding the development practices.
// See: project-management/development_practices.md

using ExanimaTools.Models;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ExanimaTools.Persistence;

public class EquipmentRepository
{
    private readonly string _connectionString;
    private readonly ILoggingService _logger;
    private readonly SqliteConnection? _externalConnection;

    public EquipmentRepository(string connectionString, ILoggingService logger)
    {
        _connectionString = connectionString;
        _logger = logger;
        EnsureTableExists();

        // Run migration helper for robust schema upgrades
        DbMigrationHelper.MigrateEquipmentTable(_connectionString);
    }

    public EquipmentRepository(SqliteConnection connection, ILoggingService logger)
    {
        _externalConnection = connection;
        _connectionString = connection.ConnectionString;
        _logger = logger;
        EnsureTableExists(connection);
        DbMigrationHelper.MigrateEquipmentTable(connection);
    }

    private void EnsureTableExists(SqliteConnection? externalConn = null)
    {
        var conn = externalConn ?? new SqliteConnection(_connectionString);
        var shouldDispose = externalConn == null;
        try {
            if (conn.State != System.Data.ConnectionState.Open)
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
                Weight REAL DEFAULT 0,
                Stats TEXT
            )";
            cmd.ExecuteNonQuery();
            _logger.LogOperation("EquipmentRepository", "Equipment table created or already exists.");

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
                try { alter.ExecuteNonQuery(); _logger.LogOperation("EquipmentRepository", "Added missing column: Rank"); } catch (SqliteException ex) { if (!ex.Message.Contains("duplicate column")) throw; }
                columns.Add("Rank");
            }
            if (!columns.Contains("Points"))
            {
                using var alter = conn.CreateCommand();
                alter.CommandText = "ALTER TABLE Equipment ADD COLUMN Points INTEGER DEFAULT 0";
                try { alter.ExecuteNonQuery(); _logger.LogOperation("EquipmentRepository", "Added missing column: Points"); } catch (SqliteException ex) { if (!ex.Message.Contains("duplicate column")) throw; }
                columns.Add("Points");
            }
            if (!columns.Contains("Weight"))
            {
                using var alter = conn.CreateCommand();
                alter.CommandText = "ALTER TABLE Equipment ADD COLUMN Weight REAL DEFAULT 0";
                try { alter.ExecuteNonQuery(); _logger.LogOperation("EquipmentRepository", "Added missing column: Weight"); } catch (SqliteException ex) { if (!ex.Message.Contains("duplicate column")) throw; }
                columns.Add("Weight");
            }
            // Add a new column for Stats (JSON)
            if (!columns.Contains("Stats"))
            {
                using var alter = conn.CreateCommand();
                alter.CommandText = "ALTER TABLE Equipment ADD COLUMN Stats TEXT DEFAULT '{}'";
                try { alter.ExecuteNonQuery(); _logger.LogOperation("EquipmentRepository", "Added missing column: Stats"); } catch (SqliteException ex) { if (!ex.Message.Contains("duplicate column")) throw; }
                columns.Add("Stats");
            }
            // Log the final table structure
            using (var tableCmd = conn.CreateCommand())
            {
                tableCmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table'";
                using var tableReader = tableCmd.ExecuteReader();
                var tables = new List<string>();
                while (tableReader.Read())
                    tables.Add(tableReader.GetString(0));
                _logger.LogOperation("EquipmentRepository", $"Current tables in DB: {string.Join(", ", tables)}");
            }
        } catch (Exception ex) {
            _logger.LogError($"[EnsureTableExists] Error: {ex.Message}", ex);
            throw;
        } finally {
            if (shouldDispose && conn.State == System.Data.ConnectionState.Open)
                conn.Close();
            if (shouldDispose)
                conn.Dispose();
        }
    }

    private static readonly JsonSerializerOptions StatsJsonOptions = new()
    {
        Converters = { new JsonStringEnumConverter() },
        PropertyNamingPolicy = null
    };

    private SqliteConnection GetConnection(out bool shouldDispose)
    {
        if (_externalConnection != null)
        {
            if (_externalConnection.State != System.Data.ConnectionState.Open)
                throw new InvalidOperationException("External connection must be open.");
            shouldDispose = false;
            return _externalConnection;
        }
        shouldDispose = true;
        return new SqliteConnection(_connectionString);
    }

    public async Task<List<EquipmentPiece>> GetAllAsync()
    {
        var result = new List<EquipmentPiece>();
        bool shouldDispose;
        var conn = GetConnection(out shouldDispose);
        try {
            if (conn.State != System.Data.ConnectionState.Open)
                await conn.OpenAsync();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, Name, Type, Description, Category, Subcategory, Rank, Points, Weight, Stats FROM Equipment";
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var statsJson = reader.IsDBNull(9) ? "{}" : reader.GetString(9);
                _logger.LogDebug($"[DEBUG][GetAllAsync] {reader.GetString(1)}: Weight={reader.GetDouble(8)}, StatsJson={statsJson}");
                var stats = new Dictionary<StatType, float>();
                try { stats = JsonSerializer.Deserialize<Dictionary<StatType, float>>(statsJson, StatsJsonOptions) ?? new Dictionary<StatType, float>(); } catch { _logger.LogDebug($"[DEBUG][GetAllAsync] Failed to deserialize stats for {reader.GetString(1)}: {statsJson}"); }
                _logger.LogDebug($"[DEBUG][GetAllAsync] {reader.GetString(1)}: StatsLoaded={string.Join(", ", stats.Select(kv => $"{kv.Key}={kv.Value}"))}");
                var weight = reader.IsDBNull(8) ? 0f : (float)reader.GetDouble(8);
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
                    Weight = weight,
                    Stats = stats
                };
                _logger.LogDebug($"[DEBUG][GetAllAsync] {eq.Name}: EquipmentPieceCreated Stats={string.Join(", ", eq.Stats.Select(kv => $"{kv.Key}={kv.Value}"))}");
                result.Add(eq);
            }
        } finally {
            if (shouldDispose) conn.Dispose();
        }
        return result;
    }

    public async Task AddAsync(EquipmentPiece equipment)
    {
        bool shouldDispose;
        var conn = GetConnection(out shouldDispose);
        try {
            if (conn.State != System.Data.ConnectionState.Open)
                await conn.OpenAsync();
            var statsJson = JsonSerializer.Serialize(equipment.Stats ?? new Dictionary<StatType, float>(), StatsJsonOptions);
            _logger.LogDebug($"[DEBUG][AddAsync] {equipment.Name}: Weight={equipment.Weight}, StatsJson={statsJson}, StatsCount={(equipment.Stats?.Count ?? 0)}");
            var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO Equipment (Name, Type, Description, Category, Subcategory, Rank, Points, Weight, Stats) VALUES ($name, $type, $desc, $cat, $subcat, $rank, $points, $weight, $stats); SELECT last_insert_rowid();";
            cmd.Parameters.AddWithValue("$name", equipment.Name);
            cmd.Parameters.AddWithValue("$type", (int)equipment.Type);
            cmd.Parameters.AddWithValue("$desc", equipment.Description);
            cmd.Parameters.AddWithValue("$cat", equipment.Category);
            cmd.Parameters.AddWithValue("$subcat", equipment.Subcategory);
            cmd.Parameters.AddWithValue("$rank", (int)equipment.Rank);
            cmd.Parameters.AddWithValue("$points", equipment.Points);
            cmd.Parameters.AddWithValue("$weight", equipment.Weight);
            cmd.Parameters.AddWithValue("$stats", statsJson);
            var result = await cmd.ExecuteScalarAsync();
            var id = result != null ? (long)result : 0L;
            equipment.Id = (int)id;
        } catch (Exception ex) {
            _logger.LogError($"[AddAsync] Error adding {equipment.Name}: {ex.Message}", ex);
            throw;
        } finally {
            if (shouldDispose && conn.State == System.Data.ConnectionState.Open)
                conn.Close();
            if (shouldDispose)
                conn.Dispose();
        }
    }

    public async Task UpdateAsync(EquipmentPiece equipment)
    {
        bool shouldDispose;
        var conn = GetConnection(out shouldDispose);
        try {
            if (conn.State != System.Data.ConnectionState.Open)
                await conn.OpenAsync();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Equipment SET Name = $name, Type = $type, Description = $desc, Category = $cat, Subcategory = $subcat, Rank = $rank, Points = $points, Weight = $weight, Stats = $stats WHERE Id = $id";
            cmd.Parameters.AddWithValue("$id", equipment.Id);
            cmd.Parameters.AddWithValue("$name", equipment.Name);
            cmd.Parameters.AddWithValue("$type", (int)equipment.Type);
            cmd.Parameters.AddWithValue("$desc", equipment.Description);
            cmd.Parameters.AddWithValue("$cat", equipment.Category);
            cmd.Parameters.AddWithValue("$subcat", equipment.Subcategory);
            cmd.Parameters.AddWithValue("$rank", (int)equipment.Rank);
            cmd.Parameters.AddWithValue("$points", equipment.Points);
            cmd.Parameters.AddWithValue("$weight", equipment.Weight);
            cmd.Parameters.AddWithValue("$stats", JsonSerializer.Serialize(equipment.Stats ?? new Dictionary<StatType, float>(), StatsJsonOptions));
            await cmd.ExecuteNonQueryAsync();
        } finally {
            if (shouldDispose && conn.State == System.Data.ConnectionState.Open)
                conn.Close();
            if (shouldDispose)
                conn.Dispose();
        }
    }

    public async Task<EquipmentPiece?> GetByIdAsync(int id)
    {
        bool shouldDispose;
        var conn = GetConnection(out shouldDispose);
        try {
            if (conn.State != System.Data.ConnectionState.Open)
                await conn.OpenAsync();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Id, Name, Type, Description, Category, Subcategory, Rank, Points, Weight, Stats FROM Equipment WHERE Id = $id";
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
                    Weight = reader.IsDBNull(8) ? 0f : (float)reader.GetDouble(8),
                    Stats = reader.IsDBNull(9) ? new Dictionary<StatType, float>() : JsonSerializer.Deserialize<Dictionary<StatType, float>>(reader.GetString(9)) ?? new Dictionary<StatType, float>()
                };
            }
            return null;
        } finally {
            if (shouldDispose && conn.State == System.Data.ConnectionState.Open)
                conn.Close();
            if (shouldDispose)
                conn.Dispose();
        }
    }

    public async Task DeleteAsync(int id)
    {
        bool shouldDispose;
        var conn = GetConnection(out shouldDispose);
        try {
            if (conn.State != System.Data.ConnectionState.Open)
                await conn.OpenAsync();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM Equipment WHERE Id = $id";
            cmd.Parameters.AddWithValue("$id", id);
            await cmd.ExecuteNonQueryAsync();
        } finally {
            if (shouldDispose && conn.State == System.Data.ConnectionState.Open)
                conn.Close();
            if (shouldDispose)
                conn.Dispose();
        }
    }

    public async Task DeleteAllAsync()
    {
        bool shouldDispose;
        var conn = GetConnection(out shouldDispose);
        try {
            if (conn.State != System.Data.ConnectionState.Open)
                await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM Equipment";
            await cmd.ExecuteNonQueryAsync();
        } finally {
            if (shouldDispose && conn.State == System.Data.ConnectionState.Open)
                conn.Close();
            if (shouldDispose)
                conn.Dispose();
        }
    }
}

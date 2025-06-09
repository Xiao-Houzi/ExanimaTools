using ExanimaTools.Models;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace ExanimaTools.Persistence;

public class ArsenalRepository
{
    private readonly string _connectionString;
    private readonly SqliteConnection? _externalConnection;

    public ArsenalRepository(string connectionString)
    {
        _connectionString = connectionString;
        EnsureTableExists();
    }

    public ArsenalRepository(SqliteConnection connection)
    {
        _externalConnection = connection;
        _connectionString = connection.ConnectionString;
        EnsureTableExists(connection);
    }

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

    private void EnsureTableExists(SqliteConnection? externalConn = null)
    {
        var conn = externalConn ?? new SqliteConnection(_connectionString);
        var shouldDispose = externalConn == null;
        try {
            if (conn.State != System.Data.ConnectionState.Open)
                conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"CREATE TABLE IF NOT EXISTS Arsenal (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                EquipmentId INTEGER NOT NULL
            )";
            cmd.ExecuteNonQuery();
        } finally {
            if (shouldDispose && conn.State == System.Data.ConnectionState.Open)
                conn.Close();
            if (shouldDispose)
                conn.Dispose();
        }
    }

    public async Task<Arsenal> GetArsenalAsync(EquipmentRepository equipmentRepo)
    {
        var arsenal = new Arsenal();
        bool shouldDispose;
        var conn = GetConnection(out shouldDispose);
        try {
            if (conn.State != System.Data.ConnectionState.Open)
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
        } finally {
            if (shouldDispose) conn.Dispose();
        }
        return arsenal;
    }

    public async Task AddToArsenalAsync(int equipmentId)
    {
        bool shouldDispose;
        var conn = GetConnection(out shouldDispose);
        try {
            if (conn.State != System.Data.ConnectionState.Open)
                await conn.OpenAsync();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO Arsenal (EquipmentId) VALUES ($id)";
            cmd.Parameters.AddWithValue("$id", equipmentId);
            await cmd.ExecuteNonQueryAsync();
        } finally {
            if (shouldDispose && conn.State == System.Data.ConnectionState.Open)
                conn.Close();
            if (shouldDispose)
                conn.Dispose();
        }
    }

    public async Task RemoveFromArsenalAsync(int equipmentId)
    {
        bool shouldDispose;
        var conn = GetConnection(out shouldDispose);
        try {
            if (conn.State != System.Data.ConnectionState.Open)
                await conn.OpenAsync();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM Arsenal WHERE Id = (SELECT Id FROM Arsenal WHERE EquipmentId = $id LIMIT 1)";
            cmd.Parameters.AddWithValue("$id", equipmentId);
            await cmd.ExecuteNonQueryAsync();
        } finally {
            if (shouldDispose && conn.State == System.Data.ConnectionState.Open)
                conn.Close();
            if (shouldDispose)
                conn.Dispose();
        }
    }

    public async Task SetArsenalAsync(IEnumerable<int> equipmentIds)
    {
        bool shouldDispose;
        var conn = GetConnection(out shouldDispose);
        try {
            if (conn.State != System.Data.ConnectionState.Open)
                await conn.OpenAsync();
            using var tx = conn.BeginTransaction();
            var clearCmd = conn.CreateCommand();
            clearCmd.CommandText = "DELETE FROM Arsenal";
            await clearCmd.ExecuteNonQueryAsync();
            foreach (var id in equipmentIds)
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = "INSERT INTO Arsenal (EquipmentId) VALUES ($id)";
                cmd.Parameters.AddWithValue("$id", id);
                await cmd.ExecuteNonQueryAsync();
            }
            tx.Commit();
        } finally {
            if (shouldDispose && conn.State == System.Data.ConnectionState.Open)
                conn.Close();
            if (shouldDispose)
                conn.Dispose();
        }
    }
}

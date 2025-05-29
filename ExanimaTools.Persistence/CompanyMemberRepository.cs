using ExanimaTools.Models;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

namespace ExanimaTools.Persistence;

public class CompanyMemberRepository
{
    private readonly string _connectionString;
    private readonly string? _dbFilePath;
    private readonly bool _reseedDb;
    private Task? _schemaInitTask;
    private readonly SqliteConnection? _externalConnection;

    public CompanyMemberRepository(string connectionString, bool reseedDb = false)
    {
        _connectionString = connectionString;
        _reseedDb = reseedDb;
        // Try to extract the DB file path for deletion logic
        if (connectionString.StartsWith("Data Source="))
        {
            _dbFilePath = connectionString.Substring("Data Source=".Length).Trim(';');
        }
        _schemaInitTask = InitializeSchemaAsync();
    }

    public CompanyMemberRepository(SqliteConnection connection)
    {
        _externalConnection = connection;
        _connectionString = connection.ConnectionString;
        _schemaInitTask = InitializeSchemaAsync();
    }

    private SqliteConnection GetConnection()
    {
        return _externalConnection ?? new SqliteConnection(_connectionString);
    }

    private static async Task EnsureConnectionOpenAsync(SqliteConnection conn)
    {
        if (conn.State != System.Data.ConnectionState.Open)
            await conn.OpenAsync();
    }

    public async Task<List<CompanyMember>> GetAllAsync()
    {
        if (_schemaInitTask != null) await _schemaInitTask;
        var result = new List<CompanyMember>();
        var conn = GetConnection();
        await EnsureConnectionOpenAsync(conn);
        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT Name, Role, Rank, Sex, Type FROM CompanyMembers";
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            result.Add(new CompanyMember
            {
                Name = reader.GetString(0),
                Role = (Role)reader.GetInt32(1),
                Rank = (Rank)reader.GetInt32(2),
                Sex = (Sex)reader.GetInt32(3),
                Type = (MemberType)reader.GetInt32(4)
            });
        }
        return result;
    }

    public async Task InitializeSchemaAsync()
    {
        if (_reseedDb && !string.IsNullOrEmpty(_dbFilePath) && File.Exists(_dbFilePath))
        {
            try { File.Delete(_dbFilePath); } catch { /* ignore if locked */ }
        }
        var conn = GetConnection();
        await EnsureConnectionOpenAsync(conn);
        var cmd = conn.CreateCommand();
        cmd.CommandText = @"
        CREATE TABLE IF NOT EXISTS CompanyMembers (
            Name TEXT PRIMARY KEY,
            Role INTEGER,
            Rank INTEGER,
            Sex INTEGER,
            Type INTEGER
        );
        CREATE TABLE IF NOT EXISTS EquipmentProfiles (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            CompanyMemberName TEXT,
            Rank INTEGER,
            Name TEXT,
            FOREIGN KEY(CompanyMemberName) REFERENCES CompanyMembers(Name)
        );
        CREATE TABLE IF NOT EXISTS EquippedItems (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            ProfileId INTEGER,
            Slot INTEGER,
            Layer INTEGER,
            EquipmentName TEXT,
            Type INTEGER,
            Description TEXT,
            Quality INTEGER,
            Condition INTEGER,
            StatsJson TEXT,
            FOREIGN KEY(ProfileId) REFERENCES EquipmentProfiles(Id)
        );
        ";
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task AddAsync(CompanyMember member)
    {
        if (_schemaInitTask != null) await _schemaInitTask;
        var conn = GetConnection();
        await EnsureConnectionOpenAsync(conn);
        using var tx = conn.BeginTransaction();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "INSERT INTO CompanyMembers (Name, Role, Rank, Sex, Type) VALUES ($name, $role, $rank, $sex, $type)";
        cmd.Parameters.AddWithValue("$name", member.Name);
        cmd.Parameters.AddWithValue("$role", (int)member.Role);
        cmd.Parameters.AddWithValue("$rank", (int)member.Rank);
        cmd.Parameters.AddWithValue("$sex", (int)member.Sex);
        cmd.Parameters.AddWithValue("$type", (int)member.Type);
        await cmd.ExecuteNonQueryAsync();
        foreach (var (rank, profile) in member.EquipmentProfiles)
        {
            var profileCmd = conn.CreateCommand();
            profileCmd.CommandText = "INSERT INTO EquipmentProfiles (CompanyMemberName, Rank, Name) VALUES ($cm, $rank, $name); SELECT last_insert_rowid();";
            profileCmd.Parameters.AddWithValue("$cm", member.Name);
            profileCmd.Parameters.AddWithValue("$rank", (int)rank);
            profileCmd.Parameters.AddWithValue("$name", profile.Name);
            var profileId = (long)await profileCmd.ExecuteScalarAsync();
            foreach (var (slot, items) in profile.EquippedItems)
            {
                foreach (var item in items)
                {
                    var itemCmd = conn.CreateCommand();
                    itemCmd.CommandText = "INSERT INTO EquippedItems (ProfileId, Slot, Layer, EquipmentName, Type, Description, Quality, Condition, StatsJson) VALUES ($pid, $slot, $layer, $ename, $type, $desc, $qual, $cond, $stats)";
                    itemCmd.Parameters.AddWithValue("$pid", profileId);
                    itemCmd.Parameters.AddWithValue("$slot", (int)slot);
                    itemCmd.Parameters.AddWithValue("$layer", item.Layer.HasValue ? (int)item.Layer.Value : (object)DBNull.Value);
                    itemCmd.Parameters.AddWithValue("$ename", item.Name);
                    itemCmd.Parameters.AddWithValue("$type", (int)item.Type);
                    itemCmd.Parameters.AddWithValue("$desc", item.Description ?? "");
                    itemCmd.Parameters.AddWithValue("$qual", (int)item.Quality);
                    itemCmd.Parameters.AddWithValue("$cond", (int)item.Condition);
                    itemCmd.Parameters.AddWithValue("$stats", System.Text.Json.JsonSerializer.Serialize(item.Stats));
                    await itemCmd.ExecuteNonQueryAsync();
                }
            }
        }
        tx.Commit();
    }

    public async Task<CompanyMember?> GetByIdAsync(string name)
    {
        if (_schemaInitTask != null) await _schemaInitTask;
        var conn = GetConnection();
        await EnsureConnectionOpenAsync(conn);
        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT Name, Role, Rank, Sex, Type FROM CompanyMembers WHERE Name = $name";
        cmd.Parameters.AddWithValue("$name", name);
        using var reader = await cmd.ExecuteReaderAsync();
        if (!await reader.ReadAsync()) return null;
        var member = new CompanyMember { Name = reader.GetString(0), Role = (Role)reader.GetInt32(1), Rank = (Rank)reader.GetInt32(2), Sex = (Sex)reader.GetInt32(3), Type = (MemberType)reader.GetInt32(4) };
        var profilesCmd = conn.CreateCommand();
        profilesCmd.CommandText = "SELECT Id, Rank, Name FROM EquipmentProfiles WHERE CompanyMemberName = $cm";
        profilesCmd.Parameters.AddWithValue("$cm", name);
        using var profilesReader = await profilesCmd.ExecuteReaderAsync();
        while (await profilesReader.ReadAsync())
        {
            var profileId = profilesReader.GetInt64(0);
            var rank = (Rank)profilesReader.GetInt32(1);
            var profileName = profilesReader.GetString(2);
            var profile = new EquipmentProfile { Name = profileName };
            var itemsCmd = conn.CreateCommand();
            itemsCmd.CommandText = "SELECT Slot, Layer, EquipmentName, Type, Description, Quality, Condition, StatsJson FROM EquippedItems WHERE ProfileId = $pid";
            itemsCmd.Parameters.AddWithValue("$pid", profileId);
            using var itemsReader = await itemsCmd.ExecuteReaderAsync();
            while (await itemsReader.ReadAsync())
            {
                var slot = (EquipmentSlot)itemsReader.GetInt32(0);
                var layer = itemsReader.IsDBNull(1) ? (ArmourLayer?)null : (ArmourLayer)itemsReader.GetInt32(1);
                var eqName = itemsReader.GetString(2);
                var eqType = (EquipmentType)itemsReader.GetInt32(3);
                var eqDesc = itemsReader.IsDBNull(4) ? "" : itemsReader.GetString(4);
                var eqQual = (EquipmentQuality)itemsReader.GetInt32(5);
                var eqCond = (EquipmentCondition)itemsReader.GetInt32(6);
                var statsJson = itemsReader.IsDBNull(7) ? "{}" : itemsReader.GetString(7);
                var stats = System.Text.Json.JsonSerializer.Deserialize<Dictionary<StatType, float>>(statsJson) ?? new();
                var eq = new EquipmentPiece { Name = eqName, Type = eqType, Slot = slot, Layer = layer, Description = eqDesc, Quality = eqQual, Condition = eqCond, Stats = stats };
                if (!profile.EquippedItems.ContainsKey(slot))
                    profile.EquippedItems[slot] = new List<EquipmentPiece>();
                profile.EquippedItems[slot].Add(eq);
            }
            member.EquipmentProfiles[rank] = profile;
        }
        return member;
    }

    public async Task UpdateAsync(CompanyMember member)
    {
        if (_schemaInitTask != null) await _schemaInitTask;
        var conn = GetConnection();
        await EnsureConnectionOpenAsync(conn);
        using var tx = conn.BeginTransaction();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "UPDATE CompanyMembers SET Role = $role, Rank = $rank, Sex = $sex, Type = $type WHERE Name = $name";
        cmd.Parameters.AddWithValue("$name", member.Name);
        cmd.Parameters.AddWithValue("$role", (int)member.Role);
        cmd.Parameters.AddWithValue("$rank", (int)member.Rank);
        cmd.Parameters.AddWithValue("$sex", (int)member.Sex);
        cmd.Parameters.AddWithValue("$type", (int)member.Type);
        await cmd.ExecuteNonQueryAsync();
        var delItemsCmd = conn.CreateCommand();
        delItemsCmd.CommandText = @"DELETE FROM EquippedItems WHERE ProfileId IN (SELECT Id FROM EquipmentProfiles WHERE CompanyMemberName = $cm); DELETE FROM EquipmentProfiles WHERE CompanyMemberName = $cm;";
        delItemsCmd.Parameters.AddWithValue("$cm", member.Name);
        await delItemsCmd.ExecuteNonQueryAsync();
        foreach (var (rank, profile) in member.EquipmentProfiles)
        {
            var profileCmd = conn.CreateCommand();
            profileCmd.CommandText = "INSERT INTO EquipmentProfiles (CompanyMemberName, Rank, Name) VALUES ($cm, $rank, $name); SELECT last_insert_rowid();";
            profileCmd.Parameters.AddWithValue("$cm", member.Name);
            profileCmd.Parameters.AddWithValue("$rank", (int)rank);
            profileCmd.Parameters.AddWithValue("$name", profile.Name);
            var profileId = (long)await profileCmd.ExecuteScalarAsync();
            foreach (var (slot, items) in profile.EquippedItems)
            {
                foreach (var item in items)
                {
                    var itemCmd = conn.CreateCommand();
                    itemCmd.CommandText = "INSERT INTO EquippedItems (ProfileId, Slot, Layer, EquipmentName, Type, Description, Quality, Condition, StatsJson) VALUES ($pid, $slot, $layer, $ename, $type, $desc, $qual, $cond, $stats)";
                    itemCmd.Parameters.AddWithValue("$pid", profileId);
                    itemCmd.Parameters.AddWithValue("$slot", (int)slot);
                    itemCmd.Parameters.AddWithValue("$layer", item.Layer.HasValue ? (int)item.Layer.Value : (object)DBNull.Value);
                    itemCmd.Parameters.AddWithValue("$ename", item.Name);
                    itemCmd.Parameters.AddWithValue("$type", (int)item.Type);
                    itemCmd.Parameters.AddWithValue("$desc", item.Description ?? "");
                    itemCmd.Parameters.AddWithValue("$qual", (int)item.Quality);
                    itemCmd.Parameters.AddWithValue("$cond", (int)item.Condition);
                    itemCmd.Parameters.AddWithValue("$stats", System.Text.Json.JsonSerializer.Serialize(item.Stats));
                    await itemCmd.ExecuteNonQueryAsync();
                }
            }
        }
        tx.Commit();
    }

    public async Task DeleteAsync(string name)
    {
        if (_schemaInitTask != null) await _schemaInitTask;
        var conn = GetConnection();
        await EnsureConnectionOpenAsync(conn);
        using var tx = conn.BeginTransaction();
        var delItemsCmd = conn.CreateCommand();
        delItemsCmd.CommandText = @"DELETE FROM EquippedItems WHERE ProfileId IN (SELECT Id FROM EquipmentProfiles WHERE CompanyMemberName = $cm); DELETE FROM EquipmentProfiles WHERE CompanyMemberName = $cm;";
        delItemsCmd.Parameters.AddWithValue("$cm", name);
        await delItemsCmd.ExecuteNonQueryAsync();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "DELETE FROM CompanyMembers WHERE Name = $name";
        cmd.Parameters.AddWithValue("$name", name);
        await cmd.ExecuteNonQueryAsync();
        tx.Commit();
    }
}

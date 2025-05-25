using ExanimaTools.Models;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExanimaTools.Persistence;

public class TeamMemberRepository
{
    private readonly string _connectionString;

    public TeamMemberRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<List<TeamMember>> GetAllAsync()
    {
        var result = new List<TeamMember>();
        using var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT Name, Role, Rank, Sex, Type FROM TeamMembers";
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            result.Add(new TeamMember
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

    public async Task AddAsync(TeamMember member)
    {
        using var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "INSERT INTO TeamMembers (Name, Role, Rank, Sex, Type) VALUES ($name, $role, $rank, $sex, $type)";
        cmd.Parameters.AddWithValue("$name", member.Name);
        cmd.Parameters.AddWithValue("$role", (int)member.Role);
        cmd.Parameters.AddWithValue("$rank", (int)member.Rank);
        cmd.Parameters.AddWithValue("$sex", (int)member.Sex);
        cmd.Parameters.AddWithValue("$type", (int)member.Type);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<TeamMember?> GetByIdAsync(string name)
    {
        using var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT Name, Role, Rank, Sex, Type FROM TeamMembers WHERE Name = $name";
        cmd.Parameters.AddWithValue("$name", name);
        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new TeamMember
            {
                Name = reader.GetString(0),
                Role = (Role)reader.GetInt32(1),
                Rank = (Rank)reader.GetInt32(2),
                Sex = (Sex)reader.GetInt32(3),
                Type = (MemberType)reader.GetInt32(4)
            };
        }
        return null;
    }

    public async Task UpdateAsync(TeamMember member)
    {
        using var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "UPDATE TeamMembers SET Role = $role, Rank = $rank, Sex = $sex, Type = $type WHERE Name = $name";
        cmd.Parameters.AddWithValue("$name", member.Name);
        cmd.Parameters.AddWithValue("$role", (int)member.Role);
        cmd.Parameters.AddWithValue("$rank", (int)member.Rank);
        cmd.Parameters.AddWithValue("$sex", (int)member.Sex);
        cmd.Parameters.AddWithValue("$type", (int)member.Type);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task DeleteAsync(string name)
    {
        using var conn = new SqliteConnection(_connectionString);
        await conn.OpenAsync();
        var cmd = conn.CreateCommand();
        cmd.CommandText = "DELETE FROM TeamMembers WHERE Name = $name";
        cmd.Parameters.AddWithValue("$name", name);
        await cmd.ExecuteNonQueryAsync();
    }
}

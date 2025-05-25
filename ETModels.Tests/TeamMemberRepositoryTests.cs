using ExanimaTools.Models;
using ExanimaTools.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ETModels.Tests;

[TestClass]
public class TeamMemberRepositoryTests
{
    private string _dbPath = "TestTeamMembers.db";
    private string _connectionString => $"Data Source={_dbPath}";

    [TestInitialize]
    public void Init()
    {
        if (File.Exists(_dbPath)) File.Delete(_dbPath);
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();
        var cmd = conn.CreateCommand();
        cmd.CommandText = @"CREATE TABLE IF NOT EXISTS TeamMembers (
            Name TEXT PRIMARY KEY,
            Role INTEGER,
            Rank INTEGER,
            Sex INTEGER,
            Type INTEGER
        )";
        cmd.ExecuteNonQuery();
    }

    [TestMethod]
    public async Task AddAndGetAllAsync_Works()
    {
        var repo = new TeamMemberRepository(_connectionString);
        var member = new TeamMember { Name = "Bob", Role = Role.Fighter, Rank = Rank.Novice, Sex = Sex.Male, Type = MemberType.Recruit };
        await repo.AddAsync(member);
        List<TeamMember> all = await repo.GetAllAsync();
        Assert.AreEqual(1, all.Count);
        Assert.AreEqual("Bob", all[0].Name);
        Assert.AreEqual(Role.Fighter, all[0].Role);
        Assert.AreEqual(Rank.Novice, all[0].Rank);
        Assert.AreEqual(Sex.Male, all[0].Sex);
        Assert.AreEqual(MemberType.Recruit, all[0].Type);
    }

    [TestMethod]
    public async Task UpdateAndDeleteAsync_Works()
    {
        var repo = new TeamMemberRepository(_connectionString);
        var member = new TeamMember { Name = "Alice", Role = Role.Fighter, Rank = Rank.Master, Sex = Sex.Female, Type = MemberType.Hireling };
        await repo.AddAsync(member);
        // Update
        member.Rank = Rank.Adept;
        await repo.UpdateAsync(member);
        var updated = (await repo.GetAllAsync())[0];
        Assert.AreEqual(Rank.Adept, updated.Rank);
        // Delete
        await repo.DeleteAsync(member.Name);
        var all = await repo.GetAllAsync();
        Assert.AreEqual(0, all.Count);
    }

    [TestMethod]
    public async Task GetByIdAsync_Works()
    {
        var repo = new TeamMemberRepository(_connectionString);
        var member = new TeamMember { Name = "Carl", Role = Role.Physician, Rank = Rank.Novice, Sex = Sex.Other, Type = MemberType.Custom };
        await repo.AddAsync(member);
        var found = await repo.GetByIdAsync("Carl");
        Assert.IsNotNull(found);
        Assert.AreEqual("Carl", found.Name);
        Assert.AreEqual(Role.Physician, found.Role);
    }

    [TestMethod]
    public async Task GetByIdAsync_ReturnsCorrectMember()
    {
        var repo = new TeamMemberRepository(_connectionString);
        var member = new TeamMember { Name = "Alice", Role = Role.Fighter, Rank = Rank.Adept, Sex = Sex.Female, Type = MemberType.Hireling };
        await repo.AddAsync(member);
        var result = await repo.GetByIdAsync("Alice");
        Assert.IsNotNull(result);
        Assert.AreEqual("Alice", result.Name);
        Assert.AreEqual(Role.Fighter, result.Role);
        Assert.AreEqual(Rank.Adept, result.Rank);
        Assert.AreEqual(Sex.Female, result.Sex);
        Assert.AreEqual(MemberType.Hireling, result.Type);
    }

    [TestMethod]
    public async Task UpdateAsync_UpdatesMember()
    {
        var repo = new TeamMemberRepository(_connectionString);
        var member = new TeamMember { Name = "Carl", Role = Role.Fighter, Rank = Rank.Novice, Sex = Sex.Male, Type = MemberType.Recruit };
        await repo.AddAsync(member);
        member.Role = Role.Physician;
        member.Rank = Rank.Expert;
        member.Sex = Sex.Female;
        member.Type = MemberType.Hireling;
        await repo.UpdateAsync(member);
        var updated = await repo.GetByIdAsync("Carl");
        Assert.IsNotNull(updated);
        Assert.AreEqual(Role.Physician, updated.Role);
        Assert.AreEqual(Rank.Expert, updated.Rank);
        Assert.AreEqual(Sex.Female, updated.Sex);
        Assert.AreEqual(MemberType.Hireling, updated.Type);
    }

    [TestMethod]
    public async Task DeleteAsync_RemovesMember()
    {
        var repo = new TeamMemberRepository(_connectionString);
        var member = new TeamMember { Name = "Dana", Role = Role.Fighter, Rank = Rank.Novice, Sex = Sex.Female, Type = MemberType.Recruit };
        await repo.AddAsync(member);
        await repo.DeleteAsync("Dana");
        var result = await repo.GetByIdAsync("Dana");
        Assert.IsNull(result);
        var all = await repo.GetAllAsync();
        Assert.AreEqual(0, all.Count);
    }

    [TestMethod]
    public async Task AddAsync_DuplicateName_ThrowsOrIgnores()
    {
        var repo = new TeamMemberRepository(_connectionString);
        var member = new TeamMember { Name = "Eve", Role = Role.Fighter, Rank = Rank.Novice, Sex = Sex.Female, Type = MemberType.Recruit };
        await repo.AddAsync(member);
        await Assert.ThrowsExceptionAsync<SqliteException>(async () =>
        {
            await repo.AddAsync(member);
        });
    }

    [TestMethod]
    public async Task GetByIdAsync_Nonexistent_ReturnsNull()
    {
        var repo = new TeamMemberRepository(_connectionString);
        var result = await repo.GetByIdAsync("NotAName");
        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task DeleteAsync_Nonexistent_DoesNotThrow()
    {
        var repo = new TeamMemberRepository(_connectionString);
        await repo.DeleteAsync("Ghost");
        // No exception expected
        Assert.IsTrue(true);
    }
}

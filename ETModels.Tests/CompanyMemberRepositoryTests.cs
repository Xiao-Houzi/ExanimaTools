using ExanimaTools.Models;
using ExanimaTools.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ETModels.Tests;

[TestClass]
public class CompanyMemberRepositoryTests
{
    private class DummyLogger : ILoggingService
    {
        public void Log(string message) { }
        public void LogOperation(string operation, string? details = null) { }
        public void LogError(string message, System.Exception? ex = null) { }
        public void LogInformation(string message) { }
    }

    private SqliteConnection? _connection;
    private string _connectionString => "Data Source=:memory:;Cache=Shared";

    [TestInitialize]
    public async Task Init()
    {
        _connection = new SqliteConnection(_connectionString);
        await _connection.OpenAsync();
        // Ensure Equipment table exists for tests on the same connection
        using (var cmd = _connection.CreateCommand())
        {
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
        }
        var eqRepo = new EquipmentRepository(_connection!, new DummyLogger());
        var repo = new CompanyMemberRepository(_connection); // new overload
        await repo.InitializeSchemaAsync();
    }

    [TestCleanup]
    public void Cleanup()
    {
        _connection?.Dispose();
    }

    [TestMethod]
    public async Task AddAndGetAllAsync_Works()
    {
        var repo = new CompanyMemberRepository(_connection!);
        var member = new CompanyMember { Name = "Bob", Role = Role.Fighter, Rank = Rank.Novice, Sex = Sex.Male, Type = MemberType.Recruit };
        await repo.AddAsync(member);
        List<CompanyMember> all = await repo.GetAllAsync();
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
        var repo = new CompanyMemberRepository(_connection!);
        var member = new CompanyMember { Name = "Alice", Role = Role.Fighter, Rank = Rank.Master, Sex = Sex.Female, Type = MemberType.Hireling };
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
        var repo = new CompanyMemberRepository(_connection!);
        var member = new CompanyMember { Name = "Carl", Role = Role.Physician, Rank = Rank.Novice, Sex = Sex.Other, Type = MemberType.Custom };
        await repo.AddAsync(member);
        var found = await repo.GetByIdAsync("Carl");
        Assert.IsNotNull(found);
        Assert.AreEqual("Carl", found.Name);
        Assert.AreEqual(Role.Physician, found.Role);
    }

    [TestMethod]
    public async Task GetByIdAsync_ReturnsCorrectMember()
    {
        var repo = new CompanyMemberRepository(_connection!);
        var member = new CompanyMember { Name = "Alice", Role = Role.Fighter, Rank = Rank.Adept, Sex = Sex.Female, Type = MemberType.Hireling };
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
    public async Task GetByIdAsync_ReturnsNullForNonExistentMember()
    {
        var repo = new CompanyMemberRepository(_connection!);
        var result = await repo.GetByIdAsync("NonExistent");
        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task AddAsync_ThrowsForDuplicateName()
    {
        var repo = new CompanyMemberRepository(_connection!);
        var member1 = new CompanyMember { Name = "Alice", Role = Role.Fighter, Rank = Rank.Adept, Sex = Sex.Female, Type = MemberType.Hireling };
        var member2 = new CompanyMember { Name = "Alice", Role = Role.Physician, Rank = Rank.Master, Sex = Sex.Male, Type = MemberType.Recruit };
        await repo.AddAsync(member1);
        await Assert.ThrowsExceptionAsync<Microsoft.Data.Sqlite.SqliteException>(async () => await repo.AddAsync(member2));
    }

    [TestMethod]
    public async Task UpdateAsync_ModifiesExistingMember()
    {
        var repo = new CompanyMemberRepository(_connection!);
        var member = new CompanyMember { Name = "Alice", Role = Role.Fighter, Rank = Rank.Adept, Sex = Sex.Female, Type = MemberType.Hireling };
        await repo.AddAsync(member);
        member.Role = Role.Physician;
        member.Rank = Rank.Master;
        await repo.UpdateAsync(member);
        var updated = await repo.GetByIdAsync("Alice");
        Assert.IsNotNull(updated);
        Assert.AreEqual(Role.Physician, updated.Role);
        Assert.AreEqual(Rank.Master, updated.Rank);
    }

    [TestMethod]
    public async Task DeleteAsync_RemovesMember()
    {
        var repo = new CompanyMemberRepository(_connection!);
        var member = new CompanyMember { Name = "Alice", Role = Role.Fighter, Rank = Rank.Adept, Sex = Sex.Female, Type = MemberType.Hireling };
        await repo.AddAsync(member);
        await repo.DeleteAsync("Alice");
        var deleted = await repo.GetByIdAsync("Alice");
        Assert.IsNull(deleted);
    }

    [TestMethod]
    public async Task DeleteAsync_HandlesNonExistentMember()
    {
        var repo = new CompanyMemberRepository(_connection!);
        // Should not throw
        await repo.DeleteAsync("NonExistent");
    }

    [TestMethod]
    public async Task AssignEquipmentToProfile_PreventsDuplicatesAndPersists()
    {
        // Insert required equipment
        using (var cmd = _connection!.CreateCommand())
        {
            cmd.CommandText = @"INSERT INTO Equipment (Name, Type, Description) VALUES
                ('Sword', 0, 'Sharp blade'),
                ('Sword2', 0, 'Backup blade')";
            cmd.ExecuteNonQuery();
        }
        var eqRepo = new EquipmentRepository(_connection!, new DummyLogger());
        var allEq = await eqRepo.GetAllAsync();
        var eq1 = allEq.Find(e => e.Name == "Sword")!;
        var eq2 = allEq.Find(e => e.Name == "Sword2")!;
        var repo = new CompanyMemberRepository(_connection!);
        var member = new CompanyMember { Name = "AssignGuy", Role = Role.Fighter, Rank = Rank.Novice, Sex = Sex.Male, Type = MemberType.Recruit };
        // Assign to Novice profile
        Assert.IsTrue(member.AssignEquipmentToProfile(Rank.Novice, EquipmentSlot.Hands, eq1));
        // Duplicate assignment should be prevented
        Assert.IsFalse(member.AssignEquipmentToProfile(Rank.Novice, EquipmentSlot.Hands, eq1));
        // Assign another weapon
        Assert.IsTrue(member.AssignEquipmentToProfile(Rank.Novice, EquipmentSlot.Hands, eq2));
        // Persist
        await repo.AddAsync(member);
        var loaded = await repo.GetByIdAsync("AssignGuy");
        Assert.IsNotNull(loaded);
        var hands = loaded.EquipmentProfiles[Rank.Novice].EquippedItems[EquipmentSlot.Hands];
        Assert.AreEqual(2, hands.Count);
        Assert.AreEqual("Sword", hands[0].Name);
        Assert.AreEqual("Sword2", hands[1].Name);
    }
}

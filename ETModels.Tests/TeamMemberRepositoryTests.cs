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
    private string? _dbPath = "TestTeamMember.db";
    private string _connectionString => $"Data Source={_dbPath}";

    [TestInitialize]
    public async Task Init()
    {
        if (File.Exists(_dbPath))
        {
            try { File.Delete(_dbPath); } catch { /* ignore if locked */ }
        }
        using (var conn = new SqliteConnection(_connectionString))
        {
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"CREATE TABLE IF NOT EXISTS Equipment (
                Name TEXT PRIMARY KEY,
                Type INTEGER,
                Description TEXT
            )";
            cmd.ExecuteNonQuery();
        }
        var repo = new TeamMemberRepository(_connectionString);
        await repo.InitializeSchemaAsync();
    }

    [TestCleanup]
    public void Cleanup()
    {
        if (File.Exists(_dbPath))
        {
            try { File.Delete(_dbPath); } catch { /* ignore */ }
        }
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

    [TestMethod]
    public async Task EquipmentProfiles_PersistsAndLoadsCorrectly()
    {
        // Insert required equipment pieces for FK constraints
        using (var conn = new SqliteConnection(_connectionString))
        {
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT INTO Equipment (Name, Type, Description) VALUES
                ('Gambeson', 1, 'Padded armour'),
                ('Mail Hauberk', 1, 'Chainmail'),
                ('Cuirass', 1, 'Plate'),
                ('Armet', 1, 'Helmet')";
            cmd.ExecuteNonQuery();
        }
        var repo = new TeamMemberRepository(_connectionString);
        var member = new TeamMember()
        {
            Name = "LayeredGuy",
            Role = Role.Fighter,
            Rank = Rank.Expert,
            Sex = Sex.Male,
            Type = MemberType.Hireling,
            EquipmentProfiles = new Dictionary<Rank, EquipmentProfile>
            {
                [Rank.Expert] = new EquipmentProfile()
                {
                    Name = "Expert Loadout",
                    EquippedItems = new Dictionary<EquipmentSlot, List<EquipmentPiece>>
                    {
                        [EquipmentSlot.Body] = new List<EquipmentPiece>
                        {
                            new EquipmentPiece { Name = "Gambeson", Type = EquipmentType.Armour, Slot = EquipmentSlot.Body, Layer = ArmourLayer.Padding, Stats = new Dictionary<StatType, float> { [StatType.CrushProtection] = 5 } },
                            new EquipmentPiece { Name = "Mail Hauberk", Type = EquipmentType.Armour, Slot = EquipmentSlot.Body, Layer = ArmourLayer.Chainmail, Stats = new Dictionary<StatType, float> { [StatType.SlashProtection] = 10 } },
                            new EquipmentPiece { Name = "Cuirass", Type = EquipmentType.Armour, Slot = EquipmentSlot.Body, Layer = ArmourLayer.Armour, Stats = new Dictionary<StatType, float> { [StatType.PierceProtection] = 15 } }
                        },
                        [EquipmentSlot.Head] = new List<EquipmentPiece>
                        {
                            new EquipmentPiece { Name = "Armet", Type = EquipmentType.Armour, Slot = EquipmentSlot.Head, Layer = ArmourLayer.Armour, Stats = new Dictionary<StatType, float> { [StatType.CrushProtection] = 8 } }
                        }
                    }
                }
            }
        };
        await repo.AddAsync(member);
        var loaded = await repo.GetByIdAsync("LayeredGuy");
        Assert.IsNotNull(loaded);
        Assert.IsNotNull(loaded.EquipmentProfiles);
        Assert.IsTrue(loaded.EquipmentProfiles.ContainsKey(Rank.Expert));
        var torso = loaded.EquipmentProfiles[Rank.Expert].EquippedItems[EquipmentSlot.Body];
        Assert.AreEqual(3, torso.Count);
        // Only assert on properties that are persisted in EquippedItems (JSON): Name, Type, Slot, Layer, Stats
        Assert.AreEqual("Gambeson", torso[0].Name);
        Assert.AreEqual(EquipmentType.Armour, torso[0].Type);
        Assert.AreEqual(EquipmentSlot.Body, torso[0].Slot);
        Assert.AreEqual(ArmourLayer.Padding, torso[0].Layer);
        Assert.IsTrue(torso[0].Stats.ContainsKey(StatType.CrushProtection));
        Assert.AreEqual(5, torso[0].Stats[StatType.CrushProtection]);

        Assert.AreEqual("Mail Hauberk", torso[1].Name);
        Assert.AreEqual(EquipmentType.Armour, torso[1].Type);
        Assert.AreEqual(EquipmentSlot.Body, torso[1].Slot);
        Assert.AreEqual(ArmourLayer.Chainmail, torso[1].Layer);
        Assert.IsTrue(torso[1].Stats.ContainsKey(StatType.SlashProtection));
        Assert.AreEqual(10, torso[1].Stats[StatType.SlashProtection]);

        Assert.AreEqual("Cuirass", torso[2].Name);
        Assert.AreEqual(EquipmentType.Armour, torso[2].Type);
        Assert.AreEqual(EquipmentSlot.Body, torso[2].Slot);
        Assert.AreEqual(ArmourLayer.Armour, torso[2].Layer);
        Assert.IsTrue(torso[2].Stats.ContainsKey(StatType.PierceProtection));
        Assert.AreEqual(15, torso[2].Stats[StatType.PierceProtection]);

        var head = loaded.EquipmentProfiles[Rank.Expert].EquippedItems[EquipmentSlot.Head];
        Assert.AreEqual(1, head.Count);
        Assert.AreEqual("Armet", head[0].Name);
        Assert.AreEqual(EquipmentType.Armour, head[0].Type);
        Assert.AreEqual(EquipmentSlot.Head, head[0].Slot);
        Assert.AreEqual(ArmourLayer.Armour, head[0].Layer);
        Assert.IsTrue(head[0].Stats.ContainsKey(StatType.CrushProtection));
        Assert.AreEqual(8, head[0].Stats[StatType.CrushProtection]);
    }
}

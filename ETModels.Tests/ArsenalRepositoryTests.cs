using ExanimaTools.Models;
using ExanimaTools.Persistence;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ExanimaTools;
using Microsoft.Data.Sqlite;

namespace ETModels.Tests
{
    [TestClass]
    [DoNotParallelize]
    public class ArsenalRepositoryTests
    {
        private class DummyLogger : ILoggingService
        {
            public void Log(string message) { }
            public void LogOperation(string operation, string? details = null) { }
            public void LogError(string message, System.Exception? ex = null) { }
            public void LogInformation(string message) { }
        }

        [TestMethod]
        public async Task AddAndGetArsenal_Works()
        {
            await using var connection = new SqliteConnection("Data Source=:memory:");
            await connection.OpenAsync();
            var logger = new DummyLogger();
            var equipmentRepo = DbManager.GetEquipmentRepository(logger, connection);
            var arsenalRepo = DbManager.GetArsenalRepository(connection);
            logger.LogInformation($"[AddAndGetArsenal_Works] Connection State after open: {connection.State}");
            await equipmentRepo.AddAsync(new EquipmentPiece { Name = $"SwordA_{Guid.NewGuid()}", Type = EquipmentType.Weapon });
            await equipmentRepo.AddAsync(new EquipmentPiece { Name = $"AxeB_{Guid.NewGuid()}", Type = EquipmentType.Weapon });
            var all = await equipmentRepo.GetAllAsync();
            logger.LogInformation($"[AddAndGetArsenal_Works] Equipment count after add: {all.Count}");
            await arsenalRepo.AddToArsenalAsync(all[0].Id);
            await arsenalRepo.AddToArsenalAsync(all[1].Id);
            logger.LogInformation($"[AddAndGetArsenal_Works] Connection State after AddToArsenalAsync: {connection.State}");
            var arsenal = await arsenalRepo.GetArsenalAsync(equipmentRepo);
            logger.LogInformation($"[AddAndGetArsenal_Works] Arsenal count: {arsenal.Equipment.Count}");
            Assert.AreEqual(2, arsenal.Equipment.Count);
            Assert.IsTrue(arsenal.Contains(all[0].Id));
            Assert.IsTrue(arsenal.Contains(all[1].Id));
        }

        [TestMethod]
        public async Task RemoveFromArsenal_RemovesItem()
        {
            await using var connection = new SqliteConnection("Data Source=:memory:");
            await connection.OpenAsync();
            var logger = new DummyLogger();
            var equipmentRepo = DbManager.GetEquipmentRepository(logger, connection);
            var arsenalRepo = DbManager.GetArsenalRepository(connection);
            logger.LogInformation($"[RemoveFromArsenal_RemovesItem] Connection State after open: {connection.State}");
            await equipmentRepo.AddAsync(new EquipmentPiece { Name = $"SwordA_{Guid.NewGuid()}", Type = EquipmentType.Weapon });
            await equipmentRepo.AddAsync(new EquipmentPiece { Name = $"AxeB_{Guid.NewGuid()}", Type = EquipmentType.Weapon });
            var all = await equipmentRepo.GetAllAsync();
            await arsenalRepo.AddToArsenalAsync(all[0].Id);
            await arsenalRepo.RemoveFromArsenalAsync(all[0].Id);
            logger.LogInformation($"[RemoveFromArsenal_RemovesItem] Connection State after RemoveFromArsenalAsync: {connection.State}");
            var arsenal = await arsenalRepo.GetArsenalAsync(equipmentRepo);
            logger.LogInformation($"[RemoveFromArsenal_RemovesItem] Arsenal count: {arsenal.Equipment.Count}");
            Assert.AreEqual(0, arsenal.Equipment.Count);
        }

        [TestMethod]
        public async Task SetArsenalAsync_Overwrites()
        {
            await using var connection = new SqliteConnection("Data Source=:memory:");
            await connection.OpenAsync();
            var logger = new DummyLogger();
            var equipmentRepo = DbManager.GetEquipmentRepository(logger, connection);
            var arsenalRepo = DbManager.GetArsenalRepository(connection);
            logger.LogInformation($"[SetArsenalAsync_Overwrites] Connection State after open: {connection.State}");
            await equipmentRepo.AddAsync(new EquipmentPiece { Name = $"SwordA_{Guid.NewGuid()}", Type = EquipmentType.Weapon });
            await equipmentRepo.AddAsync(new EquipmentPiece { Name = $"AxeB_{Guid.NewGuid()}", Type = EquipmentType.Weapon });
            var all = await equipmentRepo.GetAllAsync();
            await arsenalRepo.AddToArsenalAsync(all[0].Id);
            await arsenalRepo.SetArsenalAsync(new List<int> { all[1].Id });
            logger.LogInformation($"[SetArsenalAsync_Overwrites] Connection State after SetArsenalAsync: {connection.State}");
            var arsenal = await arsenalRepo.GetArsenalAsync(equipmentRepo);
            logger.LogInformation($"[SetArsenalAsync_Overwrites] Arsenal count: {arsenal.Equipment.Count}");
            Assert.AreEqual(1, arsenal.Equipment.Count);
            Assert.IsTrue(arsenal.Contains(all[1].Id));
        }

        [TestMethod]
        public async Task AddToArsenal_AllowsDuplicates()
        {
            await using var connection = new SqliteConnection("Data Source=:memory:");
            await connection.OpenAsync();
            var logger = new DummyLogger();
            var equipmentRepo = DbManager.GetEquipmentRepository(logger, connection);
            var arsenalRepo = DbManager.GetArsenalRepository(connection);
            logger.LogInformation($"[AddToArsenal_AllowsDuplicates] Connection State after open: {connection.State}");
            await equipmentRepo.AddAsync(new EquipmentPiece { Name = $"SwordA_{Guid.NewGuid()}", Type = EquipmentType.Weapon });
            await equipmentRepo.AddAsync(new EquipmentPiece { Name = $"AxeB_{Guid.NewGuid()}", Type = EquipmentType.Weapon });
            var all = await equipmentRepo.GetAllAsync();
            await arsenalRepo.AddToArsenalAsync(all[0].Id);
            await arsenalRepo.AddToArsenalAsync(all[0].Id);
            logger.LogInformation($"[AddToArsenal_AllowsDuplicates] Connection State after AddToArsenalAsync: {connection.State}");
            var arsenal = await arsenalRepo.GetArsenalAsync(equipmentRepo);
            logger.LogInformation($"[AddToArsenal_AllowsDuplicates] Arsenal count for Id {all[0].Id}: {arsenal.Equipment.Count(e => e.Id == all[0].Id)}");
            Assert.AreEqual(2, arsenal.Equipment.Count(e => e.Id == all[0].Id));
        }

        [TestMethod]
        public async Task RemoveFromArsenal_RemovesOneInstance()
        {
            await using var connection = new SqliteConnection("Data Source=:memory:");
            await connection.OpenAsync();
            var logger = new DummyLogger();
            var equipmentRepo = DbManager.GetEquipmentRepository(logger, connection);
            var arsenalRepo = DbManager.GetArsenalRepository(connection);
            logger.LogInformation($"[RemoveFromArsenal_RemovesOneInstance] Connection State after open: {connection.State}");
            await equipmentRepo.AddAsync(new EquipmentPiece { Name = $"SwordA_{Guid.NewGuid()}", Type = EquipmentType.Weapon });
            await equipmentRepo.AddAsync(new EquipmentPiece { Name = $"AxeB_{Guid.NewGuid()}", Type = EquipmentType.Weapon });
            var all = await equipmentRepo.GetAllAsync();
            await arsenalRepo.AddToArsenalAsync(all[0].Id);
            await arsenalRepo.AddToArsenalAsync(all[0].Id);
            await arsenalRepo.RemoveFromArsenalAsync(all[0].Id);
            logger.LogInformation($"[RemoveFromArsenal_RemovesOneInstance] Connection State after RemoveFromArsenalAsync: {connection.State}");
            var arsenal = await arsenalRepo.GetArsenalAsync(equipmentRepo);
            logger.LogInformation($"[RemoveFromArsenal_RemovesOneInstance] Arsenal count for Id {all[0].Id}: {arsenal.Equipment.Count(e => e.Id == all[0].Id)}");
            Assert.AreEqual(1, arsenal.Equipment.Count(e => e.Id == all[0].Id));
        }

        [TestMethod]
        public async Task AssignToMemberAsync_WithRank_AssignsEquipmentToSpecificRank()
        {
            await using var connection = new SqliteConnection("Data Source=:memory:");
            await connection.OpenAsync();
            var logger = new DummyLogger();
            var equipmentRepo = DbManager.GetEquipmentRepository(logger, connection);
            var arsenalRepo = DbManager.GetArsenalRepository(connection);
            var memberRepo = DbManager.GetCompanyMemberRepository(logger, connection);

            // Create a test member
            var member = new CompanyMember(logger)
            {
                Name = "Test Fighter",
                Role = Role.Fighter,
                Rank = Rank.Adept
            };
            await memberRepo.AddAsync(member);
            var members = await memberRepo.GetAllAsync();
            var memberId = members.First().Id;

            // Create and add equipment to arsenal
            var sword = new EquipmentPiece(logger) { Name = "Test Sword", Type = EquipmentType.Weapon };
            var armor = new EquipmentPiece(logger) { Name = "Test Armor", Type = EquipmentType.Armour };
            await equipmentRepo.AddAsync(sword);
            await equipmentRepo.AddAsync(armor);
            
            var equipment = await equipmentRepo.GetAllAsync();
            var swordId = equipment.First(e => e.Name == "Test Sword").Id;
            var armorId = equipment.First(e => e.Name == "Test Armor").Id;
            
            await arsenalRepo.AddToArsenalAsync(swordId);
            await arsenalRepo.AddToArsenalAsync(armorId);

            // Assign equipment to different ranks
            await arsenalRepo.AssignToMemberAsync(memberId, swordId, Rank.Novice);
            await arsenalRepo.AssignToMemberAsync(memberId, armorId, Rank.Expert);

            // Verify assignments
            var noviceEquipment = await arsenalRepo.GetMemberEquipmentForRankAsync(memberId, Rank.Novice, equipmentRepo);
            var expertEquipment = await arsenalRepo.GetMemberEquipmentForRankAsync(memberId, Rank.Expert, equipmentRepo);
            var adeptEquipment = await arsenalRepo.GetMemberEquipmentForRankAsync(memberId, Rank.Adept, equipmentRepo);

            Assert.AreEqual(1, noviceEquipment.Count);
            Assert.AreEqual("Test Sword", noviceEquipment[0].Name);
            
            Assert.AreEqual(1, expertEquipment.Count);
            Assert.AreEqual("Test Armor", expertEquipment[0].Name);
            
            Assert.AreEqual(0, adeptEquipment.Count);

            // Verify equipment was removed from arsenal
            var arsenal = await arsenalRepo.GetArsenalAsync(equipmentRepo);
            Assert.IsFalse(arsenal.Equipment.Any(e => e.Id == swordId));
            Assert.IsFalse(arsenal.Equipment.Any(e => e.Id == armorId));
        }

        [TestMethod]
        public async Task GetMemberEquipmentForRankAsync_ReturnsCorrectEquipment()
        {
            await using var connection = new SqliteConnection("Data Source=:memory:");
            await connection.OpenAsync();
            var logger = new DummyLogger();
            var equipmentRepo = DbManager.GetEquipmentRepository(logger, connection);
            var arsenalRepo = DbManager.GetArsenalRepository(connection);
            var memberRepo = DbManager.GetCompanyMemberRepository(logger, connection);

            // Create a test member
            var member = new CompanyMember(logger)
            {
                Name = "Test Fighter",
                Role = Role.Fighter,
                Rank = Rank.Adept
            };
            await memberRepo.AddAsync(member);
            var members = await memberRepo.GetAllAsync();
            var memberId = members.First().Id;

            // Create equipment
            var weapon1 = new EquipmentPiece(logger) { Name = "Novice Sword", Type = EquipmentType.Weapon };
            var weapon2 = new EquipmentPiece(logger) { Name = "Expert Sword", Type = EquipmentType.Weapon };
            var armor1 = new EquipmentPiece(logger) { Name = "Novice Armor", Type = EquipmentType.Armour };
            
            await equipmentRepo.AddAsync(weapon1);
            await equipmentRepo.AddAsync(weapon2);
            await equipmentRepo.AddAsync(armor1);
            
            var equipment = await equipmentRepo.GetAllAsync();
            var weapon1Id = equipment.First(e => e.Name == "Novice Sword").Id;
            var weapon2Id = equipment.First(e => e.Name == "Expert Sword").Id;
            var armor1Id = equipment.First(e => e.Name == "Novice Armor").Id;
            
            // Add to arsenal first
            await arsenalRepo.AddToArsenalAsync(weapon1Id);
            await arsenalRepo.AddToArsenalAsync(weapon2Id);
            await arsenalRepo.AddToArsenalAsync(armor1Id);

            // Assign to different ranks
            await arsenalRepo.AssignToMemberAsync(memberId, weapon1Id, Rank.Novice);
            await arsenalRepo.AssignToMemberAsync(memberId, weapon2Id, Rank.Expert);
            await arsenalRepo.AssignToMemberAsync(memberId, armor1Id, Rank.Novice);

            // Test retrieval
            var noviceEquipment = await arsenalRepo.GetMemberEquipmentForRankAsync(memberId, Rank.Novice, equipmentRepo);
            var expertEquipment = await arsenalRepo.GetMemberEquipmentForRankAsync(memberId, Rank.Expert, equipmentRepo);
            var masterEquipment = await arsenalRepo.GetMemberEquipmentForRankAsync(memberId, Rank.Master, equipmentRepo);

            Assert.AreEqual(2, noviceEquipment.Count);
            Assert.IsTrue(noviceEquipment.Any(e => e.Name == "Novice Sword"));
            Assert.IsTrue(noviceEquipment.Any(e => e.Name == "Novice Armor"));
            
            Assert.AreEqual(1, expertEquipment.Count);
            Assert.AreEqual("Expert Sword", expertEquipment[0].Name);
            
            Assert.AreEqual(0, masterEquipment.Count);
        }
    }
}
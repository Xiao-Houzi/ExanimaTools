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
        private static int _testCounter = 0;

        [TestMethod]
        public async Task AddAndGetArsenal_Works()
        {
            var testDb = $"TestArsenal_{Guid.NewGuid()}.db";
            if (File.Exists(testDb)) File.Delete(testDb);
            await using var connection = new SqliteConnection($"Data Source={testDb}");
            await connection.OpenAsync();
            var logger = new FileLoggingService("logs");
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
            var testDb = $"TestArsenal_{Guid.NewGuid()}.db";
            if (File.Exists(testDb)) File.Delete(testDb);
            await using var connection = new SqliteConnection($"Data Source={testDb}");
            await connection.OpenAsync();
            var logger = new FileLoggingService("logs");
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
            var testDb = $"TestArsenal_{Guid.NewGuid()}.db";
            if (File.Exists(testDb)) File.Delete(testDb);
            await using var connection = new SqliteConnection($"Data Source={testDb}");
            await connection.OpenAsync();
            var logger = new FileLoggingService("logs");
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
            var testDb = $"TestArsenal_{Guid.NewGuid()}.db";
            if (File.Exists(testDb)) File.Delete(testDb);
            await using var connection = new SqliteConnection($"Data Source={testDb}");
            await connection.OpenAsync();
            var logger = new FileLoggingService("logs");
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
            var testDb = $"TestArsenal_{Guid.NewGuid()}.db";
            if (File.Exists(testDb)) File.Delete(testDb);
            await using var connection = new SqliteConnection($"Data Source={testDb}");
            await connection.OpenAsync();
            var logger = new FileLoggingService("logs");
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
    }
}

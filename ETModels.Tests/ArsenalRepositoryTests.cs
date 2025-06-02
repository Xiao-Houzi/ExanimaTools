using ExanimaTools.Models;
using ExanimaTools.Persistence;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ETModels.Tests
{
    [TestClass]
    public class ArsenalRepositoryTests
    {
        private string TestDb;
        private string ConnStr => $"Data Source={TestDb}";
        private EquipmentRepository equipmentRepo = null!;
        private ArsenalRepository arsenalRepo = null!;
        private static int _testCounter = 0;

        [TestInitialize]
        public async Task Init()
        {
            TestDb = $"TestArsenal_{System.Threading.Interlocked.Increment(ref _testCounter)}.db";
            if (File.Exists(TestDb)) File.Delete(TestDb);
            equipmentRepo = new EquipmentRepository(ConnStr, new FileLoggingService("logs"));
            arsenalRepo = new ArsenalRepository(ConnStr);
            // Seed some equipment with unique names
            await equipmentRepo.AddAsync(new EquipmentPiece { Name = $"SwordA_{_testCounter}", Type = EquipmentType.Weapon });
            await equipmentRepo.AddAsync(new EquipmentPiece { Name = $"AxeB_{_testCounter}", Type = EquipmentType.Weapon });
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Explicitly dispose repositories to release DB file lock
            (equipmentRepo as System.IDisposable)?.Dispose();
            (arsenalRepo as System.IDisposable)?.Dispose();
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
            if (File.Exists(TestDb)) File.Delete(TestDb);
        }

        [TestMethod]
        public async Task AddAndGetArsenal_Works()
        {
            var all = await equipmentRepo.GetAllAsync();
            await arsenalRepo.AddToArsenalAsync(all[0].Id);
            await arsenalRepo.AddToArsenalAsync(all[1].Id);
            var arsenal = await arsenalRepo.GetArsenalAsync(equipmentRepo);
            Assert.AreEqual(2, arsenal.Equipment.Count);
            Assert.IsTrue(arsenal.Contains(all[0].Id));
            Assert.IsTrue(arsenal.Contains(all[1].Id));
        }

        [TestMethod]
        public async Task RemoveFromArsenal_RemovesItem()
        {
            var all = await equipmentRepo.GetAllAsync();
            await arsenalRepo.AddToArsenalAsync(all[0].Id);
            await arsenalRepo.RemoveFromArsenalAsync(all[0].Id);
            var arsenal = await arsenalRepo.GetArsenalAsync(equipmentRepo);
            Assert.AreEqual(0, arsenal.Equipment.Count);
        }

        [TestMethod]
        public async Task SetArsenalAsync_Overwrites()
        {
            var all = await equipmentRepo.GetAllAsync();
            await arsenalRepo.AddToArsenalAsync(all[0].Id);
            await arsenalRepo.SetArsenalAsync(new List<int> { all[1].Id });
            var arsenal = await arsenalRepo.GetArsenalAsync(equipmentRepo);
            Assert.AreEqual(1, arsenal.Equipment.Count);
            Assert.IsTrue(arsenal.Contains(all[1].Id));
        }

        [TestMethod]
        public async Task AddToArsenal_AllowsDuplicates()
        {
            var all = await equipmentRepo.GetAllAsync();
            await arsenalRepo.AddToArsenalAsync(all[0].Id);
            await arsenalRepo.AddToArsenalAsync(all[0].Id);
            var arsenal = await arsenalRepo.GetArsenalAsync(equipmentRepo);
            Assert.AreEqual(2, arsenal.Equipment.Count(e => e.Id == all[0].Id));
        }

        [TestMethod]
        public async Task RemoveFromArsenal_RemovesOneInstance()
        {
            var all = await equipmentRepo.GetAllAsync();
            await arsenalRepo.AddToArsenalAsync(all[0].Id);
            await arsenalRepo.AddToArsenalAsync(all[0].Id);
            await arsenalRepo.RemoveFromArsenalAsync(all[0].Id);
            var arsenal = await arsenalRepo.GetArsenalAsync(equipmentRepo);
            Assert.AreEqual(1, arsenal.Equipment.Count(e => e.Id == all[0].Id));
        }
    }
}

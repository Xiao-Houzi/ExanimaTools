using System.Threading.Tasks;
using ExanimaTools.Models;
using ExanimaTools.Persistence;
using ExanimaTools.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

namespace ETModels.Tests
{
    [TestClass]
    public class ArsenalManagerViewModelIntegrationTests
    {
        private string _dbPath = "TestArsenalVM.db";
        private string _connectionString => $"Data Source={_dbPath}";

        [TestInitialize]
        public void Init()
        {
            if (File.Exists(_dbPath))
            {
                try { File.Delete(_dbPath); } catch { /* ignore if locked */ }
            }
        }

        [TestMethod]
        public async Task AddAndRemoveEquipment_UpdatesUIAndDb()
        {
            // Arrange
            var equipmentRepo = new EquipmentRepository(_connectionString);
            var arsenalRepo = new ArsenalRepository(_connectionString);
            var eq = new EquipmentPiece { Name = "Test Dagger", Type = EquipmentType.Weapon };
            await equipmentRepo.AddAsync(eq);
            var all = await equipmentRepo.GetAllAsync();
            var vm = new ArsenalManagerViewModel(equipmentRepo, arsenalRepo);
            // Wait for async load
            await Task.Delay(100);
            // Add twice
            await vm.AddToArsenalAsync(all[0]);
            await vm.AddToArsenalAsync(all[0]);
            // Assert UI
            Assert.AreEqual(2, vm.ArsenalEquipment.Count(e => e.Id == all[0].Id));
            // Assert DB
            var arsenal = await arsenalRepo.GetArsenalAsync(equipmentRepo);
            Assert.AreEqual(2, arsenal.Equipment.Count(e => e.Id == all[0].Id));
            // Remove one
            vm.SelectedArsenalEquipment = vm.ArsenalEquipment.First(e => e.Id == all[0].Id);
            await vm.RemoveFromArsenalAsync();
            Assert.AreEqual(1, vm.ArsenalEquipment.Count(e => e.Id == all[0].Id));
            arsenal = await arsenalRepo.GetArsenalAsync(equipmentRepo);
            Assert.AreEqual(1, arsenal.Equipment.Count(e => e.Id == all[0].Id));
        }
    }
}

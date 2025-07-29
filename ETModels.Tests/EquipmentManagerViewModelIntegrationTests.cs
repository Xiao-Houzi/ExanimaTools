using System;
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
    public class EquipmentManagerViewModelIntegrationTests
    {
        private class DummyLogger : ILoggingService
        {
            public void Log(string message) { }
            public void LogOperation(string operation, string? details = null) { }
            public void LogError(string message, System.Exception? ex = null) { }
            public void LogInformation(string message) { }
        }

        private string _dbPath = "TestEquipmentVM.db";
        private string _connectionString => $"Data Source={_dbPath}";

        [TestInitialize]
        public void Init()
        {
            if (File.Exists(_dbPath))
            {
                try { File.Delete(_dbPath); } catch { /* ignore if locked */ }
            }
        }

        private EquipmentRepository GetTestEquipmentRepository() => new EquipmentRepository(_connectionString, new DummyLogger());

        [TestMethod]
        public async Task EditEquipment_UpdatesItemInDbAndCountUnchanged()
        {
            // Arrange
            var logger = new DummyLogger();
            var repo = GetTestEquipmentRepository();
            var eq = new EquipmentPiece(logger) { Name = "TestSword", Type = EquipmentType.Weapon, Description = "Sharp", Category = "Weapon", Subcategory = "Swords", Rank = Rank.Novice, Points = 10, Weight = 0.5f };
            await repo.AddAsync(eq);
            var vm = new EquipmentManagerViewModel(repo, logger);
            // Load equipment synchronously for test (avoid dispatcher)
            var loaded = await repo.GetAllAsync();
            vm.EquipmentList.Clear();
            foreach (var item in loaded)
                vm.EquipmentList.Add(item);
            var originalCount = vm.EquipmentList.Count;
            var toEdit = vm.EquipmentList.FirstOrDefault(e => e.Name == "TestSword");
            Assert.IsNotNull(toEdit);
            // Simulate edit
            vm.EditEquipmentFromTree(toEdit);
            vm.NewEquipment.Description = "Very Sharp";
            vm.NewEquipment.Points = 20;
            vm.NewEquipment.Name = "TestSwordRenamed";
            vm.IsEditMode = true;
            vm.IsAddFormVisible = true;
            vm.SaveNewEquipmentCommand.Execute(null);
            // Optionally, wait for async completion if needed (test may need to poll or expose a Task for testability)
            await Task.Delay(200); // Give time for async command to complete
            // Assert
            var all = await repo.GetAllAsync();
            Assert.AreEqual(originalCount, all.Count, "Item count should not change after edit");
            var updated = all.FirstOrDefault(e => e.Id == toEdit.Id);
            Assert.IsNotNull(updated);
            Assert.AreEqual("Very Sharp", updated.Description);
            Assert.AreEqual(20, updated.Points);
            Assert.AreEqual("TestSwordRenamed", updated.Name);
        }
    }
}

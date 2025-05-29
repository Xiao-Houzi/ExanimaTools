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

        [TestMethod]
        public async Task EquipmentFiltering_Works_ForAllFilterTypesAndCombinations()
        {
            // Arrange: Seed DB with diverse equipment
            var equipmentRepo = new EquipmentRepository(_connectionString);
            var arsenalRepo = new ArsenalRepository(_connectionString);
            var items = new[]
            {
                new EquipmentPiece { Name = "Sword1", Type = EquipmentType.Weapon, Category = "Weapon", Subcategory = "Swords", Condition = EquipmentCondition.Good, Rank = Rank.Novice, Stats = new() { { StatType.Slash, 7 }, { StatType.Weight, 2 } } },
                new EquipmentPiece { Name = "Sword2", Type = EquipmentType.Weapon, Category = "Weapon", Subcategory = "Swords", Condition = EquipmentCondition.Worn, Rank = Rank.Adept, Stats = new() { { StatType.Slash, 5 }, { StatType.Weight, 2.5f } } },
                new EquipmentPiece { Name = "Axe1", Type = EquipmentType.Weapon, Category = "Weapon", Subcategory = "Axes", Condition = EquipmentCondition.Pristine, Rank = Rank.Expert, Stats = new() { { StatType.Crush, 8 }, { StatType.Weight, 3 } } },
                new EquipmentPiece { Name = "Helmet1", Type = EquipmentType.Armour, Category = "Armour", Subcategory = "Head", Condition = EquipmentCondition.Fair, Rank = Rank.Inept, Stats = new() { { StatType.Coverage, 6 }, { StatType.Weight, 1 } } },
                new EquipmentPiece { Name = "Chest1", Type = EquipmentType.Armour, Category = "Armour", Subcategory = "Body", Condition = EquipmentCondition.Good, Rank = Rank.Master, Stats = new() { { StatType.ImpactResistance, 9 }, { StatType.Weight, 5 } } },
            };
            foreach (var eq in items) await equipmentRepo.AddAsync(eq);
            var vm = new ArsenalManagerViewModel(equipmentRepo, arsenalRepo);
            await Task.Delay(100); // Wait for async load
            var loadAsync = vm.GetType().GetMethod("LoadAsync", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (loadAsync != null) await (Task)loadAsync.Invoke(vm, null)!;

            // Helper to set filters and apply
            void SetFilters(params EquipmentFilterViewModel[] filters)
            {
                vm.Filters.Clear();
                foreach (var f in filters) vm.Filters.Add(f);
                var applyFiltersMethod = vm.GetType().GetMethod("ApplyFilters", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                applyFiltersMethod?.Invoke(vm, null);
                Task.Delay(50).Wait(); // Allow UI thread to process
            }

            // Category Equals
            var filtered = ArsenalManagerViewModel.GetFilteredEquipment(items, new[] { new EquipmentFilterViewModel { FilterField = EquipmentFilterField.Category, Operator = EquipmentFilterOperator.Equals, Value = "Weapon" } });
            Assert.IsTrue(filtered.All(e => e.Category == "Weapon"));
            Assert.AreEqual(3, filtered.Count);

            // Category NotEquals
            filtered = ArsenalManagerViewModel.GetFilteredEquipment(items, new[] { new EquipmentFilterViewModel { FilterField = EquipmentFilterField.Category, Operator = EquipmentFilterOperator.NotEquals, Value = "Weapon" } });
            Assert.IsTrue(filtered.All(e => e.Category != "Weapon"));
            Assert.AreEqual(2, filtered.Count);

            // Condition Equals
            filtered = ArsenalManagerViewModel.GetFilteredEquipment(items, new[] { new EquipmentFilterViewModel { FilterField = EquipmentFilterField.Condition, Operator = EquipmentFilterOperator.Equals, Value = EquipmentCondition.Good } });
            Assert.IsTrue(filtered.All(e => e.Condition == EquipmentCondition.Good));
            Assert.AreEqual(2, filtered.Count); // Sword1, Chest1

            // Condition NotEquals
            filtered = ArsenalManagerViewModel.GetFilteredEquipment(items, new[] { new EquipmentFilterViewModel { FilterField = EquipmentFilterField.Condition, Operator = EquipmentFilterOperator.NotEquals, Value = EquipmentCondition.Good } });
            Assert.IsTrue(filtered.All(e => e.Condition != EquipmentCondition.Good));
            Assert.AreEqual(3, filtered.Count);

            // Rank GreaterThan
            filtered = ArsenalManagerViewModel.GetFilteredEquipment(items, new[] { new EquipmentFilterViewModel { FilterField = EquipmentFilterField.Rank, Operator = EquipmentFilterOperator.GreaterThan, Value = Rank.Novice } });
            Assert.IsTrue(filtered.All(e => e.Rank > Rank.Novice));
            Assert.AreEqual(3, filtered.Count); // Sword2, Axe1, Chest1

            // Rank LessOrEqual
            filtered = ArsenalManagerViewModel.GetFilteredEquipment(items, new[] { new EquipmentFilterViewModel { FilterField = EquipmentFilterField.Rank, Operator = EquipmentFilterOperator.LessOrEqual, Value = Rank.Adept } });
            Assert.IsTrue(filtered.All(e => e.Rank <= Rank.Adept));
            Assert.AreEqual(3, filtered.Count); // Sword1, Sword2, Helmet1

            // Stat GreaterOrEqual (Slash >= 6)
            filtered = ArsenalManagerViewModel.GetFilteredEquipment(items, new[] { new EquipmentFilterViewModel { FilterField = EquipmentFilterField.Stat, Operator = EquipmentFilterOperator.GreaterOrEqual, StatName = nameof(StatType.Slash), Value = 6f } });
            Assert.IsTrue(filtered.All(e => e.Stats.TryGetValue(StatType.Slash, out var v) && v >= 6));
            Assert.AreEqual(1, filtered.Count); // Sword1

            // Stat LessThan (Weight < 2.5)
            filtered = ArsenalManagerViewModel.GetFilteredEquipment(items, new[] { new EquipmentFilterViewModel { FilterField = EquipmentFilterField.Stat, Operator = EquipmentFilterOperator.LessThan, StatName = nameof(StatType.Weight), Value = 2.5f } });
            Assert.IsTrue(filtered.All(e => e.Stats.TryGetValue(StatType.Weight, out var v) && v < 2.5f));
            Assert.AreEqual(2, filtered.Count); // Sword1, Helmet1

            // Multi-filter: Category=Weapon AND Rank>=Adept
            filtered = ArsenalManagerViewModel.GetFilteredEquipment(items, new[] {
                new EquipmentFilterViewModel { FilterField = EquipmentFilterField.Category, Operator = EquipmentFilterOperator.Equals, Value = "Weapon" },
                new EquipmentFilterViewModel { FilterField = EquipmentFilterField.Rank, Operator = EquipmentFilterOperator.GreaterOrEqual, Value = Rank.Adept }
            });
            Assert.IsTrue(filtered.All(e => e.Category == "Weapon" && e.Rank >= Rank.Adept));
            Assert.AreEqual(2, filtered.Count); // Sword2, Axe1
        }
    }
}

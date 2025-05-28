using ExanimaTools.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace ETModels.Tests
{
    [TestClass]
    public class ArsenalTests
    {
        [TestMethod]
        public void AddEquipment_AddsUniqueEquipment()
        {
            var arsenal = new Arsenal();
            var eq1 = new EquipmentPiece { Id = 1, Name = "Sword" };
            var eq2 = new EquipmentPiece { Id = 2, Name = "Axe" };
            Assert.IsTrue(arsenal.AddEquipment(eq1));
            Assert.IsTrue(arsenal.AddEquipment(eq2));
            Assert.AreEqual(2, arsenal.Equipment.Count);
        }

        [TestMethod]
        public void AddEquipment_AllowsDuplicates()
        {
            var arsenal = new Arsenal();
            var eq1 = new EquipmentPiece { Id = 1, Name = "Sword" };
            Assert.IsTrue(arsenal.AddEquipment(eq1));
            Assert.IsTrue(arsenal.AddEquipment(eq1));
            Assert.AreEqual(2, arsenal.Equipment.Count);
        }

        [TestMethod]
        public void RemoveEquipment_RemovesById()
        {
            var arsenal = new Arsenal();
            var eq1 = new EquipmentPiece { Id = 1, Name = "Sword" };
            arsenal.AddEquipment(eq1);
            Assert.IsTrue(arsenal.RemoveEquipment(1));
            Assert.AreEqual(0, arsenal.Equipment.Count);
        }

        [TestMethod]
        public void RemoveEquipment_RemovesOneInstanceOfDuplicate()
        {
            var arsenal = new Arsenal();
            var eq1 = new EquipmentPiece { Id = 1, Name = "Sword" };
            arsenal.AddEquipment(eq1);
            arsenal.AddEquipment(eq1);
            Assert.IsTrue(arsenal.RemoveEquipment(1));
            Assert.AreEqual(1, arsenal.Equipment.Count);
            Assert.IsTrue(arsenal.Contains(1));
        }

        [TestMethod]
        public void Contains_ReturnsTrueIfPresent()
        {
            var arsenal = new Arsenal();
            var eq1 = new EquipmentPiece { Id = 1, Name = "Sword" };
            arsenal.AddEquipment(eq1);
            Assert.IsTrue(arsenal.Contains(1));
            Assert.IsFalse(arsenal.Contains(2));
        }

        [TestMethod]
        public void SearchByName_FindsPartialMatches_CaseInsensitive()
        {
            var arsenal = new Arsenal();
            arsenal.AddEquipment(new EquipmentPiece { Id = 1, Name = "Iron Sword" });
            arsenal.AddEquipment(new EquipmentPiece { Id = 2, Name = "Steel Axe" });
            var results = arsenal.SearchByName("sword").ToList();
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("Iron Sword", results[0].Name);
        }
    }
}

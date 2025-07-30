using Microsoft.VisualStudio.TestTools.UnitTesting;
using ExanimaTools.Controls;
using ExanimaTools.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System;

namespace ETModels.Tests
{
    [TestClass]
    public class UniversalTreeControlFilterTests
    {
        private List<EquipmentPiece> _testEquipment = null!;

        [TestInitialize]
        public void Setup()
        {
            // Create test equipment data
            _testEquipment = new List<EquipmentPiece>
            {
                new EquipmentPiece { Id = 1, Name = "Iron Sword", Category = "Weapon", Type = EquipmentType.Weapon, Rank = Rank.Inept },
                new EquipmentPiece { Id = 2, Name = "Steel Armor", Category = "Armor", Type = EquipmentType.Armour, Rank = Rank.Novice },
                new EquipmentPiece { Id = 3, Name = "Magic Bow", Category = "Weapon", Type = EquipmentType.Weapon, Rank = Rank.Adept },
                new EquipmentPiece { Id = 4, Name = "Leather Boots", Category = "Armor", Type = EquipmentType.Armour, Rank = Rank.Inept },
                new EquipmentPiece { Id = 5, Name = "Fire Staff", Category = "Weapon", Type = EquipmentType.Weapon, Rank = Rank.Master }
            };
        }

        [TestMethod]
        public void UniversalTreeBuilder_WithFilter_BuildsCorrectTree()
        {
            // Arrange
            var builder = new UniversalTreeBuilder<EquipmentPiece>(
                e => e.Name,
                e => new List<EquipmentPiece>(), // Flat structure
                null // No actions
            );
            
            builder.WithViewModel(e => new UniversalTreeNode(e))
                   .WithFilter(e => e.Name.Contains("Sword", StringComparison.OrdinalIgnoreCase));
            
            // Act
            var result = builder.BuildTree(_testEquipment);
            
            // Assert
            Assert.AreEqual(1, result.Count(), "Should filter to only sword items");
            var item = result.First();
            Assert.IsTrue(((EquipmentPiece)((UniversalTreeNode)item).Data).Name.Contains("Sword"));
        }

        [TestMethod]
        public void UniversalTreeBuilder_WithCategoryFilter_FiltersCorrectly()
        {
            // Arrange
            var builder = new UniversalTreeBuilder<EquipmentPiece>(
                e => e.Name,
                e => new List<EquipmentPiece>(),
                null
            );
            
            builder.WithViewModel(e => new UniversalTreeNode(e))
                   .WithFilter(e => e.Category == "Weapon");
            
            // Act
            var result = builder.BuildTree(_testEquipment);
            
            // Assert
            Assert.AreEqual(3, result.Count(), "Should have 3 weapon items");
            foreach (var item in result)
            {
                var equipment = (EquipmentPiece)((UniversalTreeNode)item).Data;
                Assert.AreEqual("Weapon", equipment.Category);
            }
        }

        [TestMethod]
        public void UniversalTreeBuilder_WithRankFilter_FiltersCorrectly()
        {
            // Arrange
            var builder = new UniversalTreeBuilder<EquipmentPiece>(
                e => e.Name,
                e => new List<EquipmentPiece>(),
                null
            );
            
            builder.WithViewModel(e => new UniversalTreeNode(e))
                   .WithFilter(e => e.Rank == Rank.Master);
            
            // Act
            var result = builder.BuildTree(_testEquipment);
            
            // Assert
            Assert.AreEqual(1, result.Count(), "Should have 1 Master rank item");
            var equipment = (EquipmentPiece)((UniversalTreeNode)result.First()).Data;
            Assert.AreEqual(Rank.Master, equipment.Rank);
            Assert.AreEqual("Fire Staff", equipment.Name);
        }

        [TestMethod]
        public void UniversalTreeBuilder_WithNoFilter_ReturnsAllItems()
        {
            // Arrange
            var builder = new UniversalTreeBuilder<EquipmentPiece>(
                e => e.Name,
                e => new List<EquipmentPiece>(),
                null
            );
            
            builder.WithViewModel(e => new UniversalTreeNode(e));
            
            // Act
            var result = builder.BuildTree(_testEquipment);
            
            // Assert
            Assert.AreEqual(_testEquipment.Count, result.Count(), "Should return all items when no filter");
        }

        [TestMethod]
        public void UniversalTreeNode_CreatesCorrectly()
        {
            // Arrange
            var equipment = _testEquipment[0]; // Iron Sword
            
            // Act
            var node = new UniversalTreeNode(equipment);
            
            // Assert
            Assert.AreSame(equipment, node.Data);
            Assert.AreEqual("Iron Sword", ((IUniversalTreeNode)node).DisplayName);
            Assert.AreEqual(0, node.Children.Count);
        }

        [TestMethod]
        public void UniversalTreeNode_WithActions_StoresCorrectly()
        {
            // Arrange
            var equipment = _testEquipment[0];
            var action = new UniversalTreeNodeAction 
            { 
                Label = "Test Action", 
                CommandParameter = equipment 
            };
            
            // Act
            var node = new UniversalTreeNode(equipment);
            node.Actions.Add(action);
            
            // Assert
            Assert.AreEqual(1, node.Actions.Count);
            Assert.AreEqual("Test Action", node.Actions[0].Label);
            Assert.AreSame(equipment, node.Actions[0].CommandParameter);
        }

        [TestMethod]
        public void UniversalTreeBuilder_WithComplexFilter_CombinesConditions()
        {
            // Arrange
            var builder = new UniversalTreeBuilder<EquipmentPiece>(
                e => e.Name,
                e => new List<EquipmentPiece>(),
                null
            );
            
            // Filter: Weapons that are not Inept rank
            builder.WithViewModel(e => new UniversalTreeNode(e))
                   .WithFilter(e => e.Category == "Weapon" && e.Rank != Rank.Inept);
            
            // Act
            var result = builder.BuildTree(_testEquipment);
            
            // Assert
            Assert.AreEqual(2, result.Count(), "Should have 2 non-Inept weapons");
            foreach (var item in result)
            {
                var equipment = (EquipmentPiece)((UniversalTreeNode)item).Data;
                Assert.AreEqual("Weapon", equipment.Category);
                Assert.AreNotEqual(Rank.Inept, equipment.Rank);
            }
        }
    }
}

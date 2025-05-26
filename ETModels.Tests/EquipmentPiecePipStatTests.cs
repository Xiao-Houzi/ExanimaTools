using System;
using System.Collections.Generic;
using ExanimaTools.Models;
using ExanimaTools.ViewModels;
using Xunit;

namespace ETModels.Tests
{
    public class EquipmentPiecePipStatTests
    {
        [Fact]
        public void Setting_StatPipViewModel_Value_Updates_EquipmentPiece_Stat()
        {
            // Arrange
            var eq = new EquipmentPiece();
            eq.Stats = new Dictionary<StatType, float> { { StatType.ImpactResistance, 0.5f } };
            float lastValue = -1;
            var pipVm = new StatPipViewModel(StatType.ImpactResistance, eq.Stats[StatType.ImpactResistance], v => { eq.SetStat(StatType.ImpactResistance, v); lastValue = v; });

            // Act: simulate clicking the 3rd pip half (index 2, isHalf=true)
            pipVm.Value = 2.5f;

            // Assert
            Xunit.Assert.Equal(2.5f, eq.Stats[StatType.ImpactResistance]);
            Xunit.Assert.Equal(2.5f, pipVm.Value);
            Xunit.Assert.Equal(2.5f, lastValue);
        }

        [Fact]
        public void Setting_StatPipViewModel_Value_Updates_PipDisplayViewModel()
        {
            // Arrange
            var eq = new EquipmentPiece();
            eq.Stats = new Dictionary<StatType, float> { { StatType.ImpactResistance, 0.5f } };
            var pipVm = new StatPipViewModel(StatType.ImpactResistance, eq.Stats[StatType.ImpactResistance], v => eq.SetStat(StatType.ImpactResistance, v));

            // Act
            pipVm.Value = 4.5f;

            // Assert
            Xunit.Assert.Equal(4.5f, pipVm.PipDisplayViewModel.Value);
            Xunit.Assert.Equal(4.5f, eq.Stats[StatType.ImpactResistance]);
        }
    }
}

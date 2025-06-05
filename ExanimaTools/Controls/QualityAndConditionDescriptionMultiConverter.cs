using Avalonia.Data.Converters;
using System;
using System.Globalization;
using ExanimaTools.Models;
using System.Collections.Generic;

namespace ExanimaTools.Controls
{
    public class QualityAndConditionDescriptionMultiConverter : IMultiValueConverter
    {
        public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
        {
            if (values.Count >= 2 && values[0] is EquipmentQuality quality && values[1] is EquipmentCondition condition)
            {
                string qualityDesc = quality switch
                {
                    EquipmentQuality.Poor => "Poor (Worn, battered, unreliable)",
                    EquipmentQuality.Common => "Common (Standard, functional)",
                    EquipmentQuality.WellMade => "Well-Made (Sturdy, above average)",
                    EquipmentQuality.Masterwork => "Masterwork (Exceptional, rare)",
                    EquipmentQuality.Legendary => "Legendary (Unique, mythical)",
                    _ => quality.ToString()
                };
                string conditionDesc = condition switch
                {
                    EquipmentCondition.Ruined => "Ruined (Unusable)",
                    EquipmentCondition.Damaged => "Damaged (Reduced effectiveness)",
                    EquipmentCondition.Worn => "Worn (Noticeable wear)",
                    EquipmentCondition.Fair => "Fair (Usable, some wear)",
                    EquipmentCondition.Used => "Used (Functional, but not new)",
                    EquipmentCondition.Good => "Good (Well-kept)",
                    EquipmentCondition.Excellent => "Excellent (Almost new)",
                    EquipmentCondition.Pristine => "Pristine (Like new)",
                    _ => condition.ToString()
                };
                return $"{qualityDesc} | {conditionDesc}";
            }
            return string.Empty;
        }
    }
}

using Avalonia.Data.Converters;
using System;
using System.Globalization;
using ExanimaTools.Models;

namespace ExanimaTools.Controls
{
    public class QualityDescriptionConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is EquipmentQuality quality)
            {
                // Map to Exanima-style descriptions
                return quality switch
                {
                    EquipmentQuality.Poor => "Poor (Worn, battered, unreliable)",
                    EquipmentQuality.Common => "Common (Standard, functional)",
                    EquipmentQuality.WellMade => "Well-Made (Sturdy, above average)",
                    EquipmentQuality.Masterwork => "Masterwork (Exceptional, rare)",
                    EquipmentQuality.Legendary => "Legendary (Unique, mythical)",
                    _ => quality.ToString()
                };
            }
            return string.Empty;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    public class ConditionDescriptionConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is EquipmentCondition condition)
            {
                // Map to Exanima-style descriptions
                return condition switch
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
            }
            return string.Empty;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}

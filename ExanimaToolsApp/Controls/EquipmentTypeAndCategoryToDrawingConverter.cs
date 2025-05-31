using System;
using Avalonia.Data.Converters;
using Avalonia.Media;
using System.Globalization;

namespace ExanimaTools.Controls
{
    public class EquipmentDrawingInfo
    {
        public Geometry Geometry { get; set; } = Geometry.Parse("M 32,8 A 24,24 0 1 1 31.9,8 Z"); // Default: small circle
        public string GeometryPathString { get; set; } = "M 32,8 A 24,24 0 1 1 31.9,8 Z";
        public IBrush Fill { get; set; } = Brushes.Gray;
        public IPen Stroke { get; set; } = new Pen(Brushes.Black, 2);
    }

    public class EquipmentDrawingRequest
    {
        public string? Type { get; set; } // "Weapon", "Armour", "Shield"
        public string? Category { get; set; } // e.g. "Buckler", "Kite", etc.
    }

    public class EquipmentTypeAndCategoryToDrawingConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is EquipmentDrawingRequest req)
            {
                EquipmentDrawingInfo? result = null;
                switch (req.Type?.ToLowerInvariant())
                {
                    case "shield":
                        result = ConvertShield(req.Category);
                        break;
                    case "armour":
                        result = ConvertArmour(req.Category);
                        break;
                    case "weapon":
                        result = ConvertWeapon(req.Category);
                        break;
                    default:
                        result = CreateGeneric();
                        break;
                }
                // Fallback: if geometry is null or empty, use a visible default
                if (result == null || result.Geometry == null || result.Geometry.ToString() == "")
                {
                    result = new EquipmentDrawingInfo {
                        Geometry = Geometry.Parse("M 30,0 A 30,30 0 1 1 29.9,0 Z"),
                        Fill = Brushes.Red,
                        Stroke = new Pen(Brushes.Black, 2)
                    };
                }
                return result;
            }
            // Fallback: always return a visible geometry
            return new EquipmentDrawingInfo {
                Geometry = Geometry.Parse("M 30,0 A 30,30 0 1 1 29.9,0 Z"),
                Fill = Brushes.Red,
                Stroke = new Pen(Brushes.Black, 2)
            };
        }
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();

        private EquipmentDrawingInfo ConvertShield(string? category)
        {
            // Log the incoming category for debugging
            System.Diagnostics.Debug.WriteLine($"[ConvertShield] category: '{category}'");
            if (string.IsNullOrWhiteSpace(category)) return CreateGeneric();
            var cat = category.Trim().ToLowerInvariant();
            return cat switch
            {
                "buckler" => CreateBuckler(),
                "round" or "round shield" => CreateRound(),
                "kite" or "kite shield" => CreateKite(),
                "tower" or "tower shield" => CreateTower(),
                _ => CreateGeneric(),
            };
        }
        private EquipmentDrawingInfo ConvertArmour(string? category)
        {
            return new EquipmentDrawingInfo
            {
                GeometryPathString = "M 32,8 A 24,32 0 1 1 31.9,8 Z",
                Geometry = Geometry.Parse("M 32,8 A 24,32 0 1 1 31.9,8 Z"),
                Fill = Brushes.Silver,
                Stroke = new Pen(Brushes.Black, 3)
            };
        }
        private EquipmentDrawingInfo ConvertWeapon(string? category)
        {
            return new EquipmentDrawingInfo
            {
                GeometryPathString = "M 32,8 L 32,56 M 28,56 L 36,56",
                Geometry = Geometry.Parse("M 32,8 L 32,56 M 28,56 L 36,56"),
                Fill = Brushes.SaddleBrown,
                Stroke = new Pen(Brushes.Black, 3)
            };
        }
        private EquipmentDrawingInfo CreateBuckler() => new EquipmentDrawingInfo
        {
            GeometryPathString = "M 32,32 m -20,0 a 20,20 0 1,0 40,0 a 20,20 0 1,0 -40,0",
            Geometry = Geometry.Parse("M 32,32 m -20,0 a 20,20 0 1,0 40,0 a 20,20 0 1,0 -40,0"),
            Fill = Brushes.LightGray,
            Stroke = new Pen(Brushes.Black, 3)
        };
        private EquipmentDrawingInfo CreateRound() => new EquipmentDrawingInfo
        {
            GeometryPathString = "M 32,32 m -28,0 a 28,28 0 1,0 56,0 a 28,28 0 1,0 -56,0",
            Geometry = Geometry.Parse("M 32,32 m -28,0 a 28,28 0 1,0 56,0 a 28,28 0 1,0 -56,0"),
            Fill = Brushes.SaddleBrown,
            Stroke = new Pen(Brushes.Black, 3)
        };
        private EquipmentDrawingInfo CreateKite() => new EquipmentDrawingInfo
        {
            GeometryPathString = "M 32,8 L 56,40 Q 32,120 8,40 Z",
            Geometry = Geometry.Parse("M 32,8 L 56,40 Q 32,120 8,40 Z"),
            Fill = Brushes.LightSlateGray,
            Stroke = new Pen(Brushes.Black, 3)
        };
        private EquipmentDrawingInfo CreateTower() => new EquipmentDrawingInfo
        {
            GeometryPathString = "M 12,8 L 52,8 L 52,56 Q 32,72 12,56 Z",
            Geometry = Geometry.Parse("M 12,8 L 52,8 L 52,56 Q 32,72 12,56 Z"),
            Fill = Brushes.DarkSlateGray,
            Stroke = new Pen(Brushes.Black, 3)
        };
        private EquipmentDrawingInfo CreateGeneric() => new EquipmentDrawingInfo
        {
            GeometryPathString = "M 32,8 A 24,32 0 1 1 31.9,8 Z",
            Geometry = Geometry.Parse("M 32,8 A 24,32 0 1 1 31.9,8 Z"),
            Fill = Brushes.DimGray,
            Stroke = new Pen(Brushes.Black, 3)
        };
    }
}

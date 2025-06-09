using System;
using System.Globalization;
using Avalonia.Data.Converters;
using ExanimaTools.ViewModels;

namespace ExanimaTools.Controls
{
    // Returns the property value if EquipmentPiece is not null, otherwise returns an empty string.
    public class EquipmentPiecePropertySafeConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // value is the property (Rank, Quality, Condition) or null
            return value ?? string.Empty;
        }
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}

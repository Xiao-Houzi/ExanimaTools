using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace ExanimaTools.Controls
{
    public class CategoryToFontWeightConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string category && !string.IsNullOrEmpty(category))
            {
                // Example: make certain categories bold
                if (category.Equals("Weapon", StringComparison.OrdinalIgnoreCase) ||
                    category.Equals("Armour", StringComparison.OrdinalIgnoreCase))
                    return FontWeight.Bold;
            }
            return FontWeight.Normal;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}

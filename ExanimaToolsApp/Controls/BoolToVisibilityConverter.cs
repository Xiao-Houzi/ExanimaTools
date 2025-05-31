using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia;

namespace ExanimaTools.Controls
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool b)
                return b ? "Visible" : "Collapsed";
            return "Collapsed";
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}

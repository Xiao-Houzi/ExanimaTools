using System;
using Avalonia.Data.Converters;
using Avalonia;
using System.Globalization;
using Avalonia.Media;

namespace ExanimaTools.Controls
{
    // Converts a bool to FontWeight: true => Bold, false => Normal
    public class BooleanToFontWeightConverter : IValueConverter
    {
        public static readonly BooleanToFontWeightConverter Instance = new();
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool b)
                return b ? FontWeight.Bold : FontWeight.Normal;
            return FontWeight.Normal;
        }
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
    }
    // Inverts a bool (true => false, false => true)
    public class InverseBooleanConverter : IValueConverter
    {
        public static readonly InverseBooleanConverter Instance = new();
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool b)
                return !b;
            return false;
        }
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}

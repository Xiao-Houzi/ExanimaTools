using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace ExanimaTools.Controls
{
    public class BoolToOpacityConverter : IValueConverter
    {
        public static readonly BoolToOpacityConverter Instance = new BoolToOpacityConverter();
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
            => value is true ? 1.0 : 0.0;
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}

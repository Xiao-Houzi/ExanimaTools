using System;
using Avalonia.Data.Converters;
using Avalonia.Media;
using System.Globalization;

namespace ExanimaTools.Controls
{
    public class GeometryToStringConverter : IValueConverter
    {
        public static readonly GeometryToStringConverter Instance = new GeometryToStringConverter();
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is Geometry g)
                return g.ToString();
            return "(null)";
        }
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}

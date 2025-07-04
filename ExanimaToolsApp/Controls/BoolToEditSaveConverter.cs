using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace ExanimaTools.Controls
{
    public class BoolToEditSaveConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool b && b)
                return "Update";
            return "Save";
        }
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}

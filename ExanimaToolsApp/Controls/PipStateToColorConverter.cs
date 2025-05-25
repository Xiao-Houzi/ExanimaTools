using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace ExanimaTools.Controls
{
    public class PipStateToColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value switch
            {
                PipState.Full => new SolidColorBrush(Colors.Black),
                PipState.Half => new SolidColorBrush(Colors.Gray),
                PipState.Empty => new SolidColorBrush(Colors.Transparent),
                _ => new SolidColorBrush(Colors.Transparent)
            };
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}

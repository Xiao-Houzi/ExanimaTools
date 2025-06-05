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
            string? side = parameter as string;
            return value switch
            {
                PipState.Full => new SolidColorBrush(Colors.White),
                PipState.Half => side == "Left"
                    ? new SolidColorBrush(Colors.White)
                    : new SolidColorBrush(Colors.Gray),
                PipState.Empty => new SolidColorBrush(Color.FromArgb(64, 255, 255, 255)), // Low alpha white
                _ => new SolidColorBrush(Colors.Transparent)
            };
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}

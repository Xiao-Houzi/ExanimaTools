using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;
using Avalonia.VisualTree;

namespace ExanimaTools.Controls
{
    public partial class EquipmentManagerControl : UserControl
    {
        public EquipmentManagerControl()
        {
            InitializeComponent();
            if (Design.IsDesignMode) return;
            if (DataContext == null)
                DataContext = new ExanimaTools.ViewModels.EquipmentManagerViewModel();
        }
    }

    public class CategoryToFontWeightConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool isCategory && isCategory)
                return FontWeight.Bold;
            return FontWeight.Normal;
        }
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
    }
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // Avalonia uses bool for IsVisible, so just return the bool
            if (value is bool b)
                return b;
            return false;
        }
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}

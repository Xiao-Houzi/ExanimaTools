using Avalonia.Data.Converters;
using System;
using System.Globalization;
using System.Collections.Generic;
using ExanimaTools.Controls;
using ExanimaTools.ViewModels;

namespace ExanimaTools.Controls
{
    public class StatToPipDisplayViewModelConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        {
            // Value is a StatPipViewModel
            if (value is ExanimaTools.ViewModels.StatPipViewModel statVm)
            {
                // Explicitly cast to avoid ambiguity
                return ((ExanimaTools.ViewModels.StatPipViewModel)statVm).PipDisplayViewModel as ExanimaTools.Controls.PipDisplayViewModel;
            }
            // Fallback: Value is a float stat value (legacy)
            if (value is float f)
            {
                var pipVm = new PipDisplayViewModel();
                pipVm.SetAndGetValue(f);
                return pipVm;
            }
            // If value is a KeyValuePair<StatType, float> (from dictionary binding)
            if (value is System.Collections.Generic.KeyValuePair<ExanimaTools.Models.StatType, float> kv)
            {
                var pipVm = new PipDisplayViewModel();
                pipVm.SetAndGetValue(kv.Value);
                return pipVm;
            }
            return null;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}

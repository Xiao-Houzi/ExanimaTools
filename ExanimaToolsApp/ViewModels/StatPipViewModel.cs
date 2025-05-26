using ExanimaTools.Models;
using ExanimaTools.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace ExanimaTools.ViewModels
{
    public class StatPipViewModel : ObservableObject
    {
        private readonly ILoggingService? _logger;
        public StatType Stat { get; }
        private float _value;
        public float Value
        {
            get => _value;
            set
            {
                if (SetProperty(ref _value, value))
                {
                    _onValueChanged?.Invoke(value);
                    PipDisplayViewModel.SetAndGetValue(value);
                    _logger?.LogOperation("StatPipViewModel.ValueChanged", $"{Stat}={value}");
                }
            }
        }
        private readonly Action<float> _onValueChanged;
        public PipDisplayViewModel PipDisplayViewModel { get; }
        public string StatLabel => Stat.ToString();
        public StatPipViewModel(StatType stat, float value, Action<float> onValueChanged, ILoggingService? logger = null)
        {
            Stat = stat;
            _value = value;
            _onValueChanged = onValueChanged;
            _logger = logger;
            PipDisplayViewModel = new PipDisplayViewModel(v => {
                if (_value != v)
                {
                    Value = v; // This triggers logging and stat update
                }
            });
            // Only set the value directly, do not trigger callback/logging
            PipDisplayViewModel.Value = value;
        }
    }
}

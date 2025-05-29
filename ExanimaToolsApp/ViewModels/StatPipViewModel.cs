using ExanimaTools.Models;
using ExanimaTools.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
#if DEBUG
using CommunityToolkit.Mvvm.Input;
#endif

namespace ExanimaTools.ViewModels
{
    public partial class StatPipViewModel : ObservableObject
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
        public string ShortStatLabel
        {
            get
            {
                var name = Stat.ToString();
                // Map common protection stats to short names
                if (name == "SlashProtection") return "Slash";
                if (name == "CrushProtection") return "Crush";
                if (name == "PierceProtection") return "Pierce";
                // Remove 'Resistance', 'Protection' for display
                name = name.Replace("Resistance", "").Replace("Protection", "").Trim();
                // Optionally, further shorten common stat names
                if (name == "Impact") return "Impact";
                if (name == "Slash") return "Slash";
                if (name == "Crush") return "Crush";
                if (name == "Pierce") return "Pierce";
                if (name == "Thrust") return "Thrust";
                if (name == "Balance") return "Balance";
                if (name == "Coverage") return "Coverage";
                if (name == "Encumbrance") return "Encumbrance";
                if (name == "Points") return "Points";
                return name;
            }
        }
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
#if DEBUG
        [RelayCommand]
        private void TestRelayCommand()
        {
            // Minimal test command
            System.Diagnostics.Debug.WriteLine("RelayCommand works");
        }
#endif
    }
}

using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExanimaTools.ViewModels;
using System.Windows.Input;

namespace ExanimaTools.Controls
{
    public enum PipState { Empty, Half, Full }

    public partial class PipDisplayViewModel : ObservableObject
    {
        public ObservableCollection<PipState> Pips { get; } = new();

        private float _value;
        public float Value
        {
            get => _value;
            set
            {
                if (SetProperty(ref _value, Math.Clamp((float)Math.Round(value * 2) / 2, 0, 10)))
                {
                    UpdatePips();
                }
            }
        }

        public int MaxPips { get; } = 10;

        private Action<float>? _onValueChanged;
        public IRelayCommand IncrementCommand { get; }
        public IRelayCommand DecrementCommand { get; }

        public PipDisplayViewModel(Action<float>? onValueChanged = null)
        {
            _onValueChanged = onValueChanged;
            Value = 0;
            IncrementCommand = new RelayCommand(Increment);
            DecrementCommand = new RelayCommand(Decrement);
        }

        public void SetOnValueChanged(Action<float> callback)
        {
            _onValueChanged = callback;
        }

        public float SetAndGetValue(float value)
        {
            Value = value;
            return Value;
        }

        public void SetPips(int full, bool half, int max)
        {
            // For compatibility, but always use 10 pips
            Value = full + (half ? 0.5f : 0f);
        }

        private void UpdatePips()
        {
            Pips.Clear();
            int full = (int)Value;
            bool half = (Value - full) >= 0.5f;
            for (int i = 0; i < full; i++) Pips.Add(PipState.Full);
            if (half) Pips.Add(PipState.Half);
            for (int i = full + (half ? 1 : 0); i < MaxPips; i++) Pips.Add(PipState.Empty);
            _onValueChanged?.Invoke(Value);
        }

        private void Increment()
        {
            Value = Math.Min(Value + 0.5f, MaxPips);
        }

        private void Decrement()
        {
            Value = Math.Max(Value - 0.5f, 0);
        }

        // Call this from the view when a pip is clicked
        public void SetValueFromPip(int pipIndex, bool isHalf)
        {
            float newValue = isHalf ? pipIndex + 0.5f : pipIndex + 1f;
            // Log pip click if logger is available
            if (ExanimaTools.App.LoggingServiceInstance is ExanimaTools.Models.ILoggingService logger)
            {
                logger.LogOperation("Pip Click", $"pipIndex={pipIndex}, isHalf={isHalf}, newValue={newValue}");
            }
            Value = newValue;
            _onValueChanged?.Invoke(Value);
        }
    }
}

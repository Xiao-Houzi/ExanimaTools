using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using ExanimaTools.Models;

namespace ExanimaTools.ViewModels
{
    public enum EquipmentFilterField
    {
        Category,
        Condition,
        Rank,
        Stat
    }

    public enum EquipmentFilterOperator
    {
        Equals,
        NotEquals,
        GreaterThan,
        LessThan,
        GreaterOrEqual,
        LessOrEqual
    }

    public class EquipmentFilterViewModel : INotifyPropertyChanged
    {
        private readonly ILoggingService? _logger;
        public EquipmentFilterViewModel(ILoggingService? logger = null)
        {
            _logger = logger;
            _logger?.LogOperation("EquipmentFilterViewModel", "Created");
            FilterField = EquipmentFilterField.Category;
            UpdateAvailableOperators();
            UpdateAvailableValues();
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        private EquipmentFilterField filterField;
        public EquipmentFilterField FilterField
        {
            get => filterField;
            set
            {
                if (filterField != value)
                {
                    filterField = value;
                    Value = null; // Clear value before updating available values
                    OnPropertyChanged(nameof(FilterField));
                    _logger?.Log($"FilterField changed: {filterField}");
                    UpdateAvailableOperators();
                    UpdateAvailableValues();
                }
            }
        }
        private EquipmentFilterOperator op;
        public EquipmentFilterOperator Operator
        {
            get => op;
            set { if (op != value) { op = value; OnPropertyChanged(nameof(Operator)); } }
        }
        public string? StatName { get; set; } // Only for Stat field
        public object? Value { get; set; }
        public ICommand? RemoveCommand { get; set; }

        // Dynamic lists for UI
        private ObservableCollection<EquipmentFilterOperator> availableOperators = new();
        public ObservableCollection<EquipmentFilterOperator> AvailableOperators
        {
            get => availableOperators;
            set
            {
                if (availableOperators != value)
                {
                    availableOperators = value;
                    OnPropertyChanged(nameof(AvailableOperators));
                }
            }
        }
        private ObservableCollection<string> availableValues = new();
        public ObservableCollection<string> AvailableValues
        {
            get => availableValues;
            set
            {
                if (availableValues != value)
                {
                    availableValues = value;
                    OnPropertyChanged(nameof(AvailableValues));
                }
            }
        }
        public ObservableCollection<string> AvailableStatTypes { get; private set; } = new();
        public float StatValue
        {
            get => Value is float f ? f : 0f;
            set { Value = value; OnPropertyChanged(nameof(StatValue)); }
        }
        public string? SelectedStatType
        {
            get => StatName;
            set { StatName = value; OnPropertyChanged(nameof(SelectedStatType)); }
        }
        private void UpdateAvailableOperators()
        {
            var newOps = new List<EquipmentFilterOperator>();
            switch (FilterField)
            {
                case EquipmentFilterField.Category:
                case EquipmentFilterField.Condition:
                    newOps.Add(EquipmentFilterOperator.Equals);
                    newOps.Add(EquipmentFilterOperator.NotEquals);
                    break;
                case EquipmentFilterField.Rank:
                case EquipmentFilterField.Stat:
                    newOps.Add(EquipmentFilterOperator.Equals);
                    newOps.Add(EquipmentFilterOperator.NotEquals);
                    newOps.Add(EquipmentFilterOperator.GreaterThan);
                    newOps.Add(EquipmentFilterOperator.LessThan);
                    newOps.Add(EquipmentFilterOperator.GreaterOrEqual);
                    newOps.Add(EquipmentFilterOperator.LessOrEqual);
                    break;
            }
            AvailableOperators = new ObservableCollection<EquipmentFilterOperator>(newOps);
            // Always set Operator to a valid value or null
            if (AvailableOperators.Count > 0)
            {
                Operator = AvailableOperators[0];
            }
            else
            {
                Operator = default;
            }
            OnPropertyChanged(nameof(Operator));
        }
        public void UpdateAvailableValues()
        {
            _logger?.Log($"UpdateAvailableValues called for FilterField={FilterField}");
            var newValues = new List<string>();
            AvailableStatTypes.Clear();
            if (FilterField == EquipmentFilterField.Category && ArsenalManagerViewModel.StaticCategoryOptions != null)
            {
                foreach (var v in ArsenalManagerViewModel.StaticCategoryOptions)
                {
                    _logger?.Log($"Adding category value: {v}");
                    newValues.Add(v);
                }
            }
            else if (FilterField == EquipmentFilterField.Condition)
            {
                foreach (var v in Enum.GetValues(typeof(EquipmentCondition)).Cast<object>())
                {
                    _logger?.Log($"Adding condition value: {v}");
                    newValues.Add(v.ToString()!);
                }
            }
            else if (FilterField == EquipmentFilterField.Rank)
            {
                foreach (var v in Enum.GetValues(typeof(Rank)).Cast<object>())
                {
                    _logger?.Log($"Adding rank value: {v}");
                    newValues.Add(v.ToString()!);
                }
            }
            else if (FilterField == EquipmentFilterField.Stat && ArsenalManagerViewModel.StaticStatTypes != null)
            {
                foreach (var st in ArsenalManagerViewModel.StaticStatTypes.Select(st => st.ToString()))
                {
                    _logger?.Log($"Adding stat type: {st}");
                    AvailableStatTypes.Add(st);
                }
            }
            AvailableValues = new ObservableCollection<string>(newValues);
            _logger?.Log($"AvailableValues now has {AvailableValues.Count} items: [{string.Join(", ", AvailableValues)}]");
            OnPropertyChanged(nameof(AvailableValues));
            OnPropertyChanged(nameof(AvailableStatTypes));
            // Always set Value to a valid value or null
            if (AvailableValues.Count > 0)
            {
                Value = AvailableValues[0];
            }
            else
            {
                Value = null;
            }
            OnPropertyChanged(nameof(Value));
        }
        public void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

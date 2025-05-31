using System;
using System.Collections.Generic;
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
        public List<EquipmentFilterOperator> AvailableOperators { get; private set; } = new();
        public List<object> AvailableValues { get; private set; } = new();
        public List<string> AvailableStatTypes { get; private set; } = new();
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
        public EquipmentFilterViewModel()
        {
            FilterField = EquipmentFilterField.Category;
            UpdateAvailableOperators();
            UpdateAvailableValues();
        }
        private void UpdateAvailableOperators()
        {
            AvailableOperators.Clear();
            switch (FilterField)
            {
                case EquipmentFilterField.Category:
                case EquipmentFilterField.Condition:
                    AvailableOperators.Add(EquipmentFilterOperator.Equals);
                    AvailableOperators.Add(EquipmentFilterOperator.NotEquals);
                    break;
                case EquipmentFilterField.Rank:
                case EquipmentFilterField.Stat:
                    AvailableOperators.AddRange(new[] {
                        EquipmentFilterOperator.Equals, EquipmentFilterOperator.NotEquals,
                        EquipmentFilterOperator.GreaterThan, EquipmentFilterOperator.LessThan,
                        EquipmentFilterOperator.GreaterOrEqual, EquipmentFilterOperator.LessOrEqual });
                    break;
            }
            OnPropertyChanged(nameof(AvailableOperators));
        }
        private void UpdateAvailableValues()
        {
            AvailableValues.Clear();
            AvailableStatTypes.Clear();
            if (FilterField == EquipmentFilterField.Category && ArsenalManagerViewModel.StaticCategoryOptions != null)
            {
                AvailableValues.AddRange(ArsenalManagerViewModel.StaticCategoryOptions);
            }
            else if (FilterField == EquipmentFilterField.Condition)
            {
                AvailableValues.AddRange(Enum.GetValues(typeof(EquipmentCondition)).Cast<object>());
            }
            else if (FilterField == EquipmentFilterField.Rank)
            {
                AvailableValues.AddRange(Enum.GetValues(typeof(Rank)).Cast<object>());
            }
            else if (FilterField == EquipmentFilterField.Stat && ArsenalManagerViewModel.StaticStatTypes != null)
            {
                AvailableStatTypes.AddRange(ArsenalManagerViewModel.StaticStatTypes.Select(st => st.ToString()));
            }
            OnPropertyChanged(nameof(AvailableValues));
            OnPropertyChanged(nameof(AvailableStatTypes));
        }
        public void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

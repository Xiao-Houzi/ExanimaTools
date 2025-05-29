using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public event PropertyChangedEventHandler? PropertyChanged;
        public EquipmentFilterField FilterField { get; set; }
        public EquipmentFilterOperator Operator { get; set; }
        public string? StatName { get; set; } // Only for Stat field
        public object? Value { get; set; }
        public ICommand? RemoveCommand { get; set; }
        public void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

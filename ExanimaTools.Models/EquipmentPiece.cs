using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ExanimaTools.Models
{
    public enum EquipmentQuality
    {
        Poor,
        Common,
        WellMade,
        Masterwork,
        Legendary
    }

    public enum EquipmentCondition
    {
        Ruined,
        Damaged,
        Worn,
        Fair,
        Used,
        Good,
        Excellent,
        Pristine
    }

    public class EquipmentPiece : INotifyPropertyChanged
    {
        /// <summary>
        /// Unique database key for this equipment item.
        /// </summary>
        public int Id { get; set; } // Unique DB key

        private readonly ILoggingService? _logger;
        public EquipmentPiece(ILoggingService? logger = null)
        {
            _logger = logger;
            _logger?.LogOperation("Create EquipmentPiece", $"Name={_name}");
        }
        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            set { _name = value; _logger?.LogOperation("Set Name", value); }
        }
        private EquipmentType _type;
        public EquipmentType Type
        {
            get => _type;
            set { if (!EqualityComparer<EquipmentType>.Default.Equals(_type, value)) { _type = value; OnPropertyChanged(nameof(Type)); } }
        }
        public EquipmentSlot Slot { get; set; }
        public ArmourLayer? Layer { get; set; }
        private Dictionary<StatType, float> _stats = new();
        public Dictionary<StatType, float> Stats
        {
            get => _stats;
            set
            {
                // Clamp all stat values
                _stats = value.ToDictionary(kv => kv.Key, kv => Math.Clamp(kv.Value, 0, 10));
                _logger?.LogOperation("Set Stats", string.Join(",", _stats.Select(kv => $"{kv.Key}:{kv.Value}")));
            }
        }
        public string Description { get; set; } = string.Empty;
        public EquipmentQuality Quality { get; set; } = EquipmentQuality.Common;
        public EquipmentCondition Condition { get; set; } = EquipmentCondition.Good;
        private string _category = string.Empty;
        public string Category
        {
            get => _category;
            set { if (_category != value) { _category = value; OnPropertyChanged(nameof(Category)); } }
        }
        public string Subcategory { get; set; } = string.Empty; // e.g., "Swords", "Polearms", "Body", "Head"
        public Rank Rank { get; set; } = Rank.Novice; // Minimum required rank to equip
        public int Points { get; set; } = 0; // Loadout cost
        public float Weight
        {
            get => Stats.TryGetValue(StatType.Weight, out var w) ? w : 0f;
            set => SetStat(StatType.Weight, value);
        }
        public void SetStat(StatType stat, float value)
        {
            float clamped = Math.Clamp(value, 0, 10);
            _stats[stat] = clamped;
            _logger?.LogOperation("Set Stat", $"{stat}={clamped}");
        }

        private string? _imagePath;
        public string? ImagePath
        {
            get => _imagePath;
            set { if (_imagePath != value) { _imagePath = value; OnPropertyChanged(nameof(ImagePath)); } }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

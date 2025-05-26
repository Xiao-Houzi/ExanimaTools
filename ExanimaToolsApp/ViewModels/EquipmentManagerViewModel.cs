using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExanimaTools.Controls;
using ExanimaTools.Models;

namespace ExanimaTools.ViewModels
{
    public partial class EquipmentManagerViewModel : ObservableObject
    {
        private readonly ILoggingService? _logger;
        public EquipmentManagerViewModel(ILoggingService? logger = null)
        {
            _logger = logger;
            // Use logger for all new models
            NewEquipment = new EquipmentPiece(_logger);
        }
        // Observable collection of equipment pieces for binding
        [ObservableProperty]
        private ObservableCollection<EquipmentPiece> equipmentList = new();

        // Selected equipment for editing/removal
        [ObservableProperty]
        private EquipmentPiece? selectedEquipment;

        // Error and confirmation messaging
        [ObservableProperty]
        private string? errorMessage;

        [ObservableProperty]
        private string? confirmationMessage;

        // Fields for new equipment form
        [ObservableProperty]
        private EquipmentPiece newEquipment = new EquipmentPiece();
        [ObservableProperty]
        private string newStatInput = string.Empty;

        [ObservableProperty]
        private bool isAddFormVisible;

        public ObservableCollection<StatType> AvailableStatTypes { get; } = new();
        private StatType? selectedStatType;
        public StatType? SelectedStatType
        {
            get => selectedStatType;
            set => SetProperty(ref selectedStatType, value);
        }

        public ObservableCollection<EquipmentType> EquipmentTypes { get; } = new(Enum.GetValues(typeof(EquipmentType)).Cast<EquipmentType>());
        public ObservableCollection<EquipmentSlot> EquipmentSlots { get; } = new(Enum.GetValues(typeof(EquipmentSlot)).Cast<EquipmentSlot>());
        public ObservableCollection<ArmourLayer> ArmourLayers { get; } = new(Enum.GetValues(typeof(ArmourLayer)).Cast<ArmourLayer>());

        public ObservableCollection<StatPipViewModel> NewEquipmentStatPips { get; } = new();

        public Array EquipmentQualities => Enum.GetValues(typeof(ExanimaTools.Models.EquipmentQuality));
        public Array EquipmentConditions => Enum.GetValues(typeof(ExanimaTools.Models.EquipmentCondition));

        // Async stub for loading equipment (for future persistence)
        public async Task LoadEquipmentAsync()
        {
            // TODO: Load from storage
            await Task.CompletedTask;
        }

        // Async stub for saving equipment (for future persistence)
        public async Task SaveEquipmentAsync()
        {
            // TODO: Save to storage
            await Task.CompletedTask;
        }

        // Validation logic for equipment
        private bool ValidateEquipment(EquipmentPiece equipment, out string? error)
        {
            if (string.IsNullOrWhiteSpace(equipment.Name))
            {
                error = "Equipment name is required.";
                return false;
            }
            if (EquipmentList.Any(e => e != equipment && e.Name == equipment.Name))
            {
                error = $"An equipment named '{equipment.Name}' already exists.";
                return false;
            }
            error = null;
            return true;
        }

        // Async command to add new equipment
        [RelayCommand]
        private async Task AddEquipmentAsync(EquipmentPiece newEquipment)
        {
            _logger?.LogOperation("AddEquipmentAsync", newEquipment?.Name);
            if (newEquipment == null)
                return;
            if (!ValidateEquipment(newEquipment, out var error))
            {
                ErrorMessage = error;
                ConfirmationMessage = null;
                return;
            }
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                EquipmentList.Add(newEquipment);
                ConfirmationMessage = $"Added '{newEquipment.Name}'.";
                ErrorMessage = null;
            });
        }

        // Async command to remove selected equipment
        [RelayCommand(CanExecute = nameof(CanRemoveEquipment))]
        private async Task RemoveEquipmentAsync()
        {
            _logger?.LogOperation("RemoveEquipmentAsync", SelectedEquipment?.Name);
            if (SelectedEquipment != null)
            {
                var toRemove = SelectedEquipment;
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    EquipmentList.Remove(toRemove);
                    SelectedEquipment = null;
                    ConfirmationMessage = $"Removed '{toRemove.Name}'.";
                    ErrorMessage = null;
                });
            }
        }

        private bool CanRemoveEquipment() => SelectedEquipment != null;

        // Async command to edit equipment (replace with updated instance)
        [RelayCommand]
        private async Task EditEquipmentAsync((EquipmentPiece Old, EquipmentPiece Updated) edit)
        {
            _logger?.LogOperation("EditEquipmentAsync", $"{edit.Old?.Name} -> {edit.Updated?.Name}");
            var (oldEq, updatedEq) = edit;
            if (oldEq == null || updatedEq == null)
                return;
            if (!ValidateEquipment(updatedEq, out var error))
            {
                ErrorMessage = error;
                ConfirmationMessage = null;
                return;
            }
            int idx = EquipmentList.IndexOf(oldEq);
            if (idx >= 0)
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    EquipmentList[idx] = updatedEq;
                    SelectedEquipment = updatedEq;
                    ConfirmationMessage = $"Updated '{updatedEq.Name}'.";
                    ErrorMessage = null;
                });
            }
        }

        private void SyncStatPipViewModels()
        {
            NewEquipmentStatPips.Clear();
            foreach (var kvp in NewEquipment.Stats)
            {
                var pipVm = new StatPipViewModel(kvp.Key, kvp.Value, v => NewEquipment.SetStat(kvp.Key, v), _logger);
                NewEquipmentStatPips.Add(pipVm);
            }
            UpdateAvailableStatTypes();
        }
        private void UpdateAvailableStatTypes()
        {
            AvailableStatTypes.Clear();
            IEnumerable<StatType> allowed = NewEquipment.Type switch
            {
                EquipmentType.Weapon => WeaponOptionalStats,
                EquipmentType.Armour => ArmourOptionalStats,
                _ => Array.Empty<StatType>()
            };
            foreach (var stat in allowed.Except(NewEquipment.Stats.Keys))
                AvailableStatTypes.Add(stat);
            if (AvailableStatTypes.Count > 0)
                SelectedStatType = AvailableStatTypes[0];
            else
                SelectedStatType = null;
        }

        private static readonly StatType[] WeaponStats = new[]
        {
            StatType.Encumbrance,
            StatType.Weight
        };
        private static readonly StatType[] WeaponOptionalStats = new[]
        {
            StatType.Balance,
            StatType.Impact,
            StatType.Slash,
            StatType.Crush,
            StatType.Pierce,
            StatType.Thrust
        };
        private static readonly StatType[] ArmourStats = new[]
        {
            StatType.Coverage,
            StatType.ImpactResistance,
            StatType.Encumbrance,
            StatType.Weight
        };
        private static readonly StatType[] ArmourOptionalStats = new[]
        {
            StatType.SlashProtection,
            StatType.CrushProtection,
            StatType.PierceProtection
            // Rare/optional stats like HeatProtection, ColdProtection, BluntProtection, MagicResistance, Flexibility
            // are not defined in StatType and are not included here.
        };

        [RelayCommand]
        private void ShowAddWeaponForm()
        {
            _logger?.LogOperation("ShowAddWeaponForm");
            NewEquipment = new EquipmentPiece(_logger)
            {
                Type = EquipmentType.Weapon,
                Slot = EquipmentSlot.Hands, // Default for weapon
                Layer = null,
                Stats = WeaponStats.ToDictionary(st => st, st => 0.5f)
            };
            IsAddFormVisible = true;
            SyncStatPipViewModels();
        }

        [RelayCommand]
        private void ShowAddArmourForm()
        {
            _logger?.LogOperation("ShowAddArmourForm");
            NewEquipment = new EquipmentPiece(_logger)
            {
                Type = EquipmentType.Armour,
                Slot = EquipmentSlot.Body, // Default for armour
                Layer = ArmourLayer.Padding,
                Stats = ArmourStats.ToDictionary(st => st, st => 0.5f)
            };
            IsAddFormVisible = true;
            SyncStatPipViewModels();
        }

        [RelayCommand]
        private void SaveNewEquipment()
        {
            if (!ValidateEquipment(NewEquipment, out var error))
            {
                ErrorMessage = error;
                ConfirmationMessage = null;
                return;
            }
            EquipmentList.Add(new EquipmentPiece
            {
                Name = NewEquipment.Name,
                Type = NewEquipment.Type,
                Slot = NewEquipment.Slot,
                Layer = NewEquipment.Layer,
                Stats = new Dictionary<StatType, float>(NewEquipment.Stats),
                Description = NewEquipment.Description,
                Quality = NewEquipment.Quality,
                Condition = NewEquipment.Condition
            });
            ConfirmationMessage = $"Saved '{NewEquipment.Name}'.";
            ErrorMessage = null;
            NewEquipment = new EquipmentPiece();
            IsAddFormVisible = false;
        }

        [RelayCommand]
        private void AddStat()
        {
            if (SelectedStatType is StatType statType && !NewEquipment.Stats.ContainsKey(statType))
            {
                NewEquipment.Stats[statType] = 0.5f;
                SyncStatPipViewModels();
            }
        }

        // TODO: Add persistence hooks
    }
}

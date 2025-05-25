using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExanimaTools.Models;

namespace ExanimaTools.ViewModels
{
    public partial class EquipmentManagerViewModel : ObservableObject
    {
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

        public ObservableCollection<EquipmentType> EquipmentTypes { get; } = new(Enum.GetValues(typeof(EquipmentType)).Cast<EquipmentType>());
        public ObservableCollection<EquipmentSlot> EquipmentSlots { get; } = new(Enum.GetValues(typeof(EquipmentSlot)).Cast<EquipmentSlot>());
        public ObservableCollection<ArmourLayer> ArmourLayers { get; } = new(Enum.GetValues(typeof(ArmourLayer)).Cast<ArmourLayer>());

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

        [RelayCommand]
        private void AddStat()
        {
            // Parse stat input as "StatType:Value"
            if (string.IsNullOrWhiteSpace(NewStatInput)) return;
            var parts = NewStatInput.Split(':');
            if (parts.Length == 2 && Enum.TryParse<StatType>(parts[0], true, out var statType) && float.TryParse(parts[1], out var value))
            {
                NewEquipment.Stats[statType] = value;
                NewStatInput = string.Empty;
            }
            else
            {
                ErrorMessage = "Invalid stat format. Use e.g. Impact:5";
            }
        }

        [RelayCommand]
        private void AddNewEquipment()
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
            ConfirmationMessage = $"Added '{NewEquipment.Name}'.";
            ErrorMessage = null;
            NewEquipment = new EquipmentPiece();
            NewStatInput = string.Empty;
        }

        [RelayCommand]
        private void ShowAddWeaponForm()
        {
            NewEquipment = new EquipmentPiece
            {
                Type = EquipmentType.Weapon,
                Slot = EquipmentSlot.Hands, // Default for weapon
                Layer = null,
                Stats = new Dictionary<StatType, float>
                {
                    { StatType.Balance, 0 },
                    { StatType.Thrust, 0 },
                    { StatType.Weight, 0 }
                }
            };
            IsAddFormVisible = true;
        }

        [RelayCommand]
        private void ShowAddArmourForm()
        {
            NewEquipment = new EquipmentPiece
            {
                Type = EquipmentType.Armour,
                Slot = EquipmentSlot.Body, // Default for armour
                Layer = ArmourLayer.Padding,
                Stats = new Dictionary<StatType, float>
                {
                    { StatType.Coverage, 0 },
                    { StatType.ImpactResistance, 0 },
                    { StatType.SlashProtection, 0 },
                    { StatType.CrushProtection, 0 },
                    { StatType.PierceProtection, 0 },
                    { StatType.Encumbrance, 0 },
                    { StatType.Weight, 0 }
                }
            };
            IsAddFormVisible = true;
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

        // TODO: Add persistence hooks
    }
}

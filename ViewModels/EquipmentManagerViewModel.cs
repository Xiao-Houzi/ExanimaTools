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

        // TODO: Add persistence hooks
    }
}

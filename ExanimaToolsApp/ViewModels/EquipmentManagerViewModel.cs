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
using ExanimaTools.Persistence;
using System.IO;

namespace ExanimaTools.ViewModels
{
    public partial class EquipmentManagerViewModel : ObservableObject
    {
        private readonly ILoggingService? _logger;
        private readonly EquipmentRepository _equipmentRepository;
        private const string DefaultDbFile = "exanima_tools.db";

        public EquipmentManagerViewModel(ILoggingService? logger = null)
        {
            _logger = logger;
            // Use logger for all new models
            NewEquipment = new EquipmentPiece(_logger);
            NewEquipment.Rank = Rank.Inept; // Set default rank to Inept immediately after creation
            // Set up repository with default DB path in app directory
            var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DefaultDbFile);
            _equipmentRepository = new EquipmentRepository($"Data Source={dbPath}");
            // Load equipment from DB on startup
            _ = LoadEquipmentAsync();
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

        // Category and subcategory options for dropdowns
        // New: Separate maps for weapon and armour categories
        public static readonly Dictionary<string, List<string>> WeaponCategorySubcategoryMap = new()
        {
            ["Sword"] = new List<string> { "Longsword", "Shortsword", "Greatsword", "Arming Sword" },
            ["Axe"] = new List<string> { "Battleaxe", "Handaxe", "Greataxe" },
            ["Polearm"] = new List<string> { "Spear", "Halberd", "Glaive" },
            ["Bludgeon"] = new List<string> { "Mace", "Hammer", "Club" },
            ["Dagger"] = new List<string> { "Stiletto", "Dirk", "Main Gauche" },
            ["Shield"] = new List<string> { "Buckler", "Round", "Kite", "Tower" },
            ["Unconventional"] = new List<string> { "Improvised", "Exotic" }
        };
        public static readonly Dictionary<string, List<string>> ArmourCategorySubcategoryMap = new()
        {
            ["Body"] = new List<string> { "Cuirass", "Brigandine", "Gambeson" },
            ["Head"] = new List<string> { "Helmet", "Coif", "Cap" },
            ["Shoulders"] = new List<string> { "Pauldron", "Spaulder" },
            ["Elbows"] = new List<string> { "Couter" },
            ["Wrists"] = new List<string> { "Vambrace", "Bracer" },
            ["Hands"] = new List<string> { "Gauntlet", "Glove" },
            ["Legs"] = new List<string> { "Cuisses", "Greaves" },
            ["Feet"] = new List<string> { "Sabatons", "Boots" }
        };

        [ObservableProperty]
        private string selectedCategory = string.Empty;
        [ObservableProperty]
        private string selectedSubcategory = string.Empty;
        public ObservableCollection<string> CategoryOptions { get; } = new();
        public ObservableCollection<string> SubcategoryOptions { get; } = new();

        private enum AddFormMode { None, Weapon, Armour }
        private AddFormMode addFormMode = AddFormMode.None;

        private void SetCategoryOptionsForMode()
        {
            CategoryOptions.Clear();
            if (addFormMode == AddFormMode.Weapon)
            {
                foreach (var cat in WeaponCategorySubcategoryMap.Keys)
                    CategoryOptions.Add(cat);
            }
            else if (addFormMode == AddFormMode.Armour)
            {
                foreach (var cat in ArmourCategorySubcategoryMap.Keys)
                    CategoryOptions.Add(cat);
            }
        }

        partial void OnSelectedCategoryChanged(string value)
        {
            SubcategoryOptions.Clear();
            if (addFormMode == AddFormMode.Weapon && !string.IsNullOrEmpty(value) && WeaponCategorySubcategoryMap.TryGetValue(value, out var subs))
            {
                foreach (var sub in subs)
                    SubcategoryOptions.Add(sub);
                SelectedSubcategory = SubcategoryOptions.FirstOrDefault() ?? string.Empty;
            }
            else if (addFormMode == AddFormMode.Armour && !string.IsNullOrEmpty(value) && ArmourCategorySubcategoryMap.TryGetValue(value, out var subs2))
            {
                foreach (var sub in subs2)
                    SubcategoryOptions.Add(sub);
                SelectedSubcategory = SubcategoryOptions.FirstOrDefault() ?? string.Empty;
            }
            else
            {
                SelectedSubcategory = string.Empty;
            }
            NewEquipment.Category = value;
            NewEquipment.Subcategory = SelectedSubcategory;
        }
        partial void OnSelectedSubcategoryChanged(string value)
        {
            NewEquipment.Subcategory = value;
        }

        // Async stub for loading equipment (for future persistence)
        public async Task LoadEquipmentAsync()
        {
            try
            {
                var loaded = await _equipmentRepository.GetAllAsync();
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    EquipmentList.Clear();
                    foreach (var eq in loaded)
                        EquipmentList.Add(eq);
                    BuildEquipmentTree(); // Ensure tree is built after loading
                    ConfirmationMessage = $"Loaded {loaded.Count} equipment from database.";
                    ErrorMessage = null;
                });
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load equipment: {ex.Message}\n{ex.StackTrace}";
                // Use LogError for error logging
                _logger?.LogError($"Failed to load equipment: {ex.Message}", ex);
            }
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
                if (kvp.Key == StatType.Weight) continue; // Remove Weight from pip stats
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
            StatType.Encumbrance
        };
        private static readonly StatType[] WeaponOptionalStats = new[]
        {
            StatType.Balance,
            StatType.Impact,
            StatType.Slash,
            StatType.Crush,
            StatType.Pierce,
            StatType.Thrust,
            StatType.Points // Allow Points as an optional pip stat
        };
        private static readonly StatType[] ArmourStats = new[]
        {
            StatType.Coverage,
            StatType.ImpactResistance,
            StatType.Encumbrance
        };
        private static readonly StatType[] ArmourOptionalStats = new[]
        {
            StatType.SlashProtection,
            StatType.CrushProtection,
            StatType.PierceProtection,
            StatType.Points // Allow Points as an optional pip stat
        };

        [RelayCommand]
        private void ShowAddWeaponForm()
        {
            _logger?.LogOperation("ShowAddWeaponForm");
            addFormMode = AddFormMode.Weapon;
            SetCategoryOptionsForMode();
            SelectedCategory = CategoryOptions.FirstOrDefault() ?? string.Empty;
            SelectedSubcategory = SubcategoryOptions.FirstOrDefault() ?? string.Empty;
            NewEquipment = new EquipmentPiece(_logger!)
            {
                Type = EquipmentType.Weapon,
                Slot = EquipmentSlot.Hands, // Default for weapon
                Layer = null,
                Stats = WeaponStats.ToDictionary(st => st, st => 0.5f),
                Category = SelectedCategory,
                Subcategory = SelectedSubcategory,
                Rank = Rank.Inept // Explicitly set default rank
            };
            IsAddFormVisible = true;
            SyncStatPipViewModels();
        }

        [RelayCommand]
        private void ShowAddArmourForm()
        {
            _logger?.LogOperation("ShowAddArmourForm");
            addFormMode = AddFormMode.Armour;
            SetCategoryOptionsForMode();
            SelectedCategory = CategoryOptions.FirstOrDefault() ?? string.Empty;
            SelectedSubcategory = SubcategoryOptions.FirstOrDefault() ?? string.Empty;
            NewEquipment = new EquipmentPiece(_logger!)
            {
                Type = EquipmentType.Armour,
                Slot = EquipmentSlot.Body, // Default for armour
                Layer = ArmourLayer.Padding,
                Stats = ArmourStats.ToDictionary(st => st, st => 0.5f),
                Category = SelectedCategory,
                Subcategory = SelectedSubcategory,
                Rank = Rank.Inept // Explicitly set default rank
            };
            IsAddFormVisible = true;
            SyncStatPipViewModels();
        }

        [RelayCommand]
        private async Task SaveNewEquipment()
        {
            if (!ValidateEquipment(NewEquipment, out var error))
            {
                ErrorMessage = error;
                ConfirmationMessage = null;
                return;
            }
            var eq = new EquipmentPiece
            {
                Name = NewEquipment.Name,
                Type = NewEquipment.Type,
                Slot = NewEquipment.Slot,
                Layer = NewEquipment.Layer,
                Stats = new Dictionary<StatType, float>(NewEquipment.Stats),
                Description = NewEquipment.Description,
                Quality = NewEquipment.Quality,
                Condition = NewEquipment.Condition,
                Category = NewEquipment.Category,
                Subcategory = NewEquipment.Subcategory
            };
            try
            {
                await _equipmentRepository.AddAsync(eq);
                EquipmentList.Add(eq);
                BuildEquipmentTree(); // Ensure tree updates after add
                ConfirmationMessage = $"Saved '{eq.Name}'.";
                ErrorMessage = null;
                NewEquipment = new EquipmentPiece();
                IsAddFormVisible = false;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to save equipment: {ex.Message}";
            }
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

        public class EquipmentTreeNodeViewModel : ObservableObject
        {
            public string Name { get; set; } = string.Empty;
            public ObservableCollection<EquipmentTreeNodeViewModel> Children { get; set; } = new();
            public EquipmentPiece? EquipmentPiece { get; set; }
            public bool IsCategory => EquipmentPiece == null;
            public bool IsLeaf => EquipmentPiece != null;
            public EquipmentTreeNodeViewModel(string name) { Name = name; }
            public EquipmentTreeNodeViewModel(EquipmentPiece piece) { Name = piece.Name; EquipmentPiece = piece; }
        }

        public ObservableCollection<EquipmentTreeNodeViewModel> EquipmentTree { get; } = new();
        private EquipmentTreeNodeViewModel? selectedEquipmentTreeItem;
        public EquipmentTreeNodeViewModel? SelectedEquipmentTreeItem
        {
            get => selectedEquipmentTreeItem;
            set
            {
                if (SetProperty(ref selectedEquipmentTreeItem, value))
                {
                    if (value?.EquipmentPiece != null)
                        SelectedEquipment = value.EquipmentPiece;
                }
            }
        }
        private void BuildEquipmentTree()
        {
            EquipmentTree.Clear();
            var byCategory = EquipmentList.GroupBy(e => e.Category);
            foreach (var catGroup in byCategory)
            {
                var catNode = new EquipmentTreeNodeViewModel(catGroup.Key);
                var bySubcat = catGroup.GroupBy(e => e.Subcategory);
                foreach (var subGroup in bySubcat)
                {
                    var subNode = new EquipmentTreeNodeViewModel(subGroup.Key);
                    foreach (var eq in subGroup)
                        subNode.Children.Add(new EquipmentTreeNodeViewModel(eq));
                    catNode.Children.Add(subNode);
                }
                EquipmentTree.Add(catNode);
            }
        }
        partial void OnEquipmentListChanged(ObservableCollection<EquipmentPiece> value)
        {
            BuildEquipmentTree();
        }

        // TODO: Add persistence hooks

        public ObservableCollection<Rank> AllRanks { get; } = new ObservableCollection<Rank>((Rank[])Enum.GetValues(typeof(Rank)));
    }
}

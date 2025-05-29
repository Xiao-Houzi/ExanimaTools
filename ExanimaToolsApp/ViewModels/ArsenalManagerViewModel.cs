using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExanimaTools.Models;
using ExanimaTools.Persistence;
using ExanimaTools.ViewModels;
using System.Collections.Generic;

namespace ExanimaTools.ViewModels;

public partial class ArsenalManagerViewModel : ObservableObject
{
    private readonly EquipmentRepository _equipmentRepository;
    private readonly ArsenalRepository _arsenalRepository;

    [ObservableProperty]
    private ObservableCollection<EquipmentPiece> filteredEquipment = new();
    [ObservableProperty]
    private ObservableCollection<EquipmentPiece> arsenalEquipment = new();
    [ObservableProperty]
    private EquipmentPiece? selectedDatabaseEquipment;
    [ObservableProperty]
    private EquipmentPiece? selectedArsenalEquipment;
    [ObservableProperty]
    private string? searchText;
    [ObservableProperty]
    private string? statusMessage;

    // Add dialog state and commands for adding equipment
    [ObservableProperty]
    private bool isAddDialogOpen;
    [ObservableProperty]
    private EquipmentPiece newEquipment = new EquipmentPiece();
    [ObservableProperty]
    private string? addDialogTitle;
    [ObservableProperty]
    private bool isEditMode;
    [ObservableProperty]
    private string? errorMessage;
    [ObservableProperty]
    private string? confirmationMessage;

    public ObservableCollection<string> CategoryOptions { get; } = new();
    public ObservableCollection<string> SubcategoryOptions { get; } = new();
    public Array EquipmentQualities => System.Enum.GetValues(typeof(EquipmentQuality));
    public Array EquipmentConditions => System.Enum.GetValues(typeof(EquipmentCondition));
    public Array AllRanks => System.Enum.GetValues(typeof(Rank));
    public ObservableCollection<StatType> AvailableStatTypes { get; } = new();
    private StatType? selectedStatType;
    public StatType? SelectedStatType { get => selectedStatType; set => SetProperty(ref selectedStatType, value); }
    public ObservableCollection<StatPipViewModel> NewEquipmentStatPips { get; } = new();

    private enum AddFormMode { None, Weapon, Armour }
    private AddFormMode addFormMode = AddFormMode.None;

    public ObservableCollection<EquipmentTreeNodeViewModel> PoolTree { get; } = new();
    public ObservableCollection<EquipmentTreeNodeViewModel> ArsenalTree { get; } = new();
    private EquipmentTreeNodeViewModel? selectedPoolTreeItem;
    public EquipmentTreeNodeViewModel? SelectedPoolTreeItem
    {
        get => selectedPoolTreeItem;
        set
        {
            if (SetProperty(ref selectedPoolTreeItem, value))
            {
                if (value?.EquipmentPiece != null)
                    SelectedDatabaseEquipment = value.EquipmentPiece;
            }
        }
    }
    private EquipmentTreeNodeViewModel? selectedArsenalTreeItem;
    public EquipmentTreeNodeViewModel? SelectedArsenalTreeItem
    {
        get => selectedArsenalTreeItem;
        set
        {
            if (SetProperty(ref selectedArsenalTreeItem, value))
            {
                if (value?.EquipmentPiece != null)
                    SelectedArsenalEquipment = value.EquipmentPiece;
            }
        }
    }

    // New: ViewModels for EquipmentTreeControl
    public EquipmentTreeBrowserViewModel PoolTreeViewModel { get; }
    public EquipmentTreeBrowserViewModel ArsenalTreeViewModel { get; }

    public ICommand SaveNewEquipmentCommand { get; }
    public ICommand CloseAddDialogCommand { get; }
    public ICommand AddStatCommand { get; }
    public ICommand ShowAddWeaponFormCommand { get; }
    public ICommand ShowAddArmourFormCommand { get; }
    public ICommand AddFilterCommand { get; }

    public ObservableCollection<EquipmentFilterViewModel> Filters { get; } = new();

    private List<EquipmentPiece> _allEquipment = new(); // Cache for filtering

    public ArsenalManagerViewModel(EquipmentRepository equipmentRepo, ArsenalRepository arsenalRepo)
    {
        _equipmentRepository = equipmentRepo;
        _arsenalRepository = arsenalRepo;
        // Tree-building is handled in LoadAsync and after add/remove actions.
        _ = LoadAsync(); // Ensure trees are populated on startup

        PoolTreeViewModel = new EquipmentTreeBrowserViewModel
        {
            TreeItems = PoolTree,
            SelectedTreeItem = SelectedPoolTreeItem,
            SearchText = string.Empty,
            ActionLabel = "Add",
            ActionCommand = AddToArsenalCommand
        };
        ArsenalTreeViewModel = new EquipmentTreeBrowserViewModel
        {
            TreeItems = ArsenalTree,
            SelectedTreeItem = SelectedArsenalTreeItem,
            SearchText = string.Empty,
            ActionLabel = "Remove",
            ActionCommand = RemoveFromArsenalCommand
        };
        SaveNewEquipmentCommand = new AsyncSimpleCommand(SaveNewEquipmentAsync);
        CloseAddDialogCommand = new SimpleCommand(CloseAddDialog);
        AddStatCommand = new SimpleCommand(AddStat);
        ShowAddWeaponFormCommand = new SimpleCommand(ShowAddWeaponForm);
        ShowAddArmourFormCommand = new SimpleCommand(ShowAddArmourForm);
        AddFilterCommand = new SimpleCommand(AddFilter);
        Filters.CollectionChanged += (s, e) => ApplyFilters();
    }

    private async Task LoadAsync()
    {
        var allEquipment = await _equipmentRepository.GetAllAsync();
        _allEquipment = allEquipment.ToList();
        FilteredEquipment = new ObservableCollection<EquipmentPiece>(_allEquipment);
        var arsenal = await _arsenalRepository.GetArsenalAsync(_equipmentRepository);
        ArsenalEquipment = new ObservableCollection<EquipmentPiece>(arsenal.Equipment);
        BuildTree(FilteredEquipment, PoolTree);
        BuildTree(ArsenalEquipment, ArsenalTree);
    }

    partial void OnSearchTextChanged(string? value)
    {
        _ = FilterEquipment();
    }

    private async Task FilterEquipment()
    {
        var all = await _equipmentRepository.GetAllAsync();
        if (string.IsNullOrWhiteSpace(SearchText))
            FilteredEquipment = new ObservableCollection<EquipmentPiece>(all);
        else
            FilteredEquipment = new ObservableCollection<EquipmentPiece>(all.Where(e => e.Name.Contains(SearchText, System.StringComparison.OrdinalIgnoreCase)));
    }

    [RelayCommand]
    public async Task AddToArsenalAsync(EquipmentPiece? piece = null)
    {
        var toAdd = piece ?? SelectedDatabaseEquipment;
        if (toAdd == null)
        {
            StatusMessage = "Select equipment to add.";
            return;
        }
        // Removed duplicate check to allow multiple of the same item
        await _arsenalRepository.AddToArsenalAsync(toAdd.Id);
        ArsenalEquipment.Add(toAdd);
        BuildTree(ArsenalEquipment, ArsenalTree);
        StatusMessage = $"Added {toAdd.Name} to arsenal.";
    }

    [RelayCommand]
    public async Task RemoveFromArsenalAsync()
    {
        if (SelectedArsenalEquipment == null)
        {
            StatusMessage = "Select equipment to remove.";
            return;
        }
        var name = SelectedArsenalEquipment.Name;
        await _arsenalRepository.RemoveFromArsenalAsync(SelectedArsenalEquipment.Id);
        ArsenalEquipment.Remove(SelectedArsenalEquipment);
        BuildTree(ArsenalEquipment, ArsenalTree);
        StatusMessage = $"Removed {name} from arsenal.";
    }

    // Move the tree-building logic into a private method
    private void BuildTree(ObservableCollection<EquipmentPiece> source, ObservableCollection<EquipmentTreeNodeViewModel> target)
    {
        target.Clear();
        var byType = source.GroupBy(e => e.Type);
        foreach (var typeGroup in byType)
        {
            var typeNode = new EquipmentTreeNodeViewModel(typeGroup.Key.ToString());
            var byCategory = typeGroup.GroupBy(e => e.Category);
            foreach (var catGroup in byCategory)
            {
                var catNode = new EquipmentTreeNodeViewModel(catGroup.Key);
                var bySubcat = catGroup.GroupBy(e => e.Subcategory);
                foreach (var subGroup in bySubcat)
                {
                    if (subGroup.Count() == 1 && subGroup.Key == subGroup.First().Name)
                    {
                        catNode.Children.Add(new EquipmentTreeNodeViewModel(subGroup.First()));
                    }
                    else
                    {
                        var subNode = new EquipmentTreeNodeViewModel(subGroup.Key);
                        foreach (var eq in subGroup)
                            subNode.Children.Add(new EquipmentTreeNodeViewModel(eq));
                        catNode.Children.Add(subNode);
                    }
                }
                typeNode.Children.Add(catNode);
            }
            target.Add(typeNode);
        }
    }

    private void CloseAddDialog()
    {
        IsAddDialogOpen = false;
    }

    private async Task SaveNewEquipmentAsync()
    {
        if (!ValidateEquipment(NewEquipment, out var error))
        {
            ErrorMessage = error;
            ConfirmationMessage = null;
            return;
        }
        await _equipmentRepository.AddAsync(NewEquipment);
        FilteredEquipment.Add(NewEquipment);
        PoolTree.Add(new EquipmentTreeNodeViewModel(NewEquipment));
        StatusMessage = $"Added '{NewEquipment.Name}' to equipment pool.";
        ConfirmationMessage = StatusMessage;
        ErrorMessage = null;
        IsAddDialogOpen = false;
        await LoadAsync(); // Refresh lists/trees
    }

    private void NotifyCanExecuteChanged()
    {
        // SaveNewEquipmentAsyncCommand.NotifyCanExecuteChanged();
    }

    partial void OnNewEquipmentChanged(EquipmentPiece value)
    {
        NotifyCanExecuteChanged();
    }

    private string selectedCategory = string.Empty;
    public string SelectedCategory
    {
        get => selectedCategory;
        set
        {
            if (SetProperty(ref selectedCategory, value))
            {
                SubcategoryOptions.Clear();
                if (addFormMode == AddFormMode.Weapon && !string.IsNullOrEmpty(value) && EquipmentManagerViewModel.WeaponCategorySubcategoryMap.TryGetValue(value, out var subs))
                {
                    foreach (var sub in subs)
                        SubcategoryOptions.Add(sub);
                    SelectedSubcategory = SubcategoryOptions.FirstOrDefault() ?? string.Empty;
                }
                else if (addFormMode == AddFormMode.Armour && !string.IsNullOrEmpty(value) && EquipmentManagerViewModel.ArmourCategorySubcategoryMap.TryGetValue(value, out var subs2))
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
                NotifyCanExecuteChanged();
            }
        }
    }
    private string selectedSubcategory = string.Empty;
    public string SelectedSubcategory
    {
        get => selectedSubcategory;
        set
        {
            if (SetProperty(ref selectedSubcategory, value))
            {
                NewEquipment.Subcategory = value;
                NotifyCanExecuteChanged();
            }
        }
    }

    private void SyncStatPipViewModels()
    {
        NewEquipmentStatPips.Clear();
        foreach (var kvp in NewEquipment.Stats)
        {
            if (kvp.Key == StatType.Weight) continue;
            var pipVm = new StatPipViewModel(kvp.Key, kvp.Value, v => NewEquipment.SetStat(kvp.Key, v), null);
            NewEquipmentStatPips.Add(pipVm);
        }
        UpdateAvailableStatTypes();
    }
    private void UpdateAvailableStatTypes()
    {
        AvailableStatTypes.Clear();
        IEnumerable<StatType> allowed = NewEquipment.Type switch
        {
            EquipmentType.Weapon => new[] { StatType.Balance, StatType.Impact, StatType.Slash, StatType.Crush, StatType.Pierce, StatType.Thrust, StatType.Points },
            EquipmentType.Armour => new[] { StatType.SlashProtection, StatType.CrushProtection, StatType.PierceProtection, StatType.Points },
            _ => System.Array.Empty<StatType>()
        };
        foreach (var stat in allowed.Except(NewEquipment.Stats.Keys))
            AvailableStatTypes.Add(stat);
        if (AvailableStatTypes.Count > 0)
            SelectedStatType = AvailableStatTypes[0];
        else
            SelectedStatType = null;
    }

    private void AddStat()
    {
        if (SelectedStatType is StatType statType && !NewEquipment.Stats.ContainsKey(statType))
        {
            NewEquipment.Stats[statType] = 0.5f;
            SyncStatPipViewModels();
        }
    }

    // Dummy property to force source generator to run
    public bool ForceSourceGen { get; set; } = false;

    private bool ValidateEquipment(EquipmentPiece equipment, out string? error)
    {
        if (string.IsNullOrWhiteSpace(equipment.Name))
        {
            error = "Name is required.";
            return false;
        }
        if (string.IsNullOrWhiteSpace(equipment.Category))
        {
            error = "Category is required.";
            return false;
        }
        if (string.IsNullOrWhiteSpace(equipment.Subcategory))
        {
            error = "Subcategory is required.";
            return false;
        }
        error = null;
        return true;
    }

    private void ShowAddWeaponForm()
    {
        addFormMode = AddFormMode.Weapon;
        SetCategoryOptionsForMode();
        SelectedCategory = CategoryOptions.FirstOrDefault() ?? string.Empty;
        SelectedSubcategory = SubcategoryOptions.FirstOrDefault() ?? string.Empty;
        NewEquipment = new EquipmentPiece()
        {
            Type = EquipmentType.Weapon,
            Slot = EquipmentSlot.Hands,
            Layer = null,
            Stats = new[] { StatType.Encumbrance }.ToDictionary(st => st, st => 0.5f),
            Category = SelectedCategory,
            Subcategory = SelectedSubcategory,
            Rank = Rank.Inept
        };
        AddDialogTitle = "Add Weapon";
        IsEditMode = false;
        IsAddDialogOpen = true;
        SyncStatPipViewModels();
    }
    private void ShowAddArmourForm()
    {
        addFormMode = AddFormMode.Armour;
        SetCategoryOptionsForMode();
        SelectedCategory = CategoryOptions.FirstOrDefault() ?? string.Empty;
        SelectedSubcategory = SubcategoryOptions.FirstOrDefault() ?? string.Empty;
        NewEquipment = new EquipmentPiece()
        {
            Type = EquipmentType.Armour,
            Slot = EquipmentSlot.Body,
            Layer = ArmourLayer.Padding,
            Stats = new[] { StatType.ImpactResistance, StatType.Encumbrance, StatType.Coverage }.ToDictionary(st => st, st => 0.5f),
            Category = SelectedCategory,
            Subcategory = SelectedSubcategory,
            Rank = Rank.Inept
        };
        AddDialogTitle = "Add Armour";
        IsEditMode = false;
        IsAddDialogOpen = true;
        SyncStatPipViewModels();
    }

    private void SetCategoryOptionsForMode()
    {
        CategoryOptions.Clear();
        if (addFormMode == AddFormMode.Weapon)
        {
            foreach (var cat in EquipmentManagerViewModel.WeaponCategorySubcategoryMap.Keys)
                CategoryOptions.Add(cat);
        }
        else if (addFormMode == AddFormMode.Armour)
        {
            foreach (var cat in EquipmentManagerViewModel.ArmourCategorySubcategoryMap.Keys)
                CategoryOptions.Add(cat);
        }
    }

    private void AddFilter()
    {
        var filter = new EquipmentFilterViewModel { FilterField = EquipmentFilterField.Category, Operator = EquipmentFilterOperator.Equals };
        filter.RemoveCommand = new SimpleCommand(() => { Filters.Remove(filter); });
        Filters.Add(filter);
    }

    private void ApplyFilters()
    {
        FilteredEquipment = new ObservableCollection<EquipmentPiece>(GetFilteredEquipment(_allEquipment, Filters));
    }

    public static List<EquipmentPiece> GetFilteredEquipment(IEnumerable<EquipmentPiece> source, IEnumerable<EquipmentFilterViewModel> filters)
    {
        var all = source.ToList();
        foreach (var filter in filters)
        {
            all = ApplyFilterStatic(all, filter);
        }
        return all;
    }

    private static List<EquipmentPiece> ApplyFilterStatic(List<EquipmentPiece> source, EquipmentFilterViewModel filter)
    {
        switch (filter.FilterField)
        {
            case EquipmentFilterField.Category:
                if (filter.Value is string cat && !string.IsNullOrEmpty(cat))
                {
                    if (filter.Operator == EquipmentFilterOperator.Equals)
                        return source.FindAll(e => e.Category == cat);
                    if (filter.Operator == EquipmentFilterOperator.NotEquals)
                        return source.FindAll(e => e.Category != cat);
                }
                break;
            case EquipmentFilterField.Condition:
                if (filter.Value is EquipmentCondition cond)
                {
                    if (filter.Operator == EquipmentFilterOperator.Equals)
                        return source.FindAll(e => e.Condition == cond);
                    if (filter.Operator == EquipmentFilterOperator.NotEquals)
                        return source.FindAll(e => e.Condition != cond);
                }
                break;
            case EquipmentFilterField.Rank:
                if (filter.Value is Rank rank)
                {
                    if (filter.Operator == EquipmentFilterOperator.Equals)
                        return source.FindAll(e => e.Rank == rank);
                    if (filter.Operator == EquipmentFilterOperator.NotEquals)
                        return source.FindAll(e => e.Rank != rank);
                    if (filter.Operator == EquipmentFilterOperator.GreaterThan)
                        return source.FindAll(e => e.Rank > rank);
                    if (filter.Operator == EquipmentFilterOperator.LessThan)
                        return source.FindAll(e => e.Rank < rank);
                    if (filter.Operator == EquipmentFilterOperator.GreaterOrEqual)
                        return source.FindAll(e => e.Rank >= rank);
                    if (filter.Operator == EquipmentFilterOperator.LessOrEqual)
                        return source.FindAll(e => e.Rank <= rank);
                }
                break;
            case EquipmentFilterField.Stat:
                if (filter.Value is float statVal && !string.IsNullOrEmpty(filter.StatName))
                {
                    if (Enum.TryParse<StatType>(filter.StatName, out var statType))
                    {
                        switch (filter.Operator)
                        {
                            case EquipmentFilterOperator.Equals:
                                return source.FindAll(e => e.Stats.TryGetValue(statType, out var v) && v == statVal);
                            case EquipmentFilterOperator.NotEquals:
                                return source.FindAll(e => e.Stats.TryGetValue(statType, out var v) && v != statVal);
                            case EquipmentFilterOperator.GreaterThan:
                                return source.FindAll(e => e.Stats.TryGetValue(statType, out var v) && v > statVal);
                            case EquipmentFilterOperator.LessThan:
                                return source.FindAll(e => e.Stats.TryGetValue(statType, out var v) && v < statVal);
                            case EquipmentFilterOperator.GreaterOrEqual:
                                return source.FindAll(e => e.Stats.TryGetValue(statType, out var v) && v >= statVal);
                            case EquipmentFilterOperator.LessOrEqual:
                                return source.FindAll(e => e.Stats.TryGetValue(statType, out var v) && v <= statVal);
                        }
                    }
                }
                break;
        }
        return source;
    }

    // Simple ICommand implementation for sync commands
    public class SimpleCommand : ICommand
    {
        private readonly Action _execute;
        public SimpleCommand(Action execute) => _execute = execute;
        public event EventHandler? CanExecuteChanged { add { } remove { } }
        public bool CanExecute(object? parameter) => true;
        public void Execute(object? parameter) => _execute();
    }
    // ICommand implementation for async commands
    public class AsyncSimpleCommand : ICommand
    {
        private readonly Func<Task> _execute;
        public AsyncSimpleCommand(Func<Task> execute) => _execute = execute;
        public event EventHandler? CanExecuteChanged { add { } remove { } }
        public bool CanExecute(object? parameter) => true;
        public async void Execute(object? parameter) => await _execute();
    }
}

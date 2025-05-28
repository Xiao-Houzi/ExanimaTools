using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExanimaTools.Models;
using ExanimaTools.Persistence;
using ExanimaTools.ViewModels;

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

    public ArsenalManagerViewModel(EquipmentRepository equipmentRepo, ArsenalRepository arsenalRepo)
    {
        _equipmentRepository = equipmentRepo;
        _arsenalRepository = arsenalRepo;
        // Tree-building is handled in LoadAsync and after add/remove actions.
        _ = LoadAsync(); // Ensure trees are populated on startup
    }

    private async Task LoadAsync()
    {
        var allEquipment = await _equipmentRepository.GetAllAsync();
        FilteredEquipment = new ObservableCollection<EquipmentPiece>(allEquipment);
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
}

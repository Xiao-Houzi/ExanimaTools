using System.Collections.ObjectModel;
using Avalonia.Controls;
using System.Windows.Input;
using System.ComponentModel;
using ExanimaTools.Models;

namespace ExanimaTools.ViewModels
{
    public class EquipmentTreeBrowserViewModel : INotifyPropertyChanged
    {
        private readonly ILoggingService? _logger;
        public EquipmentTreeBrowserViewModel(ILoggingService? logger = null)
        {
            _logger = logger;
            _logger?.LogOperation("EquipmentTreeBrowserViewModel", "Created");
        }
        private ObservableCollection<EquipmentTreeNodeViewModel> treeItems = new();
        public ObservableCollection<EquipmentTreeNodeViewModel> TreeItems
        {
            get => treeItems;
            set { if (treeItems != value) { treeItems = value; OnPropertyChanged(nameof(TreeItems)); _logger?.Log($"TreeItems changed: {treeItems.Count} items"); } }
        }

        private EquipmentTreeNodeViewModel? selectedTreeItem;
        public EquipmentTreeNodeViewModel? SelectedTreeItem
        {
            get => selectedTreeItem;
            set { if (selectedTreeItem != value) { selectedTreeItem = value; OnPropertyChanged(nameof(SelectedTreeItem)); _logger?.Log($"SelectedTreeItem changed: {selectedTreeItem?.Name}"); } }
        }

        private string searchText = string.Empty;
        public string SearchText
        {
            get => searchText;
            set { if (searchText != value) { searchText = value; OnPropertyChanged(nameof(SearchText)); } }
        }

        private string actionLabel = "";
        public string ActionLabel
        {
            get => actionLabel;
            set { if (actionLabel != value) { actionLabel = value; OnPropertyChanged(nameof(ActionLabel)); } }
        }

        private ICommand? actionCommand;
        public ICommand? ActionCommand
        {
            get => actionCommand;
            set { if (actionCommand != value) { actionCommand = value; OnPropertyChanged(nameof(ActionCommand)); } }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

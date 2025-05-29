using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Avalonia.Controls;
using System.Windows.Input;

namespace ExanimaTools.ViewModels
{
    public partial class EquipmentTreeBrowserViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<EquipmentTreeNodeViewModel> treeItems = new();

        [ObservableProperty]
        private EquipmentTreeNodeViewModel? selectedTreeItem;

        [ObservableProperty]
        private string searchText = string.Empty;

        [ObservableProperty]
        private string actionLabel = "";

        [ObservableProperty]
        private ICommand? actionCommand;
    }
}

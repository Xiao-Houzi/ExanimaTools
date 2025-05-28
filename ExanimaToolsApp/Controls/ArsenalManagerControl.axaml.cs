using Avalonia.Controls;
using ExanimaTools.Models;
using ExanimaTools.Persistence;
using ExanimaTools.ViewModels;
using ExanimaToolsApp;

namespace ExanimaTools.Controls;

public partial class ArsenalManagerControl : UserControl
{
    public ArsenalManagerControl()
    {
        InitializeComponent();
        if (Avalonia.Controls.Design.IsDesignMode)
            return;
        // Use default DB path as in EquipmentManagerViewModel
        var dbPath = DbManager.GetDbPath();
        var equipmentRepo = new EquipmentRepository($"Data Source={dbPath}");
        var arsenalRepo = new ArsenalRepository($"Data Source={dbPath}");
        DataContext = new ArsenalManagerViewModel(equipmentRepo, arsenalRepo);
    }
}

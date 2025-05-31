using Avalonia.Controls;
using ExanimaTools.Persistence;
using ExanimaTools.ViewModels;
using ExanimaToolsApp;
using ExanimaTools.Models;

namespace ExanimaTools.Controls
{
    public partial class ArsenalManagerControl : UserControl
    {
        public ArsenalManagerControl()
        {
            InitializeComponent();
            var logger = ExanimaTools.App.LoggingServiceInstance;
            logger?.LogOperation("ArsenalManagerControl", "Created");
            if (DataContext == null)
            {
                var dbPath = DbManager.GetDbPath();
                var equipmentRepo = new EquipmentRepository($"Data Source={dbPath}");
                var arsenalRepo = new ArsenalRepository($"Data Source={dbPath}");
                DataContext = new ArsenalManagerViewModel(equipmentRepo, arsenalRepo, logger);
            }
        }
    }
}

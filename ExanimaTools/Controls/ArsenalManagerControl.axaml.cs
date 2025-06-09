using Avalonia.Controls;
using ExanimaTools.Persistence;
using ExanimaTools.ViewModels;
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
                var equipmentRepo = DbManager.GetEquipmentRepository(logger);
                var arsenalRepo = DbManager.GetArsenalRepository();
                DataContext = new ArsenalManagerViewModel(equipmentRepo, arsenalRepo, logger);
            }
        }
    }
}

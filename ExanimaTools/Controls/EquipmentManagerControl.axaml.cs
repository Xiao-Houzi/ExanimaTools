// No work should be done in this file without understanding the development practices.
// See: project-management/development_practices.md

using Avalonia.Controls;
using ExanimaTools.Services;
using ExanimaTools.Models;
using ExanimaTools.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace ExanimaTools.Controls
{
    public partial class EquipmentManagerControl : UserControl
    {
        public EquipmentManagerControl()
        {
            InitializeComponent();
            
            // Use service locator for Avalonia controls where constructor injection isn't available
            var serviceLocator = Program.ServiceProvider.GetRequiredService<IServiceLocator>();
            var logger = serviceLocator.GetRequiredService<ILoggingService>();
            var vm = serviceLocator.GetRequiredService<EquipmentManagerViewModel>();
            
            this.DataContext = vm;
            logger.LogOperation("EquipmentManagerControl.DataContextSet", $"DataContext set to {vm.GetType().Name} via service locator");
        }
    }
}

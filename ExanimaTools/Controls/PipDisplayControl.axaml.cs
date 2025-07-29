// No work should be done in this file without understanding the development practices.
// See: project-management/development_practices.md

using Avalonia.Controls;
using Avalonia.Input;
using ExanimaTools.Models;
using Microsoft.Extensions.DependencyInjection;

namespace ExanimaTools.Controls
{
    public partial class PipDisplayControl : UserControl
    {
        private readonly ILoggingService _logger;

        public PipDisplayControl()
        {
            _logger = Program.ServiceProvider.GetRequiredService<ILoggingService>();
            InitializeComponent();
            this.DataContextChanged += (s, e) => {
                var vm = this.DataContext;
                if (vm != null)
                {
                    _logger.LogOperation("PipDisplayControl.DataContextChanged", $"New DataContext: {vm.GetType().Name}");
                }
                else
                {
                    _logger.LogOperation("PipDisplayControl.DataContextChanged", "New DataContext: null");
                }
            };
        }

        private void PipHalf_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            // TODO: Implement logic to set pip to half (or call ViewModel)
        }

        private void PipFull_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            // TODO: Implement logic to set pip to full (or call ViewModel)
        }
    }
}

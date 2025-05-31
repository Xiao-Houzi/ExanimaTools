using Avalonia.Controls;
using Avalonia.Input;

namespace ExanimaTools.Controls
{
    public partial class PipDisplayControl : UserControl
    {
        public PipDisplayControl()
        {
            InitializeComponent();
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

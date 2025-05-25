using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using ExanimaTools.ViewModels;

namespace ExanimaTools.Controls
{
    public partial class PipDisplayControl : UserControl
    {
        public PipDisplayControl()
        {
            InitializeComponent();
        }

        private void PipFull_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (DataContext is PipDisplayViewModel vm && sender is Avalonia.Controls.Shapes.Ellipse ellipse)
            {
                var border = ellipse.FindAncestorOfType<Border>();
                var itemsControl = border?.FindAncestorOfType<ItemsControl>();
                if (itemsControl != null && border != null)
                {
                    int pipIndex = itemsControl.IndexFromContainer(border);
                    vm.SetValueFromPip(pipIndex, false);
                }
            }
        }

        private void PipHalf_PointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (DataContext is PipDisplayViewModel vm && sender is Avalonia.Controls.Shapes.Ellipse ellipse)
            {
                var border = ellipse.FindAncestorOfType<Border>();
                var itemsControl = border?.FindAncestorOfType<ItemsControl>();
                if (itemsControl != null && border != null)
                {
                    int pipIndex = itemsControl.IndexFromContainer(border);
                    vm.SetValueFromPip(pipIndex, true);
                }
            }
        }
    }
}

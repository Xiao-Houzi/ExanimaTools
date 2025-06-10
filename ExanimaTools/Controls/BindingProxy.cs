using Avalonia;
using Avalonia.Controls;

namespace ExanimaTools.Controls
{
    public class BindingProxy : Control
    {
        public static readonly StyledProperty<object?> DataProperty =
            AvaloniaProperty.Register<BindingProxy, object?>(nameof(Data));

        public object? Data
        {
            get => GetValue(DataProperty);
            set => SetValue(DataProperty, value);
        }
    }
}

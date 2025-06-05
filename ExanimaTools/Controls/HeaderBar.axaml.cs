using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;

namespace ExanimaTools.Controls
{
    public partial class HeaderBar : UserControl
    {
        public HeaderBar()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void OnCloseButtonClick(object? sender, RoutedEventArgs e)
        {
            // Find the main window and close it
            var window = (this.GetVisualRoot() as Window);
            window?.Close();
        }
    }
}

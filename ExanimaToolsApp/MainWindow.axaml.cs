using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;
using Avalonia.Controls.ApplicationLifetimes;
using System.Linq;
using System;

namespace ExanimaTools;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        // Get all screens
        var screens = this.Screens?.All;
        if (screens != null && screens.Count > 0)
        {
            // Find the first portrait monitor (height > width)
            var portrait = screens.FirstOrDefault(s => s.WorkingArea.Height > s.WorkingArea.Width);
            if (portrait != null)
            {
                var wa = portrait.WorkingArea;
                // Make window fullscreen and borderless on the portrait monitor
                this.WindowStartupLocation = WindowStartupLocation.Manual;
                this.Position = new PixelPoint(wa.X, wa.Y);
                this.Width = wa.Width;
                this.Height = wa.Height;
                this.SystemDecorations = SystemDecorations.None;
                this.CanResize = false;
                this.Topmost = true;
            }
        }
    }

    private void OnCloseButtonClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        this.Close();
    }
}
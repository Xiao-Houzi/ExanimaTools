// No work should be done in this file without understanding the development practices.
// See: project-management/development_practices.md

using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;
using Avalonia.Controls.ApplicationLifetimes;
using System.Linq;
using System;
using ExanimaTools.Models;
using ExanimaTools.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace ExanimaTools;

public partial class MainWindow : Window
{
    private readonly CompanyViewModel _companyViewModel;
    private readonly ILoggingService _logger;

    public MainWindow(CompanyViewModel companyViewModel, ILoggingService logger)
    {
        _companyViewModel = companyViewModel ?? throw new ArgumentNullException(nameof(companyViewModel));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        try
        {
            _logger.LogOperation("MainWindow", "Starting initialization");
            InitializeComponent();
            _logger.LogOperation("MainWindow", "InitializeComponent completed");

            // Get all screens
            var screens = this.Screens?.All;
            if (screens != null && screens.Count > 0)
            {
                _logger.LogOperation("MainWindow", $"Found {screens.Count} screens");
                // Find the first portrait monitor (height > width)
                var portrait = screens.FirstOrDefault(s => s.WorkingArea.Height > s.WorkingArea.Width);
                if (portrait != null)
                {
                    var wa = portrait.WorkingArea;
                    _logger.LogOperation("MainWindow", $"Configuring for portrait monitor: {wa.Width}x{wa.Height}");
                    // Make window fullscreen and borderless on the portrait monitor
                    this.WindowStartupLocation = WindowStartupLocation.Manual;
                    this.Position = new PixelPoint(wa.X, wa.Y);
                    this.Width = wa.Width;
                    this.Height = wa.Height;
                    this.SystemDecorations = SystemDecorations.None;
                    this.CanResize = false;
                    this.Topmost = true;
                }
                else
                {
                    _logger.LogOperation("MainWindow", "No portrait monitor found, using default window configuration");
                }
            }
            else
            {
                _logger.LogOperation("MainWindow", "No screens found, using default window configuration");
            }
            
            // Set the DataContext for the window so all bindings work
            this.DataContext = _companyViewModel;
            _logger.LogOperation("MainWindow", "DataContext set");

            // Ensure the property is set after everything is initialized
            this.Opened += (_, __) =>
            {
                try
                {
                    if (MemberManagementControl != null)
                    {
                        MemberManagementControl.ArsenalManagerViewModel = _companyViewModel.ArsenalManagerViewModel;
                        _logger.LogOperation("MainWindow", "ArsenalManagerViewModel set on MemberManagementControl");
                    }
                }
                catch (Exception openedEx)
                {
                    _logger.LogError("Error in MainWindow.Opened event handler", openedEx);
                }
            };
            
            _logger.LogOperation("MainWindow", "Initialization completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError("Error during MainWindow initialization", ex);
            throw;
        }
    }

    [Obsolete("Use dependency injection constructor instead")]
    public MainWindow() : this(
        Program.ServiceProvider.GetRequiredService<CompanyViewModel>(),
        Program.ServiceProvider.GetRequiredService<ILoggingService>()) { }

    [Obsolete("Use dependency injection constructor instead")]
    public MainWindow(ILoggingService? logger) : this(
        Program.ServiceProvider.GetRequiredService<CompanyViewModel>(),
        logger ?? Program.ServiceProvider.GetRequiredService<ILoggingService>()) { }

    // ...existing code...

    public string? StatusMessage { get; set; }

    private void OnCloseButtonClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        this.Close();
    }
}
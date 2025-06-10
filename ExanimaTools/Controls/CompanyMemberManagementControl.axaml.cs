using Avalonia;
using Avalonia.Controls;
using ExanimaTools.ViewModels;

namespace ExanimaTools.Controls;

public partial class CompanyMemberManagementControl : UserControl
{
    public CompanyMemberManagementControl()
    {
        InitializeComponent();
    }

    public static readonly StyledProperty<ArsenalManagerViewModel> ArsenalManagerViewModelProperty =
        AvaloniaProperty.Register<CompanyMemberManagementControl, ArsenalManagerViewModel>(nameof(ArsenalManagerViewModel));

    public ArsenalManagerViewModel ArsenalManagerViewModel
    {
        get => GetValue(ArsenalManagerViewModelProperty);
        set => SetValue(ArsenalManagerViewModelProperty, value);
    }
}

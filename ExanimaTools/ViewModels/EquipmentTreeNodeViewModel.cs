// No work should be done in this file without understanding the development practices.
// See: project-management/development_practices.md

using System.Collections.ObjectModel;
using ExanimaTools.Models;
using System.Linq;

namespace ExanimaTools.ViewModels;

public class EquipmentTreeNodeViewModel
{
    private readonly ILoggingService? _logger;
    public string Name { get; set; } = string.Empty;
    public ObservableCollection<EquipmentTreeNodeViewModel> Children { get; set; } = new();
    public EquipmentPiece? EquipmentPiece { get; set; }
    public bool IsCategory => EquipmentPiece == null;
    public bool IsLeaf => EquipmentPiece != null;
    public ObservableCollection<ActionButtonViewModel> ActionButtons { get; } = new();

    public EquipmentTreeNodeViewModel(string name, ILoggingService? logger = null) { Name = name; _logger = logger; _logger?.LogOperation("EquipmentTreeNodeViewModel", $"Category node created: {name}"); }
    public EquipmentTreeNodeViewModel(EquipmentPiece piece, ILoggingService? logger = null) { Name = piece.Name; EquipmentPiece = piece; _logger = logger; _logger?.LogOperation("EquipmentTreeNodeViewModel", $"Leaf node created: {piece.Name}"); }

    // In your tree-building logic, populate ActionButtons for leaf nodes as needed.
    // Example:
    // if (IsLeaf) ActionButtons.Add(new ActionButtonViewModel { Label = "Add to Arsenal", Command = addToArsenalCommand });

    public int TotalDescendantCount => IsLeaf ? 1 : (Children?.Sum(child => child.TotalDescendantCount) ?? 0);
    public override string ToString()
    {
        if (IsLeaf && EquipmentPiece != null)
            return EquipmentPiece.Name;
        return Name;
    }
}

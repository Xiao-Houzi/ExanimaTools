using System.Collections.ObjectModel;
using ExanimaTools.Models;

namespace ExanimaTools.ViewModels;

public class EquipmentTreeNodeViewModel
{
    private readonly ILoggingService? _logger;
    public string Name { get; set; } = string.Empty;
    public ObservableCollection<EquipmentTreeNodeViewModel> Children { get; set; } = new();
    public EquipmentPiece? EquipmentPiece { get; set; }
    public bool IsCategory => EquipmentPiece == null;
    public bool IsLeaf => EquipmentPiece != null;
    public EquipmentTreeNodeViewModel(string name, ILoggingService? logger = null) { Name = name; _logger = logger; _logger?.LogOperation("EquipmentTreeNodeViewModel", $"Category node created: {name}"); }
    public EquipmentTreeNodeViewModel(EquipmentPiece piece, ILoggingService? logger = null) { Name = piece.Name; EquipmentPiece = piece; _logger = logger; _logger?.LogOperation("EquipmentTreeNodeViewModel", $"Leaf node created: {piece.Name}"); }
}

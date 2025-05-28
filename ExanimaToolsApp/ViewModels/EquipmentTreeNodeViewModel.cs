using System.Collections.ObjectModel;
using ExanimaTools.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ExanimaTools.ViewModels;

public class EquipmentTreeNodeViewModel : ObservableObject
{
    public string Name { get; set; } = string.Empty;
    public ObservableCollection<EquipmentTreeNodeViewModel> Children { get; set; } = new();
    public EquipmentPiece? EquipmentPiece { get; set; }
    public bool IsCategory => EquipmentPiece == null;
    public bool IsLeaf => EquipmentPiece != null;
    public EquipmentTreeNodeViewModel(string name) { Name = name; }
    public EquipmentTreeNodeViewModel(EquipmentPiece piece) { Name = piece.Name; EquipmentPiece = piece; }
}

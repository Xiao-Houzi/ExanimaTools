using System.Collections.Generic;
using System.Windows.Input;

namespace ExanimaTools.Controls
{
    public class UniversalTreeNodeAction
    {
        public string Label { get; set; } = string.Empty;
        public ICommand? Command { get; set; }
        public object? CommandParameter { get; set; }
        public string? Icon { get; set; }
    }

    public interface IUniversalTreeNode
    {
        string DisplayName { get; }
        bool IsExpanded { get; set; }
        IList<IUniversalTreeNode> Children { get; }
        void SetChildren(IEnumerable<object> children);
        List<UniversalTreeNodeAction> Actions { get; set; }
    }

    // Simple default tree node implementation
    public class UniversalTreeNode : IUniversalTreeNode
    {
        public object Data { get; set; }
        public List<object> Children { get; set; } = new();
        public List<UniversalTreeNodeAction> Actions { get; set; } = new();

        public UniversalTreeNode(object data)
        {
            Data = data;
        }

        public void SetChildren(IEnumerable<object> children)
        {
            Children = new List<object>(children);
        }

        string IUniversalTreeNode.DisplayName
        {
            get
            {
                if (Data == null) return string.Empty;
                
                // Handle specific types with name properties
                if (Data is ExanimaTools.Models.EquipmentPiece equipment)
                    return equipment.Name;
                    
                if (Data is ExanimaTools.Models.CompanyMember member)
                    return member.Name;
                    
                if (Data is ExanimaTools.Models.Arsenal arsenal)
                    return $"Arsenal ({arsenal.Equipment.Count} items)";
                
                // Fallback to ToString for other types
                return Data.ToString() ?? string.Empty;
            }
        }
        bool IUniversalTreeNode.IsExpanded { get; set; }
        IList<IUniversalTreeNode> IUniversalTreeNode.Children => Children.ConvertAll(child => (IUniversalTreeNode)child);
    }
}

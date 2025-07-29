# Universal Tree Control Documentation

## Overview

The Universal Tree Control is a reusable, configurable Avalonia UI control designed to provide consistent tree view functionality across the ExanimaTools application. It supports dynamic filtering, custom item templates, action button injection, and builder-pattern construction.

## Features

### ✅ Core Features
- **Silver Border Styling**: Consistent visual appearance with silver border and rounded corners
- **Builder Pattern**: Flexible tree construction using `UniversalTreeBuilder<T>`
- **Action Button Injection**: Add custom buttons to tree nodes (e.g., "Add to Arsenal", "Remove")
- **Dynamic Filtering**: Real-time filtering with custom predicates
- **Template Injection**: Custom item templates for different node types
- **Type Safety**: Generic builder supports any data type
- **Hierarchical Support**: Handles both flat and nested data structures

### 🔄 Enhanced Features (Latest Updates)
- **Dynamic Filter Updates**: Filter property changes automatically update tree content
- **Custom Filter Predicates**: Support for complex filtering logic beyond string matching
- **Original Data Preservation**: Maintains unfiltered data for efficient re-filtering
- **Recursive Filtering**: Supports filtering on child nodes and preserving parent context

## Basic Usage

### 1. XAML Declaration

```xml
<controls:UniversalTreeControl 
    TreeItems="{Binding TreeItems}"
    SelectedItem="{Binding SelectedTreeItem}"
    Filter="{Binding FilterText}"
    FilterPredicate="{Binding CustomFilterPredicate}" />
```

### 2. ViewModel Setup

```csharp
public class MyViewModel : ViewModelBase
{
    private ObservableCollection<IUniversalTreeNode> _treeItems = new();
    private string _filterText = "";
    
    public ObservableCollection<IUniversalTreeNode> TreeItems 
    { 
        get => _treeItems; 
        set => SetProperty(ref _treeItems, value); 
    }
    
    public string FilterText 
    { 
        get => _filterText; 
        set => SetProperty(ref _filterText, value); 
    }
    
    // Optional: Custom filter logic
    public Func<object, string, bool> CustomFilterPredicate => (item, filter) =>
    {
        if (item is MyDataType myItem)
        {
            return myItem.Name.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                   myItem.Category.Contains(filter, StringComparison.OrdinalIgnoreCase);
        }
        return false;
    };
}
```

## Advanced Usage with Builder Pattern

### Building Tree from Equipment Data

```csharp
// In your ViewModel constructor or initialization method
private void SetupEquipmentTree()
{
    var builder = new UniversalTreeBuilder<EquipmentPiece>(
        displayNameSelector: e => e.Name,
        childrenSelector: e => GetChildEquipment(e), // Your logic for hierarchical data
        actionsSelector: e => CreateActionButtons(e)
    );
    
    // Configure the builder
    builder.WithViewModel(e => new EquipmentTreeNodeViewModel(e))
           .WithFilter(e => IsEquipmentVisible(e));
    
    // Build the tree
    var equipment = await _equipmentRepository.GetAllAsync();
    var treeNodes = builder.BuildTree(equipment);
    TreeItems = new ObservableCollection<IUniversalTreeNode>(treeNodes);
}

private IEnumerable<UniversalTreeNodeAction> CreateActionButtons(EquipmentPiece equipment)
{
    return new[]
    {
        new UniversalTreeNodeAction
        {
            Label = "Add to Arsenal",
            Command = AddToArsenalCommand,
            CommandParameter = equipment,
            Icon = "Plus"
        },
        new UniversalTreeNodeAction
        {
            Label = "View Details", 
            Command = ViewDetailsCommand,
            CommandParameter = equipment,
            Icon = "Eye"
        }
    };
}
```

## Filtering

### Basic String Filtering
If no `FilterPredicate` is provided, the control uses default string-based filtering:

```csharp
// This will filter items based on their ToString() representation
FilterText = "sword"; // Shows items containing "sword"
```

### Custom Filter Predicates
For advanced filtering scenarios:

```csharp
public Func<object, string, bool> EquipmentFilterPredicate => (item, filter) =>
{
    if (item is not EquipmentPiece equipment) return false;
    
    if (string.IsNullOrEmpty(filter)) return true;
    
    // Multiple field search
    return equipment.Name.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
           equipment.Category.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
           equipment.Type.ToString().Contains(filter, StringComparison.OrdinalIgnoreCase) ||
           equipment.Rank.ToString().Contains(filter, StringComparison.OrdinalIgnoreCase);
};
```

### Filter Behavior
- **Empty Filter**: Shows all items
- **Parent-Child Logic**: If a child matches the filter, its parents are also shown for context
- **Real-time Updates**: Filter changes immediately update the tree
- **Performance**: Original data is preserved to avoid re-querying

## Action Buttons

### Button Configuration
Action buttons appear on leaf nodes and support:

```csharp
public class UniversalTreeNodeAction
{
    public string Label { get; set; }        // Button text
    public ICommand Command { get; set; }    // Command to execute
    public object CommandParameter { get; set; } // Parameter for command
    public string Icon { get; set; }         // Icon identifier (optional)
}
```

### Example: Arsenal Management Actions

```csharp
private IEnumerable<UniversalTreeNodeAction> CreateArsenalActions(EquipmentPiece equipment)
{
    var actions = new List<UniversalTreeNodeAction>();
    
    if (CanAddToArsenal(equipment))
    {
        actions.Add(new UniversalTreeNodeAction
        {
            Label = "Add to Arsenal",
            Command = AddToArsenalCommand,
            CommandParameter = equipment
        });
    }
    
    if (CanRemoveFromArsenal(equipment))
    {
        actions.Add(new UniversalTreeNodeAction
        {
            Label = "Remove",
            Command = RemoveFromArsenalCommand,
            CommandParameter = equipment
        });
    }
    
    return actions;
}
```

## Custom Item Templates

### Template Selection
The control automatically uses different templates for leaf vs. non-leaf nodes:

- **Leaf Nodes**: Show action buttons, equipment details (Rank, Quality, Condition)
- **Non-Leaf Nodes**: Show name, child count, no action buttons

### Extending Templates
To add custom display logic, modify the `TreeDataTemplate` in `UniversalTreeControl.axaml`:

```xml
<TreeDataTemplate DataType="viewmodels:MyCustomNodeViewModel" ItemsSource="{Binding Children}">
    <StackPanel Orientation="Horizontal">
        <!-- Your custom template content -->
        <TextBlock Text="{Binding CustomProperty}" />
        <Image Source="{Binding CustomIcon}" />
    </StackPanel>
</TreeDataTemplate>
```

## Integration Examples

### Equipment Pool Tree
```csharp
// Equipment organized by Type -> Category -> Items
var equipmentBuilder = new UniversalTreeBuilder<EquipmentPiece>(
    e => e.Name,
    e => GetEquipmentsByCategory(e.Type),
    e => CreateEquipmentActions(e)
);
```

### Company Member Tree
```csharp
// Members organized by Role -> Rank -> Individual Members
var memberBuilder = new UniversalTreeBuilder<CompanyMember>(
    m => m.Name,
    m => GetMembersByRank(m.Role),
    m => CreateMemberActions(m)
);
```

### Arsenal Tree
```csharp
// Arsenal items organized by assignment status
var arsenalBuilder = new UniversalTreeBuilder<Arsenal>(
    a => a.EquipmentPiece.Name,
    a => new List<Arsenal>(), // Flat structure
    a => CreateArsenalActions(a)
);
```

## Performance Considerations

### Large Data Sets
- Use lazy loading in `childrenSelector` for large hierarchical data
- Implement efficient filtering predicates to avoid expensive operations
- Consider virtualization for very large trees

### Memory Management
- Builder pattern creates minimal object overhead
- Original data is preserved but filtered views are lightweight
- Action buttons are created on-demand during tree building

## Troubleshooting

### Common Issues

1. **Filter Not Working**
   - Ensure `FilterPredicate` is properly bound or use default string filtering
   - Check that the filter text binding is TwoWay

2. **Action Buttons Not Appearing**
   - Verify `actionsSelector` returns valid actions for leaf nodes
   - Check that `IsLeaf` property is correctly calculated

3. **Tree Not Updating**
   - Ensure TreeItems is an ObservableCollection
   - Check property change notifications in your ViewModel

### Debug Mode
Enable debug logging in the tree builder for troubleshooting:

```csharp
builder.WithFilter(e => {
    var result = MyFilterLogic(e);
    _logger.LogDebug($"Filter {e.Name}: {result}");
    return result;
});
```

## Migration Guide

### From Legacy Tree Controls
1. Replace custom tree XAML with `UniversalTreeControl`
2. Implement `UniversalTreeBuilder<T>` for your data type
3. Convert action callbacks to `UniversalTreeNodeAction` objects
4. Update filter logic to use `FilterPredicate` property

### Backwards Compatibility
The control maintains compatibility with existing tree implementations:
- Existing ViewModels can be gradually migrated
- Legacy action button patterns still work
- Filter bindings remain unchanged

## Future Enhancements

### Planned Features
- **Icon Support**: Enhanced icon system for action buttons and nodes
- **Drag & Drop**: Support for tree item reordering and cross-tree operations
- **Context Menus**: Right-click menu integration
- **Export/Import**: Tree state serialization

### Extension Points
- Custom node types implementing `IUniversalTreeNode`
- Pluggable filter providers
- Custom action button renderers
- Theme-aware styling support

---

*Last Updated: July 29, 2025*
*Universal Tree Control v2.0 - Enhanced with dynamic filtering*

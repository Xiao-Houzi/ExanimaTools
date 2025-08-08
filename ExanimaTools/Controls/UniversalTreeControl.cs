using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Markup.Xaml;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System;
using System.Linq;
using System.Collections.ObjectModel;

namespace ExanimaTools.Controls
{
    public partial class UniversalTreeControl : UserControl
    {
        public static readonly AvaloniaProperty<object?> TreeItemsProperty = AvaloniaProperty.Register<UniversalTreeControl, object?>(nameof(TreeItems));

        public static readonly AvaloniaProperty<object?> SelectedItemProperty = AvaloniaProperty.Register<UniversalTreeControl, object?>(nameof(SelectedItem));

        public static readonly AvaloniaProperty<IDataTemplate?> ItemTemplateProperty = AvaloniaProperty.Register<UniversalTreeControl, IDataTemplate?>(nameof(ItemTemplate));

        public static readonly AvaloniaProperty<string?> FilterProperty = AvaloniaProperty.Register<UniversalTreeControl, string?>(nameof(Filter), defaultValue: "", defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);

        public static readonly AvaloniaProperty<Func<object, string, bool>?> FilterPredicateProperty = AvaloniaProperty.Register<UniversalTreeControl, Func<object, string, bool>?>(nameof(FilterPredicate));

        public static readonly AvaloniaProperty<object?> OriginalTreeItemsProperty = AvaloniaProperty.Register<UniversalTreeControl, object?>(nameof(OriginalTreeItems));

        public object? TreeItems
        {
            get => (object?)GetValue(TreeItemsProperty);
            set => SetValue(TreeItemsProperty, value);
        }

        public object? SelectedItem
        {
            get => (object?)GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public IDataTemplate? ItemTemplate
        {
            get => (IDataTemplate?)GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        public string? Filter
        {
            get => (string?)GetValue(FilterProperty);
            set => SetValue(FilterProperty, value);
        }

        /// <summary>
        /// Custom filter predicate that takes (item, filterText) and returns true if item should be visible.
        /// If not set, default string-based filtering will be used.
        /// </summary>
        public Func<object, string, bool>? FilterPredicate
        {
            get => (Func<object, string, bool>?)GetValue(FilterPredicateProperty);
            set => SetValue(FilterPredicateProperty, value);
        }

        /// <summary>
        /// Stores the original unfiltered tree items for dynamic filtering
        /// </summary>
        public object? OriginalTreeItems
        {
            get => (object?)GetValue(OriginalTreeItemsProperty);
            set => SetValue(OriginalTreeItemsProperty, value);
        }

        public UniversalTreeControl()
        {
            AvaloniaXamlLoader.Load(this);
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            // Early safety check: ensure we're properly attached to visual tree
            if (!IsInitialized)
                return;

            try
            {
                base.OnPropertyChanged(change);
                
                // Additional safety check after base call
                if (Parent == null)
                    return;
                
                if (change.Property == FilterProperty)
                {
                    ApplyFilter();
                }
                else if (change.Property == TreeItemsProperty)
                {
                    // Handle TreeItems change safely
                    if (change.NewValue != null)
                    {
                        // Only store as original if we're not in the middle of filtering
                        if (OriginalTreeItems == null)
                        {
                            OriginalTreeItems = change.NewValue;
                        }
                    }
                    else
                    {
                        // Clear both original and current when TreeItems is set to null
                        OriginalTreeItems = null;
                    }
                }
            }
            catch (Exception)
            {
                // Silently handle visual tree disposal issues during tab switching
                // This prevents crashes when the control is being disposed
            }
        }

        /// <summary>
        /// Applies the current filter to the tree items dynamically
        /// </summary>
        private void ApplyFilter()
        {
            try
            {
                // Safety checks to prevent crashes during disposal or tab switching
                if (OriginalTreeItems == null || !IsInitialized || Parent == null) 
                    return;

                var filterText = Filter?.Trim() ?? "";
                
                if (string.IsNullOrEmpty(filterText))
                {
                    // No filter - show original items
                    TreeItems = OriginalTreeItems;
                    return;
                }

                // Apply filtering based on FilterPredicate or default string matching
                if (OriginalTreeItems is IEnumerable originalItems)
                {
                    var filteredItems = FilterTreeItems(originalItems, filterText);
                    TreeItems = filteredItems;
                }
            }
            catch (Exception)
            {
                // Silently handle any filtering issues during disposal
            }
        }

        /// <summary>
        /// Recursively filters tree items based on the filter text
        /// </summary>
        private IEnumerable FilterTreeItems(IEnumerable items, string filterText)
        {
            var result = new ObservableCollection<object>();
            
            foreach (var item in items)
            {
                if (item == null) continue;

                bool matchesFilter = false;
                
                // Use custom predicate if available, otherwise default string matching
                if (FilterPredicate != null)
                {
                    matchesFilter = FilterPredicate(item, filterText);
                }
                else
                {
                    // Default: check if item's string representation contains filter text
                    matchesFilter = item.ToString()?.Contains(filterText, StringComparison.OrdinalIgnoreCase) == true;
                }

                // Check if item has children and recursively filter them
                var hasMatchingChildren = false;
                IEnumerable? filteredChildren = null;
                
                if (item is IUniversalTreeNode treeNode && treeNode.Children.Any())
                {
                    filteredChildren = FilterTreeItems(treeNode.Children, filterText);
                    hasMatchingChildren = filteredChildren.Cast<object>().Any();
                }

                // Include item if it matches filter OR has matching children
                if (matchesFilter || hasMatchingChildren)
                {
                    // If it's a tree node with filtered children, update the children
                    if (item is IUniversalTreeNode nodeWithChildren && filteredChildren != null)
                    {
                        // Create a copy of the node with filtered children
                        var clonedNode = CloneTreeNode(nodeWithChildren);
                        clonedNode.SetChildren(filteredChildren.Cast<object>());
                        result.Add(clonedNode);
                    }
                    else
                    {
                        result.Add(item);
                    }
                }
            }
            
            return result;
        }

        /// <summary>
        /// Creates a shallow copy of a tree node for filtering purposes
        /// </summary>
        private IUniversalTreeNode CloneTreeNode(IUniversalTreeNode original)
        {
            if (original is UniversalTreeNode universalNode)
            {
                var cloned = new UniversalTreeNode(universalNode.Data)
                {
                    Actions = new List<UniversalTreeNodeAction>(universalNode.Actions)
                };
                return cloned;
            }
            
            // Fallback: create a new UniversalTreeNode with the original as data
            return new UniversalTreeNode(original)
            {
                Actions = new List<UniversalTreeNodeAction>(original.Actions)
            };
        }
    }
}

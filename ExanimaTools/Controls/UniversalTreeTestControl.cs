using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Markup.Xaml;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System;

namespace ExanimaTools.Controls
{
    public partial class UniversalTreeTestControl : UserControl
    {
        public ObservableCollection<UniversalTreeNode> TestTreeItems { get; } = new();
        public object? SelectedTestItem { get; set; }
        public IDataTemplate TestItemTemplate { get; }
        public Func<object, bool>? TestFilter { get; set; }

        public UniversalTreeTestControl()
        {
            AvaloniaXamlLoader.Load(this);
            // Sample data
            var root1 = new UniversalTreeNode("Root 1");
            var child1 = new UniversalTreeNode("Child 1.1");
            var child2 = new UniversalTreeNode("Child 1.2");
            root1.Children.Add(child1);
            root1.Children.Add(child2);
            var root2 = new UniversalTreeNode("Root 2");
            var child3 = new UniversalTreeNode("Child 2.1");
            root2.Children.Add(child3);
            TestTreeItems.Add(root1);
            TestTreeItems.Add(root2);
            // Simple item template
            TestItemTemplate = new FuncDataTemplate<object>((item, _) =>
                new TextBlock { Text = item is UniversalTreeNode node ? node.Data?.ToString() : item?.ToString() });
            // Example filter: show only nodes containing '1'
            TestFilter = obj =>
            {
                if (obj is UniversalTreeNode node && node.Data is string s)
                    return s.Contains("1");
                return true;
            };
            DataContext = this;
        }
    }
}

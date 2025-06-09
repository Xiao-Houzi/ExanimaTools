using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Markup.Xaml;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System;

namespace ExanimaTools.Controls
{
    public partial class UniversalTreeControl : UserControl
    {
        public static readonly AvaloniaProperty<object?> TreeItemsProperty = AvaloniaProperty.Register<UniversalTreeControl, object?>(nameof(TreeItems));

        public static readonly AvaloniaProperty<object?> SelectedItemProperty = AvaloniaProperty.Register<UniversalTreeControl, object?>(nameof(SelectedItem));

        public static readonly AvaloniaProperty<IDataTemplate?> ItemTemplateProperty = AvaloniaProperty.Register<UniversalTreeControl, IDataTemplate?>(nameof(ItemTemplate));

        public static readonly AvaloniaProperty<string?> FilterProperty = AvaloniaProperty.Register<UniversalTreeControl, string?>(nameof(Filter));

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

        public UniversalTreeControl()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using ExanimaTools.Controls;

namespace ExanimaTools.Controls
{
    // Generic builder for hierarchical tree structures
    public class UniversalTreeBuilder<T> : IUniversalTreeBuilder<T>
    {
        private Func<T, IEnumerable<T>>? _childrenSelector;
        private Func<T, object>? _viewModelSelector;
        private Func<T, bool>? _filter;
        private readonly Func<T, IEnumerable<UniversalTreeNodeAction>>? _actionsSelector;
        private readonly Func<T, string> _displayNameSelector;

        public UniversalTreeBuilder<T> WithChildren(Func<T, IEnumerable<T>> childrenSelector)
        {
            _childrenSelector = childrenSelector;
            return this;
        }

        public UniversalTreeBuilder<T> WithViewModel(Func<T, object> viewModelSelector)
        {
            _viewModelSelector = viewModelSelector;
            return this;
        }

        public UniversalTreeBuilder<T> WithFilter(Func<T, bool> filter)
        {
            _filter = filter;
            return this;
        }

        public UniversalTreeBuilder(
            Func<T, string> displayNameSelector,
            Func<T, IEnumerable<T>> childrenSelector,
            Func<T, IEnumerable<UniversalTreeNodeAction>>? actionsSelector = null)
        {
            _displayNameSelector = displayNameSelector;
            _childrenSelector = childrenSelector;
            _actionsSelector = actionsSelector;
        }

        public IEnumerable<object> Build(IEnumerable<T> source)
        {
            if (_childrenSelector == null || _viewModelSelector == null)
                throw new InvalidOperationException("Builder requires children and view model selectors.");
            return BuildRecursive(source);
        }

        public IEnumerable<IUniversalTreeNode> BuildTree(IEnumerable<T> source)
        {
            if (_childrenSelector == null || _viewModelSelector == null)
                throw new InvalidOperationException("Builder requires children and view model selectors.");
            return BuildRecursive(source).ToList();
        }

        private IEnumerable<IUniversalTreeNode> BuildRecursive(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                if (item == null) continue;
                if (_filter != null && !_filter(item))
                    continue;
                var vm = _viewModelSelector!(item);
                var node = new UniversalTreeNode(item)
                {
                    Actions = _actionsSelector?.Invoke(item)?.ToList() ?? new List<UniversalTreeNodeAction>()
                };
                if (_childrenSelector != null)
                {
                    var children = _childrenSelector(item);
                    if (children != null && children.Any())
                    {
                        var childItems = BuildRecursive(children);
                        node.SetChildren(childItems);
                    }
                }
                yield return node;
            }
        }
    }

    public interface IUniversalTreeBuilder<T>
    {
        IEnumerable<IUniversalTreeNode> BuildTree(IEnumerable<T> source);
    }
}

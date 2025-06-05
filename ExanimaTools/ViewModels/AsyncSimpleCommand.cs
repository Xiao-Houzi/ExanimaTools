using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ExanimaTools.ViewModels
{
    public class AsyncSimpleCommand : ICommand
    {
        private readonly Func<object?, Task> _execute;
        private readonly Func<bool>? _canExecute;
        private bool _isExecuting;
        public event EventHandler? CanExecuteChanged;

        public AsyncSimpleCommand(Func<object?, Task> execute, Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => !_isExecuting && (_canExecute?.Invoke() ?? true);
        public async void Execute(object? parameter)
        {
            if (!CanExecute(parameter)) return;
            _isExecuting = true;
            RaiseCanExecuteChanged();
            try { await _execute(parameter); }
            finally { _isExecuting = false; RaiseCanExecuteChanged(); }
        }
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}

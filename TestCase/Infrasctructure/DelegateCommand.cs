using System.Windows.Input;
using static System.Windows.Input.CommandManager;

namespace TestCase.Infrasctructure;

public class DelegateCommand(Action<object> execute, Func<object, bool>? canExecute = null)
    : ICommand
{
    public event EventHandler CanExecuteChanged
    {
        add => RequerySuggested += value;
        remove => RequerySuggested -= value;
    }

    public bool CanExecute(object parameter)
    {
        return canExecute == null || canExecute(parameter);
    }

    public void Execute(object parameter)
    {
        execute(parameter);
    }
}
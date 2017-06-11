using System;
using System.Windows.Input;

namespace LibraryWPF.ViewModels
{
    //This class has to be credited to Rachel Lim. The code that is used and modified here can be found on here blog: https://rachel53461.wordpress.com/2011/05/08/simplemvvmexample/. Link has been checked on 19.06.2016.

    /// <summary>
    /// Intermediate class between command events and executions.
    /// Purpose of this class is to let ViewModels to declare commands based on their needs. For example: changeView() after button is pressed.
    /// This approach also takes away necessity of "code behind" design in Views. That means ExampleView.xaml.cs doesn't need to have some buttonClickedEvent. 
    /// </summary>
    public class DelegateCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        private readonly Action<object> _executionAction;
        private readonly Predicate<object> _canExecutePredicate;
        public DelegateCommand(Action<object> execute) : this(execute, null)
        {

        }

        public DelegateCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }
            _executionAction = execute;
            _canExecutePredicate = canExecute;
        }
        public bool CanExecute(object parameter)
        {
            //return false; //If false is returned then command cannot be executed. In addition if command is assigned to button, then button is grayed out and cannot be pressed.
            if (_canExecutePredicate == null)
            {
                return true;
            }
            else
            {
                return _canExecutePredicate(parameter);
            }
            //return this.canExecutePredicate == null ? true : this.canExecutePredicate(parameter);            
        }

        public void Execute(object parameter)
        {
            if (!CanExecute(parameter))
            {
                throw new InvalidOperationException("The Command is not valid for execution, check the CanExecute method before attempting to execute.");
            }
            _executionAction(parameter);
        }
    }
}

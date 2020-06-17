using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace PrinterCenter
{
    public class RelayCommand : ICommand
    {
        public RelayCommand(Action execute)
            : this(execute, null)
        {
        }

        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
        }

   
        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute();
        }
        public event EventHandler CanExecuteChanged
        {   //这里把实现注释掉了，这样在SL下面也可以用。
            add { }
            remove { }
            //add
            //{
            //    if (_canExecute != null)
            //        CommandManager.RequerySuggested += value;
            //}
            //remove
            //{
            //    if (_canExecute != null)
            //        CommandManager.RequerySuggested -= value;
            //}
        }

        public void Execute(object parameter)
        {
            _execute();
        }

        readonly Action _execute;
        readonly Func<bool> _canExecute;
    }
}

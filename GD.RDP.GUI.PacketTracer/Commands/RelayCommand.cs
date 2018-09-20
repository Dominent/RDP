using System;
using System.Windows.Input;

namespace GD.RDP.GUI.PacketTracer.Commands
{
    public delegate void ExecuteDelegate(object parameter);
    public delegate bool CanExecuteDelegate(object parameter);

    public class RelayCommand : ICommand
    {
        private ExecuteDelegate _execute;
        private CanExecuteDelegate _canExecute;

        public RelayCommand(ExecuteDelegate execute)
            :this(execute, null)
        {
            this._execute = execute;
        }

        public RelayCommand(ExecuteDelegate execute, CanExecuteDelegate canExecute)
        {
            this._execute = execute;
            this._canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            if(this._canExecute == null) {
                return true;
            }

            return this._canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            this._execute(parameter);
        }
    }
}

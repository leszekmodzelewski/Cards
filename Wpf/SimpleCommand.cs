using System;
using System.Windows.Input;

namespace GeoLib.Wpf
{
    public class SimpleCommand : ICommand
    {

        private readonly Action execute;

        public SimpleCommand(Action execute)
        {
            this.execute = execute;

        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            this.execute();
        }

        public event EventHandler CanExecuteChanged;
    }
}
using System;
using System.Windows;
using System.Windows.Input;

namespace DesktopPanelTool.Commands.ControlCommands
{
    public class CloseWindowCommand
        : ICommand
    {
        public bool CanExecute(object parameter)
        {
            var p = GetParameter(parameter);
            return p != null;
        }

        Window GetParameter(object parameter)
        {
            return parameter as Window;
        }

        public void Execute(object parameter)
        {
            var p = GetParameter(parameter);
            p.Close();
        }

        public virtual event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}

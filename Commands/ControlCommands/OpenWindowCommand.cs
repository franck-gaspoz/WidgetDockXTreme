using System;
using System.Windows;
using System.Windows.Input;

namespace DesktopPanelTool.Commands.ControlCommands
{

    public class OpenWindowCommand
        : ICommand
    {
        public bool CanExecute(object parameter)
        {
            var p = GetParameter(parameter);
            return p != null;
        }

        Type GetParameter(object parameter)
        {
            return parameter as Type;
        }

        public void Execute(object parameter)
        {
            var p = GetParameter(parameter);
            var win = (Window)Activator.CreateInstance(p);
            win.Show();
        }

        public virtual event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}

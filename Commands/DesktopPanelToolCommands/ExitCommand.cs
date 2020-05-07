using System;
using System.Windows;
using System.Windows.Input;

namespace DesktopPanelTool.Commands.DesktopPanelToolCommands
{

    public class ExitCommand
        : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Application.Current.Shutdown();
        }

        public virtual event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}

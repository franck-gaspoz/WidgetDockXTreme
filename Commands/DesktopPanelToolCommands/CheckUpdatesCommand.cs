using System;
using System.Windows.Input;

namespace DesktopPanelTool.Commands.DesktopPanelToolCommands
{

    public class CheckUpdatesCommand
        : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return false;
        }

        public void Execute(object parameter)
        {
            
        }

        public virtual event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}

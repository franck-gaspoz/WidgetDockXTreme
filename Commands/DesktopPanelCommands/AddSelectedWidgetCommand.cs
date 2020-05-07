using System;
using System.Windows.Input;

namespace DesktopPanelTool.Commands.DesktopPanelCommands
{
    public class AddSelectedWidgetCommand
        : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
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

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace DesktopPanelTool.Commands.DesktopPanelToolCommands
{
    public class SettingsCommand
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

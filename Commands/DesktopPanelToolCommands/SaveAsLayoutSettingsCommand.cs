using DesktopPanelTool.Services;
using System;
using System.Windows.Input;

namespace DesktopPanelTool.Commands.DesktopPanelToolCommands
{
    public class SaveAsLayoutSettingsCommand
        : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            DesktopPanelToolService.SaveSettings();
        }

        public virtual event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}

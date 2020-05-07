using DesktopPanelTool.Services;
using System;
using System.Windows.Input;

namespace DesktopPanelTool.Commands.DesktopPanelToolCommands
{
    public class LoadLayoutSettingsCommand
        : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            DesktopPanelToolService.LoadLayoutSettings();
        }

        public virtual event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}

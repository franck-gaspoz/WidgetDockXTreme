using DesktopPanelTool.Services;
using System;
using System.Windows.Input;

namespace DesktopPanelTool.Commands.DesktopPanelToolCommands
{

    public class SaveSettingsCommand
        : ICommand
    {
        public bool CanExecute(object parameter)
        {
            var p = GetParameter(parameter);
            return p.HasValue;
        }

        bool? GetParameter(object parameter)
        {
            if (parameter is bool)
                return (bool)parameter;
            else
                return false;
        }

        public void Execute(object parameter)
        {
            var p = GetParameter(parameter).Value;
            DesktopPanelToolService.SaveSettings();
            if (p)
                NotificationBarService.Notify("settings have been saved");
        }

        public virtual event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}

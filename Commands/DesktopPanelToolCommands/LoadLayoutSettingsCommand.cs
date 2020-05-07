using DesktopPanelTool.Models;
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
            DesktopPanelToolService.LoadLayoutSettings();
            if (p && AppSettings.EnableNotifications)
                NotificationBarService.Notify("settings have been loaded");
        }

        public virtual event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}

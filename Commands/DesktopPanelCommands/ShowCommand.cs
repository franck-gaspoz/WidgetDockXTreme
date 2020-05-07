using DesktopPanelTool.Services;
using DesktopPanelTool.Views;
using System;
using System.Windows.Input;

namespace DesktopPanelTool.Commands.DesktopPanelCommands
{
    public class ShowCommand
        : ICommand
    {
        public bool CanExecute(object parameter)
        {
            var p = GetParameter(parameter);
            return p != null;
        }

        DesktopPanelBase GetParameter(object parameter)
        {
            return parameter as DesktopPanelBase;
        }

        public void Execute(object parameter)
        {
            var p = GetParameter(parameter);
            DesktopPanelToolService.ShowDockPanel(p);
        }

        public virtual event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}

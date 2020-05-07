using DesktopPanelTool.Lib;
using DesktopPanelTool.Services;
using System;
using System.Windows.Input;

namespace DesktopPanelTool.Commands.DesktopPanelToolCommands
{
    public class AddPanelCommand
        : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var panel = DesktopPanelToolService.AddDesktopPanel(null, DockName.None);
            panel.ViewModel.Title = $"dock {DesktopPanelToolService.DesktopPanelToolViewModel.PanelsViewModels.Count + DesktopPanelToolService.DesktopPanelToolViewModel.RecentPanelsViewModels.Count}";
            panel.Show();
        }

        public virtual event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}

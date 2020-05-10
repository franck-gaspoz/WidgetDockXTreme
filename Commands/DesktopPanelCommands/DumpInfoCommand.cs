using DesktopPanelTool.Views;
using System;
using System.Windows.Input;

namespace DesktopPanelTool.Commands.DesktopPanelCommands
{
    public class DumpInfoCommand
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
            System.Diagnostics.Debug.WriteLine($"{p.WidgetsPanel.DumpInfo()}");
        }

        public virtual event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}

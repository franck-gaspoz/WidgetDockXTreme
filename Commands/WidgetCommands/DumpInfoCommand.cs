using DesktopPanelTool.Controls;
using DesktopPanelTool.Views;
using System;
using System.Windows.Input;

namespace DesktopPanelTool.Commands.WidgetCommands
{
    public class DumpInfoCommand
        : ICommand
    {
        public bool CanExecute(object parameter)
        {
            var (x, y) = GetParameters(parameter);
            return x != null && y != null;
        }

        (DesktopPanelBase x, WidgetControl y) GetParameters(object parameter)
        {
            return (parameter != null && parameter is object[] ar && ar.Length == 2) ?
                    (ar[0] as DesktopPanelBase, ar[1] as WidgetControl)
                    : (null, null);
        }

        public void Execute(object parameter)
        {
            var (x, y) = GetParameters(parameter);

            System.Diagnostics.Debug.WriteLine($"{y.ViewModel.DumpInfo()}");
        }

        public virtual event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}

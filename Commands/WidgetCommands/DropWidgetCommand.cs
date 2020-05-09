using DesktopPanelTool.Controls;
using DesktopPanelTool.Models;
using DesktopPanelTool.Services;
using System;
using System.Windows.Input;

namespace DesktopPanelTool.Commands.WidgetCommands
{
    public class DropWidgetCommand
        : ICommand
    {
        public bool CanExecute(object parameter)
        {
            var p = GetParameter(parameter);
            return p != null && p.DataObject!=null && p.DataObject.GetDataPresent(typeof(WidgetControl)) && p.Target!=null;
        }

        DragData GetParameter(object parameter)
        {
            return parameter as DragData;
        }

        public void Execute(object parameter)
        {
            var p = GetParameter(parameter);
            DesktopPanelToolService.DropWidget((WidgetControl)p.DataObject.GetData(typeof(WidgetControl)), p.Target, p.DragEventArgs);
        }

        public virtual event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}

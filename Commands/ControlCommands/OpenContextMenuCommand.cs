using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace DesktopPanelTool.Commands.ControlCommands
{
    public class OpenContextMenuCommand
        : ICommand
    {
        public bool CanExecute(object parameter)
        {
            var (cm, fe) = GetParameters(parameter);
            return cm != null && fe != null;
        }

        (ContextMenu cm,FrameworkElement fe) GetParameters(object parameter)
        {
            return (parameter != null && parameter is object[] ar && ar.Length == 2) ?
                (ar[0] as ContextMenu, ar[1] as FrameworkElement)
                :(null, null);
        }

        public void Execute(object parameter)
        {
            var (cm, fe) = GetParameters(parameter);
            cm.PlacementTarget = fe;
            cm.Placement = PlacementMode.Relative;
            cm.HorizontalOffset = fe.ActualWidth / 2;
            cm.VerticalOffset = fe.ActualHeight / 2;
            cm.IsOpen = true;
        }

        public virtual event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}

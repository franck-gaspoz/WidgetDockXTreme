using DesktopPanelTool.Behaviors.WindowBehaviors;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace DesktopPanelTool.Commands.ControlCommands
{

    public class EnableClickOutsideHideOrCloseWindowBehaviorCommand
        : ICommand
    {
        public bool CanExecute(object parameter)
        {
            var p = GetParameter(parameter);
            return p != null;
        }

        Window GetParameter(object parameter)
        {
            return parameter as Window;
        }

        public void Execute(object parameter)
        {
            var p = GetParameter(parameter);
            ControlCommands.SetIsEnabledBehavior.Execute(new object[] { p, typeof(ClickOutsideHideOrCloseWindowBehavior), true });
        }

        public virtual event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}

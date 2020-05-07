using System;
using System.Diagnostics;
using System.Windows.Input;

namespace DesktopPanelTool.Commands.DesktopPanelToolCommands
{

    public class OpenWebPageCommand
        : ICommand
    {
        public bool CanExecute(object parameter)
        {
            var p = GetParameter(parameter);
            return p != null;
        }

        string GetParameter(object parameter)
        {
            return parameter as string;
        }

        public void Execute(object parameter)
        {
            var p = GetParameter(parameter);
            Process.Start("explorer.exe", p);
        }

        public virtual event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}

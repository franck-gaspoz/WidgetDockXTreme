using Microsoft.Xaml.Behaviors;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace DesktopPanelTool.Commands.ControlCommands
{
    public class SetIsEnabledBehaviorCommand
        : ICommand
    {
        public bool CanExecute(object parameter)
        {
            var (window, type, isEnabled) = GetParameters(parameter);
            return window != null && type != null;
        }

        (Window x, Type type, bool y) GetParameters(object parameter)
        {
            return (parameter != null && parameter is object[] ar && ar.Length == 3 && ar[2] is bool) ?
                    (ar[0] as Window, ar[1] as Type, (bool)ar[2])
                    : (null, null, false);
        }

        public void Execute(object parameter)
        {
            var (frameworkElement,type,isEnabled) = GetParameters(parameter);
            var o = Interaction.GetBehaviors(frameworkElement)
                .Where(x => x.GetType()==type)
                .FirstOrDefault();
            if (o != null)
            {
                var propInfo = type.GetProperty("IsEnabled");
                if (propInfo != null)
                {
                    propInfo.SetValue(o, isEnabled);
                    return;
                }
                var fieldInfo = type.GetField("IsEnabled");
                if (fieldInfo != null)
                    fieldInfo.SetValue(o, isEnabled);
            }
        }

        public virtual event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}

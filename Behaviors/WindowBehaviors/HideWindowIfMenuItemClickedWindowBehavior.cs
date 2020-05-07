using DesktopPanelTool.Lib;
using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DesktopPanelTool.Behaviors.WindowBehaviors
{
    public class HideWindowIfMenuItemClickedWindowBehavior
        : Behavior<Window>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewMouseDown += AssociatedObject_PreviewMouseDown;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewMouseDown -= AssociatedObject_PreviewMouseDown;
        }

        private void AssociatedObject_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var o = Mouse.DirectlyOver as DependencyObject;
            var menuItem = WPFUtil.FindParent<MenuItem>(o);
            if (menuItem!=null)
                AssociatedObject.Hide();
        }
    }
}

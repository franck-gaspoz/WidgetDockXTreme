using DesktopPanelTool.Lib;
using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;

namespace DesktopPanelTool.Behaviors.WindowBehaviors
{

    public class CloseAnySubmenuOnClosingWindowBehavior
        : Behavior<Window>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Closing += AssociatedObject_Closing;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Closing -= AssociatedObject_Closing;
        }

        private void AssociatedObject_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (AssociatedObject.IsVisible)
            {
                var menuItems = WPFHelper.FindChilds<MenuItem>(AssociatedObject, true);
                foreach (var menuItem in menuItems)
                    if (menuItem.IsSubmenuOpen)
                        menuItem.IsSubmenuOpen = false;
            }
        }
    }
}

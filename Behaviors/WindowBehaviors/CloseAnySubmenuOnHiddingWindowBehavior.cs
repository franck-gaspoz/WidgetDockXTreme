using DesktopPanelTool.Lib;
using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;

namespace DesktopPanelTool.Behaviors.WindowBehaviors
{

    public class CloseAnySubmenuOnHiddingWindowBehavior
        : Behavior<Window>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.IsVisibleChanged += AssociatedObject_IsVisibleChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.IsVisibleChanged -= AssociatedObject_IsVisibleChanged;
        }

        private void AssociatedObject_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {            
            if (!AssociatedObject.IsVisible)
            {
                var menuItems = WPFUtil.FindChilds<MenuItem>(AssociatedObject, true);
                foreach (var menuItem in menuItems)
                    if (menuItem.IsSubmenuOpen)
                        menuItem.IsSubmenuOpen = false;
            }
        }
    }
}

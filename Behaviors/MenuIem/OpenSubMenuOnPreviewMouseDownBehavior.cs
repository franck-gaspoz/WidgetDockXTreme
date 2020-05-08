using DesktopPanelTool.Lib;
using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DesktopPanelTool.Behaviors.MenuIem
{

    public class OpenSubMenuOnPreviewMouseDownBehavior
        : Behavior<MenuItem>
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
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var win = WPFHelper.FindLogicalParent<Window>(AssociatedObject);
                if (win != null)
                {
                    var menuItems = WPFHelper.FindChilds<MenuItem>(win, true);
                    foreach (var menuItem in menuItems)
                        if (menuItem.IsSubmenuOpen)
                        {
                            menuItem.IsSubmenuOpen = false;
                            var prop = typeof(MenuItem).GetProperty("IsHighlighted");
                            prop.SetValue(menuItem, false);
                        }
                }
                AssociatedObject.IsSubmenuOpen = true;
            }
        }
    }
}

using DesktopPanelTool.Lib;
using Microsoft.Xaml.Behaviors;
using System.Windows.Controls;
using System.Windows.Input;

namespace DesktopPanelTool.Behaviors.MenuIem
{

    public class CloseSelfSubMenuOnPreviewMouseDownBehaviorBehavior
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
                var parent = WPFUtil.FindLogicalAncestor(AssociatedObject);
                if (parent!=null && parent is MenuItem mi)
                    mi.IsSubmenuOpen = false;
            }
        }
    }
}

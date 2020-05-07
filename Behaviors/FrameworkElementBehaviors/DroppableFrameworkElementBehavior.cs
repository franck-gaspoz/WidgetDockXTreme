﻿#define dbg

using Microsoft.Xaml.Behaviors;
using System.Windows;

namespace DesktopPanelTool.Behaviors.FrameworkElementBehaviors
{

    public class DroppableFrameworkElementBehavior
        : Behavior<FrameworkElement>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.AllowDrop = true;
            AssociatedObject.Drop += AssociatedObject_Drop;
            AssociatedObject.MouseEnter += AssociatedObject_MouseEnter;
        }

        private void AssociatedObject_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
#if dbg
            DesktopPanelTool.Lib.Debug.WriteLine($"enter");
#endif
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.AllowDrop = false;
            AssociatedObject.Drop -= AssociatedObject_Drop;
        }

        private void AssociatedObject_Drop(object sender, DragEventArgs e)
        {
#if dbg
                DesktopPanelTool.Lib.Debug.WriteLine($"dropping");
#endif
        }
    }
}

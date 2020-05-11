//#define dbg

using Microsoft.Xaml.Behaviors;
using System.Windows;

namespace DesktopPanelTool.Behaviors.WindowBehaviors
{
    public class ActivateOnDragOverWindowBehavior
        : Behavior<Window>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.DragOver += AssociatedObject_DragOver;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.DragOver -= AssociatedObject_DragOver;
        }

        private void AssociatedObject_DragOver(object sender, DragEventArgs e)
        {
#if alldbg || dbg
            DesktopPanelTool.Lib.Debug.WriteLine($"drag over");
#endif
            if (!AssociatedObject.IsActive)
                AssociatedObject.Activate();
        }

    }
}

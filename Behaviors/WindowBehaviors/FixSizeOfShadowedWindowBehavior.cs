using Microsoft.Xaml.Behaviors;
using System.Windows;

namespace DesktopPanelTool.Behaviors.WindowBehaviors
{

    public class FixSizeOfShadowedWindowBehavior
        : Behavior<Window>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            var areaSize = (Thickness)AssociatedObject.FindResource("WindowShadowAreaSize");
            AssociatedObject.Width += areaSize.Left*2d;
            AssociatedObject.Height += areaSize.Left*2d;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
        }
    }
}

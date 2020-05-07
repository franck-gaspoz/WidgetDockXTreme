using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;

namespace DesktopPanelTool.Behaviors.PanelBehaviors
{
    public class DesktopPanelOrientableStackPanelBehavior
        : Behavior<StackPanel>
    {
        Window Window;

        public Orientation? CurrentOrientation { get; protected set; }

        protected override void OnAttached()
        {
            base.OnAttached();
            Window = Window.GetWindow(AssociatedObject);
            Window.SizeChanged += Window_SizeChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.SizeChanged -= Window_SizeChanged;
            Window = null;
        }

        private void Window_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            if (AssociatedObject.Parent != null)
            {
                var newOrientation = Window.Width >= Window.Height ? Orientation.Horizontal : Orientation.Vertical;

                if (!CurrentOrientation.HasValue || CurrentOrientation.Value != newOrientation)
                {
                    CurrentOrientation = newOrientation;
                    ApplyPanelOrientation();
                }
            }
        }

        void ApplyPanelOrientation()
        {
            if (CurrentOrientation.HasValue)
            {
                var orientation = CurrentOrientation.Value;
                AssociatedObject.Orientation = CurrentOrientation.Value;
            }
        }

    }
}

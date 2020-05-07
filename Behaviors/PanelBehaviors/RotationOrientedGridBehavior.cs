using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DesktopPanelTool.Behaviors.PanelBehaviors
{
    public class RotationOrientedGridBehavior
        : Behavior<Grid>
    {
        Window Window;
        RotateTransform RotateTransform;

        public Orientation? CurrentOrientation { get; protected set; }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            AssociatedObject.Loaded -= AssociatedObject_Loaded;
            Window = Window.GetWindow(AssociatedObject);
            Window.SizeChanged += Window_SizeChanged;
            Window_SizeChanged(this, null);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.SizeChanged -= Window_SizeChanged;
            Window = null;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
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
                if (RotateTransform == null)
                {
                    RotateTransform = new RotateTransform();
                    AssociatedObject.LayoutTransform = RotateTransform;
                }
                RotateTransform.Angle = orientation == Orientation.Horizontal?
                    -90:0;
                AssociatedObject.VerticalAlignment = orientation == Orientation.Horizontal ?
                    VerticalAlignment.Stretch : VerticalAlignment.Top;
                AssociatedObject.HorizontalAlignment = orientation == Orientation.Horizontal ?
                    HorizontalAlignment.Right : HorizontalAlignment.Stretch;
            }
        }

    }
}

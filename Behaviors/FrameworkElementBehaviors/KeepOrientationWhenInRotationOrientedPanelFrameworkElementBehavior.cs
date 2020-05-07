using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DesktopPanelTool.Behaviors.FrameworkElementBehaviors
{
    public class KeepOrientationWhenInRotationOrientedPanelFrameworkElementBehavior
        : Behavior<FrameworkElement>
    {
        Window Window;
        RotateTransform RotateTransform;

        public bool IsEnabled
        {
            get { return (bool)GetValue(IsEnabledProperty); }
            set { SetValue(IsEnabledProperty, value); }
        }

        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.Register("IsEnabled", typeof(bool), typeof(KeepOrientationWhenInRotationOrientedPanelFrameworkElementBehavior), new PropertyMetadata(false));

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
            if (AssociatedObject.Parent != null && IsEnabled)
            {
                var newOrientation = Window.Width >= Window.Height ? Orientation.Horizontal : Orientation.Vertical;

                if (!CurrentOrientation.HasValue || CurrentOrientation.Value != newOrientation)
                {
                    CurrentOrientation = newOrientation;
                    ApplyRevertPanelOrientation();
                }
            }
        }

        void ApplyRevertPanelOrientation()
        {
            if (CurrentOrientation.HasValue)
            {
                var orientation = CurrentOrientation.Value;
                if (RotateTransform == null)
                {
                    RotateTransform = new RotateTransform();
                    AssociatedObject.LayoutTransform = RotateTransform;
                }
                RotateTransform.Angle = orientation == Orientation.Horizontal ?
                    90 : 0;
            }
        }

    }
}


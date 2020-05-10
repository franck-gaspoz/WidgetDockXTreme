using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;

namespace DesktopPanelTool.Behaviors.PanelBehaviors
{
    public class WidgetsGridPanelBehavior
        : Behavior<Grid>
    {
        Window Window;

        public Orientation? CurrentOrientation { get; protected set; }

        public FrameworkElement RelatedElement
        {
            get { return (FrameworkElement)GetValue(RelatedElementProperty); }
            set { SetValue(RelatedElementProperty, value); }
        }

        public static readonly DependencyProperty RelatedElementProperty =
            DependencyProperty.Register("RelatedElement", typeof(FrameworkElement), typeof(WidgetsGridPanelBehavior), new PropertyMetadata(null));

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
            if (CurrentOrientation.HasValue && RelatedElement!=null)
            {
                var orientation = CurrentOrientation.Value;
                AssociatedObject.Margin =
                    orientation == Orientation.Horizontal ?
                        new Thickness( RelatedElement.ActualHeight, 0, 0, 0)
                        : new Thickness(0,RelatedElement.ActualWidth, 0, 0);                
            }
        }

    }
}

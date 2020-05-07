using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;

namespace DesktopPanelTool.Behaviors.PanelBehaviors
{
    public class WidgetsStackPanelBehavior
        : Behavior<StackPanel>
    {
        Window Window;

        public Orientation? CurrentOrientation { get; protected set; }

        public double WidgetSpacing
        {
            get { return (double)GetValue(WidgetSpacingProperty); }
            set { SetValue(WidgetSpacingProperty, value); }
        }
       
        public static readonly DependencyProperty WidgetSpacingProperty =
            DependencyProperty.Register("WidgetSpacing", typeof(double), typeof(WidgetsStackPanelBehavior), new PropertyMetadata(0d));

        public double PanelPadding
        {
            get { return (double)GetValue(PanelPaddingProperty); }
            set { SetValue(PanelPaddingProperty, value); }
        }

        public static readonly DependencyProperty PanelPaddingProperty =
            DependencyProperty.Register("PanelPadding", typeof(double), typeof(WidgetsStackPanelBehavior), new PropertyMetadata(8d));

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
                AssociatedObject.Margin =
                    orientation == Orientation.Horizontal ?
                        new Thickness(0, PanelPadding, PanelPadding, PanelPadding)
                        : new Thickness(PanelPadding, 0, PanelPadding, PanelPadding);
                foreach (var child in AssociatedObject.Children)
                {
                    if (child is FrameworkElement widget)
                    {
                        if (orientation == Orientation.Horizontal)
                            widget.Margin = new Thickness(WidgetSpacing/2d, 0, WidgetSpacing, 0);
                        else
                            widget.Margin = new Thickness(0, WidgetSpacing/2d, 0, WidgetSpacing);
                    }
                }
            }
        }

    }
}

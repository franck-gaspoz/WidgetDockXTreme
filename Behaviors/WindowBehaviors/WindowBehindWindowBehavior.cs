using Microsoft.Xaml.Behaviors;
using System;
using System.Windows;

namespace DesktopPanelTool.Behaviors.WindowBehaviors
{
    public class WindowBehindWindowBehavior
        : Behavior<Window>
    {
        public Window ShadowLayer { get; protected set; }
        double _dx = 17;
        double _dy = 17;

        public Type LayerWindowType
        {
            get { return (Type)GetValue(LayerWindowTypeProperty); }
            set { SetValue(LayerWindowTypeProperty, value); }
        }

        public static readonly DependencyProperty LayerWindowTypeProperty =
            DependencyProperty.Register("LayerWindowType", typeof(Type), typeof(WindowBehindWindowBehavior), new PropertyMetadata(null));

        public bool IsEnabled
        {
            get { return (bool)GetValue(IsEnabledProperty); }
            set { SetValue(IsEnabledProperty, value); }
        }

        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.Register("IsEnabled", typeof(bool), typeof(WindowBehindWindowBehavior), new PropertyMetadata(true));

        protected override void OnAttached()
        {
            base.OnAttached();
            ShadowLayer = (Window)Activator.CreateInstance(LayerWindowType);
            AssociatedObject.SizeChanged += AssociatedObject_SizeChanged;
            AssociatedObject.LocationChanged += AssociatedObject_LocationChanged;
            AssociatedObject.IsVisibleChanged += AssociatedObject_IsVisibleChanged;
            AssociatedObject.Activated += AssociatedObject_Activated;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.SizeChanged -= AssociatedObject_SizeChanged;
            AssociatedObject.LocationChanged -= AssociatedObject_LocationChanged;
            AssociatedObject.IsVisibleChanged -= AssociatedObject_IsVisibleChanged;
            AssociatedObject.Activated -= AssociatedObject_Activated;
        }

        private void AssociatedObject_Activated(object sender, EventArgs e)
        {
            SetZOrder();
        }

        private void AssociatedObject_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!IsEnabled) return;
            if (!AssociatedObject.IsVisible)
            {
                ShadowLayer.Hide();
            }
            else
            {
                SetZOrder();
                ShadowLayer.Show();
            }
        }

        void SetZOrder()
        {
            ShadowLayer.Topmost = true;
            AssociatedObject.Topmost = true;
        }

        private void AssociatedObject_LocationChanged(object sender, EventArgs e)
        {
            if (!IsEnabled) return;
            ShadowLayer.Left = AssociatedObject.Left-_dx;
            ShadowLayer.Top = AssociatedObject.Top-_dy;
        }

        private void AssociatedObject_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!IsEnabled) return;
            ShadowLayer.Width = AssociatedObject.ActualWidth + 2d*_dx;
            ShadowLayer.Height = AssociatedObject.ActualHeight + 2d*_dy;
        }
    }
}

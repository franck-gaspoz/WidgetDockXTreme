using DesktopPanelTool.Views;
using Microsoft.Xaml.Behaviors;
using System;
using System.Windows;

namespace DesktopPanelTool.Behaviors.WindowBehaviors
{
    public class ShadowLayerBehindBehavior
        : Behavior<Window>
    {
        WindowShowLayer _shadowLayer;
        double _dx = 17;
        double _dy = 17;

        public bool IsEnabled
        {
            get { return (bool)GetValue(IsEnabledProperty); }
            set { SetValue(IsEnabledProperty, value); }
        }

        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.Register("IsEnabled", typeof(bool), typeof(ShadowLayerBehindBehavior), new PropertyMetadata(false));

        protected override void OnAttached()
        {
            base.OnAttached();
            _shadowLayer = new WindowShowLayer();
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
                _shadowLayer.Hide();
            }
            else
            {
                SetZOrder();
                _shadowLayer.Show();
            }
        }

        void SetZOrder()
        {
            _shadowLayer.Topmost = true;
            AssociatedObject.Topmost = true;
        }

        private void AssociatedObject_LocationChanged(object sender, EventArgs e)
        {
            if (!IsEnabled) return;
            _shadowLayer.Left = AssociatedObject.Left-_dx;
            _shadowLayer.Top = AssociatedObject.Top-_dy;
        }

        private void AssociatedObject_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!IsEnabled) return;
            _shadowLayer.Width = AssociatedObject.ActualWidth + 2d*_dx;
            _shadowLayer.Height = AssociatedObject.ActualHeight + 2d*_dy;
        }
    }
}

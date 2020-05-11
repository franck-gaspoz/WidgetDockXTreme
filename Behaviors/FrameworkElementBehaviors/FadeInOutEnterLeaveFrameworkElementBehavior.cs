#define dbg

using Microsoft.Xaml.Behaviors;
using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace DesktopPanelTool.Behaviors.FrameworkElementBehaviors
{
    public class FadeInOutEnterLeaveFrameworkElementBehavior
        : Behavior<FrameworkElement>
    {
        public double TransparencyChangeDuration
        {
            get { return (double)GetValue(TransparencyChangeDurationProperty); }
            set { SetValue(TransparencyChangeDurationProperty, value); }
        }

        public static readonly DependencyProperty TransparencyChangeDurationProperty =
            DependencyProperty.Register("TransparencyChangeDuration", typeof(double), typeof(FadeInOutEnterLeaveFrameworkElementBehavior), new PropertyMetadata(500d));

        public double MouseOverTransparency
        {
            get { return (double)GetValue(MouseOverTransparencyProperty); }
            set { SetValue(MouseOverTransparencyProperty, value); }
        }

        public static readonly DependencyProperty MouseOverTransparencyProperty =
            DependencyProperty.Register("MouseOverTransparency", typeof(double), typeof(FadeInOutEnterLeaveFrameworkElementBehavior), new PropertyMetadata(1d));

        public double MouseOutTransparency
        {
            get { return (double)GetValue(MouseOutTransparencyProperty); }
            set { SetValue(MouseOutTransparencyProperty, value); }
        }

        public static readonly DependencyProperty MouseOutTransparencyProperty =
            DependencyProperty.Register("MouseOutTransparency", typeof(double), typeof(FadeInOutEnterLeaveFrameworkElementBehavior), new PropertyMetadata(null));

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.MouseEnter += AssociatedObject_MouseEnter;
            AssociatedObject.MouseLeave += AssociatedObject_MouseLeave;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.MouseEnter += AssociatedObject_MouseEnter;
            AssociatedObject.MouseLeave += AssociatedObject_MouseLeave;
        }

        private void AssociatedObject_MouseLeave(object sender, EventArgs e)
        {
#if dbg
            DesktopPanelTool.Lib.Debug.WriteLine($"deactivated");
#endif
            var an = new DoubleAnimation(MouseOverTransparency, MouseOutTransparency, new Duration(TimeSpan.FromMilliseconds(TransparencyChangeDuration)));
            AssociatedObject.BeginAnimation(FrameworkElement.OpacityProperty, an);
        }

        private void AssociatedObject_MouseEnter(object sender, EventArgs e)
        {
#if dbg
            DesktopPanelTool.Lib.Debug.WriteLine($"activated");
#endif
            var an = new DoubleAnimation(MouseOutTransparency, MouseOverTransparency, new Duration(TimeSpan.FromMilliseconds(TransparencyChangeDuration)));
            AssociatedObject.BeginAnimation(FrameworkElement.OpacityProperty, an);
        }


    }
}

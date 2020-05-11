//#define dbg

using Microsoft.Xaml.Behaviors;
using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace DesktopPanelTool.Behaviors.WindowBehaviors
{
    public class ActivationBindedTransparencyWindowBehavior
        : Behavior<Window>
    {
        public double TransparencyChangeDuration
        {
            get { return (double)GetValue(TransparencyChangeDurationProperty); }
            set { SetValue(TransparencyChangeDurationProperty, value); }
        }

        public static readonly DependencyProperty TransparencyChangeDurationProperty =
            DependencyProperty.Register("TransparencyChangeDuration", typeof(double), typeof(ActivationBindedTransparencyWindowBehavior), new PropertyMetadata(500d));

        public double ActivatedTransparency
        {
            get { return (double)GetValue(ActivatedTransparencyProperty); }
            set { SetValue(ActivatedTransparencyProperty, value); }
        }

        public static readonly DependencyProperty ActivatedTransparencyProperty =
            DependencyProperty.Register("ActivatedTransparency", typeof(double), typeof(ActivationBindedTransparencyWindowBehavior), new PropertyMetadata(1d));

        public double DeactivatedTransparency
        {
            get { return (double)GetValue(DeactivatedTransparencyProperty); }
            set { SetValue(DeactivatedTransparencyProperty, value); }
        }

        public static readonly DependencyProperty DeactivatedTransparencyProperty =
            DependencyProperty.Register("DeactivatedTransparency", typeof(double), typeof(ActivationBindedTransparencyWindowBehavior), new PropertyMetadata(0.5));

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Activated += AssociatedObject_Activated;
            AssociatedObject.Deactivated += AssociatedObject_Deactivated;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Activated -= AssociatedObject_Activated;
            AssociatedObject.Deactivated -= AssociatedObject_Deactivated;
        }

        private void AssociatedObject_Deactivated(object sender, EventArgs e)
        {
#if alldbg || dbg
            DesktopPanelTool.Lib.Debug.WriteLine($"deactivated");
#endif
            var an = new DoubleAnimation(ActivatedTransparency, DeactivatedTransparency, new Duration(TimeSpan.FromMilliseconds(TransparencyChangeDuration)));
            AssociatedObject.BeginAnimation(FrameworkElement.OpacityProperty, an);
        }

        private void AssociatedObject_Activated(object sender, EventArgs e)
        {
#if alldbg || dbg
            DesktopPanelTool.Lib.Debug.WriteLine($"activated");
#endif
            var an = new DoubleAnimation(DeactivatedTransparency, ActivatedTransparency, new Duration(TimeSpan.FromMilliseconds(TransparencyChangeDuration)));
            AssociatedObject.BeginAnimation(FrameworkElement.OpacityProperty, an);
        }


    }
}

using Microsoft.Xaml.Behaviors;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace DesktopPanelTool.Behaviors.FrameworkElementBehaviors
{
    public class MouseOverOpacityChangeBehavior
        : Behavior<FrameworkElement>
    {
        public double MouseOverOpacity
        {
            get { return (double)GetValue(MouseOverOpacityProperty); }
            set { SetValue(MouseOverOpacityProperty, value); }
        }

        public static readonly DependencyProperty MouseOverOpacityProperty =
            DependencyProperty.Register("MouseOverOpacity", typeof(double), typeof(MouseOverOpacityChangeBehavior), new PropertyMetadata(1d));

        public double MouseOutOpacity
        {
            get { return (double)GetValue(MouseOutOpacityProperty); }
            set { SetValue(MouseOutOpacityProperty, value); }
        }

        public static readonly DependencyProperty MouseOutOpacityProperty =
            DependencyProperty.Register("MouseOutOpacity", typeof(double), typeof(MouseOverOpacityChangeBehavior), new PropertyMetadata(0.5d));

        public double OpacityTransitionAnimationDuration
        {
            get { return (double)GetValue(OpacityTransitionAnimationDurationProperty); }
            set { SetValue(OpacityTransitionAnimationDurationProperty, value); }
        }

        public static readonly DependencyProperty OpacityTransitionAnimationDurationProperty =
            DependencyProperty.Register("OpacityTransitionAnimationDuration", typeof(double), typeof(MouseOverOpacityChangeBehavior), new PropertyMetadata(1000d));

        public bool OpacityEffectOnClickEnabled
        {
            get { return (bool)GetValue(OpacityEffectOnClickEnabledProperty); }
            set { SetValue(OpacityEffectOnClickEnabledProperty, value); }
        }

        public static readonly DependencyProperty OpacityEffectOnClickEnabledProperty =
            DependencyProperty.Register("OpacityEffectOnClickEnabled", typeof(bool), typeof(MouseOverOpacityChangeBehavior), new PropertyMetadata(true));

        bool AvoidNextMouseLeaveTransition = false;

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.MouseEnter += AssociatedObject_MouseEnter;
            AssociatedObject.MouseLeave += AssociatedObject_MouseLeave;
            AssociatedObject.MouseDown += AssociatedObject_MouseDown;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.MouseEnter -= AssociatedObject_MouseEnter;
            AssociatedObject.MouseLeave -= AssociatedObject_MouseLeave;
            AssociatedObject.MouseDown -= AssociatedObject_MouseDown;
        }

        private void AssociatedObject_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (AvoidNextMouseLeaveTransition && !OpacityEffectOnClickEnabled)
                return;
            var an = new DoubleAnimation(
                MouseOverOpacity,
                MouseOutOpacity,
                new Duration(TimeSpan.FromMilliseconds(OpacityTransitionAnimationDuration)));
            AssociatedObject.BeginAnimation(FrameworkElement.OpacityProperty, an);
        }

        private void AssociatedObject_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var avoidNextMouseLeaveTransition = AvoidNextMouseLeaveTransition;
            AvoidNextMouseLeaveTransition = false;
            if (avoidNextMouseLeaveTransition && !OpacityEffectOnClickEnabled)
                return;
            var an = new DoubleAnimation(
                MouseOutOpacity,
                MouseOverOpacity,
                new Duration(TimeSpan.FromMilliseconds(OpacityTransitionAnimationDuration)));
            AssociatedObject.BeginAnimation(FrameworkElement.OpacityProperty, an);
        }

        private void AssociatedObject_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && !OpacityEffectOnClickEnabled)
                AvoidNextMouseLeaveTransition = true;
        }

    }
}

//#define dbg

using Microsoft.Xaml.Behaviors;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace DesktopPanelTool.Behaviors.FrameworkElementBehaviors
{
    public class FadeInOutDragOverOutFrameworkElementBehavior
        : Behavior<FrameworkElement>
    {
        bool _isOver = false;

        public double TransparencyChangeDuration
        {
            get { return (double)GetValue(TransparencyChangeDurationProperty); }
            set { SetValue(TransparencyChangeDurationProperty, value); }
        }

        public static readonly DependencyProperty TransparencyChangeDurationProperty =
            DependencyProperty.Register("TransparencyChangeDuration", typeof(double), typeof(FadeInOutDragOverOutFrameworkElementBehavior), new PropertyMetadata(500d));

        public double MouseOverTransparency
        {
            get { return (double)GetValue(MouseOverTransparencyProperty); }
            set { SetValue(MouseOverTransparencyProperty, value); }
        }

        public static readonly DependencyProperty MouseOverTransparencyProperty =
            DependencyProperty.Register("MouseOverTransparency", typeof(double), typeof(FadeInOutDragOverOutFrameworkElementBehavior), new PropertyMetadata(1d));

        public double MouseOutTransparency
        {
            get { return (double)GetValue(MouseOutTransparencyProperty); }
            set { SetValue(MouseOutTransparencyProperty, value); }
        }

        public static readonly DependencyProperty MouseOutTransparencyProperty =
            DependencyProperty.Register("MouseOutTransparency", typeof(double), typeof(FadeInOutDragOverOutFrameworkElementBehavior), new PropertyMetadata(0d));

        public FrameworkElement Target
        {
            get { return (FrameworkElement)GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }

        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.Register("Target", typeof(FrameworkElement), typeof(FadeInOutDragOverOutFrameworkElementBehavior), new PropertyMetadata(null));

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.DragOver += AssociatedObject_DragOver; ;
            AssociatedObject.DragLeave += AssociatedObject_DragLeave;
            AssociatedObject.Drop += AssociatedObject_Drop;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.DragOver -= AssociatedObject_DragOver; ;
            AssociatedObject.DragLeave -= AssociatedObject_DragLeave;
            AssociatedObject.Drop -= AssociatedObject_Drop;
        }

        private void AssociatedObject_DragLeave(object sender, DragEventArgs e)
        {
            _isOver = false;
            if (Target == null) return;
#if dbg
            var mo = Mouse.DirectlyOver;
            DesktopPanelTool.Lib.Debug.WriteLine($"drag leave - mouse over: {mo?.GetType().Name}");
#endif
            var an = new DoubleAnimation(
                MouseOverTransparency, 
                MouseOutTransparency, 
                new Duration(TimeSpan.FromMilliseconds(TransparencyChangeDuration)));
            Target.BeginAnimation(FrameworkElement.OpacityProperty, an);
        }

        private void AssociatedObject_DragOver(object sender, DragEventArgs e)
        {
            if (Target == null || _isOver) return;
#if dbg
            var mo = Mouse.DirectlyOver;
            DesktopPanelTool.Lib.Debug.WriteLine($"drag over - mouse over: {mo?.GetType().Name}");
#endif
            _isOver = true;
            var an = new DoubleAnimation(Target.Opacity, MouseOverTransparency, new Duration(TimeSpan.FromMilliseconds(TransparencyChangeDuration)));
            Target.BeginAnimation(FrameworkElement.OpacityProperty, an);
        }

        private void AssociatedObject_Drop(object sender, DragEventArgs e)
        {
            if (!_isOver) return;
#if dbg
            var mo = Mouse.DirectlyOver;
            DesktopPanelTool.Lib.Debug.WriteLine($"drag drop - mouse over: {mo?.GetType().Name}");
#endif
            AssociatedObject_DragLeave(sender, null);
        }

    }
}

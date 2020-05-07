using Microsoft.Xaml.Behaviors;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Animation;

namespace DesktopPanelTool.Behaviors.WindowBehaviors
{
    public class FadeOutOnClosingBehavior
        : Behavior<Window>
    {
        public static int FadeOutAnimationDuration = 500;
        bool _isEnabled = true;

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Closing += AssociatedObject_Closing;
        }
        
        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Closing -= AssociatedObject_Closing;
        }

        private void AssociatedObject_Closing(object sender, CancelEventArgs e)
        {
            if (_isEnabled)
            {
                e.Cancel = true;
                var an = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromMilliseconds(FadeOutAnimationDuration)));
                an.Completed += (o, e) => { _isEnabled = false; AssociatedObject.Close(); };
                AssociatedObject.BeginAnimation(FrameworkElement.OpacityProperty, an);
            }
        }
    }
}

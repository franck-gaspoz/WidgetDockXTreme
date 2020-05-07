//#define dbg

using DesktopPanelTool.Lib;
using Microsoft.Xaml.Behaviors;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace DesktopPanelTool.Behaviors.FrameworkElementBehaviors
{
    public class AnimateGradientBackgroundFrameworkElementBehavior
        : Behavior<FrameworkElement>
    {
        Storyboard _storyboard;
        static int _cnt = 0;
        bool _alreadyActivated = false;
        static Random _random = new Random();
        static int _cntDelta = 0;
        static int _delta = 7000;

        public bool StartsImmediately
        {
            get { return (bool)GetValue(StartsImmediatelyProperty); }
            set { SetValue(StartsImmediatelyProperty, value); }
        }

        public static readonly DependencyProperty StartsImmediatelyProperty =
            DependencyProperty.Register("StartsImmediately", typeof(bool), typeof(AnimateGradientBackgroundFrameworkElementBehavior), new PropertyMetadata(false));

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.SizeChanged += AssociatedObject_SizeChanged;

#if dbg
            var win = WPFUtil.FindLogicalParent<Window>(AssociatedObject);
            DesktopPanelTool.Lib.Debug.WriteLine($"win={win} name={win?.Name}");
#endif
        }

        private void AssociatedObject_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!_alreadyActivated)
            {
                _alreadyActivated = true;
                StartAnimation();
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.SizeChanged -= AssociatedObject_SizeChanged;
        }

        void BuildAnimation()
        {
            var backgroundProperty = AssociatedObject.GetType().GetProperty("Background");
            if (backgroundProperty != null)
            {
                var backgroundValue = backgroundProperty.GetValue(AssociatedObject);
                if (backgroundValue != null && backgroundValue is GradientBrush gb)
                {
                    _storyboard = new Storyboard()
                    {
                        RepeatBehavior = RepeatBehavior.Forever
                    };
                    NameScope.SetNameScope(AssociatedObject, new NameScope());

                    int idx = 1;
                    
                    var opac = 0.3d;
                    var endOffset = 1d;
                    var fadeInTime = 5000;
                    var leftToRightTime = 20000;
                    var leftToRightAutoReverseMul = 1;
                    var fadeOutTime = 5000;
                    var waitTime = 10000;
                    var fadeInDur = new Duration(TimeSpan.FromMilliseconds(fadeInTime));
                    var leftToRightDur = new Duration(TimeSpan.FromMilliseconds(leftToRightTime));
                    var fadeOutDur = new Duration(TimeSpan.FromMilliseconds(fadeOutTime));
                    var waitDur = new Duration(TimeSpan.FromMilliseconds(waitTime));

                    var gs = gb.GradientStops[idx];
                    var gradientAn = $"sb{_cnt}";
                    var opacAn = $"sbt{_cnt}";
                    AssociatedObject.RegisterName(gradientAn, gs);
                    AssociatedObject.RegisterName(opacAn, AssociatedObject);

                    var fadeIn = new DoubleAnimation(0, opac, fadeInDur) { AutoReverse = false };
                    Storyboard.SetTargetName(fadeIn, opacAn);
                    Storyboard.SetTargetProperty(fadeIn, new PropertyPath(FrameworkElement.OpacityProperty));

                    var leftToRight = new DoubleAnimation(0d, endOffset, leftToRightDur) { 
                        AutoReverse = leftToRightAutoReverseMul==2,
                        //EasingFunction  = new BounceEase() { Bounces = 5, Bounciness=2.5, EasingMode=EasingMode.EaseInOut }
                        //EasingFunction = new BackEase() { Amplitude = 1 }
                    };
                    Storyboard.SetTargetName(leftToRight, gradientAn);
                    Storyboard.SetTargetProperty(leftToRight, new PropertyPath(GradientStop.OffsetProperty));

                    var fadeOut = new DoubleAnimation(opac, 0d, fadeOutDur) { AutoReverse = false };
                    Storyboard.SetTargetName(fadeOut, opacAn);
                    Storyboard.SetTargetProperty(fadeOut, new PropertyPath(FrameworkElement.OpacityProperty));

                    var wait = new DoubleAnimation(endOffset, endOffset, waitDur) { AutoReverse = false };
                    Storyboard.SetTargetName(wait, gradientAn);
                    Storyboard.SetTargetProperty(wait, new PropertyPath(GradientStop.OffsetProperty));


                    fadeOut.BeginTime = TimeSpan.FromMilliseconds(leftToRightTime* leftToRightAutoReverseMul);                    
                    wait.BeginTime = TimeSpan.FromMilliseconds(leftToRightTime* leftToRightAutoReverseMul + fadeOutTime);

                    _storyboard.Children.Add(fadeIn);
                    _storyboard.Children.Add(leftToRight);
                    _storyboard.Children.Add(fadeOut);
                    _storyboard.Children.Add(wait);
                    _cnt++;

                    idx++;                    
                }
            }
        }

        void StartAnimation()
        {
            if (_storyboard == null)
                BuildAnimation();
            
            if (StartsImmediately)
                _storyboard.Begin(AssociatedObject);
            else
            {
                var wait = _random.Next(100, 2000) + _cntDelta;
                _cntDelta += _delta;
                new Thread(() =>
                {
                    Thread.Sleep(wait);
                    _storyboard.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        _storyboard.Begin(AssociatedObject);
                    }
                    ));
                }).Start();
            }
        }
    }
}

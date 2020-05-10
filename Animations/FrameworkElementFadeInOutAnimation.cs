using DesktopPanelTool.Lib;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Animation;

namespace DesktopPanelTool.Animations
{
    public class FrameworkElementFadeInOutAnimation
        : IAnimations
    {
        public const string FadeInAnimationName = "FadeInAnimationName";
        public const string FadeOutAnimationName = "FadeOutAnimationName";
        public const string MouseInOpacityParameterName = "MouseInOpacityParameterName";
        public const string MouseOutOpacityParameterName = "MouseOutOpacityParameterName";

        public double _mouseInOpacity = 1d;
        public double _mouseOutOpacity = 0d;

        Storyboard _fadeInStoryboard;
        Storyboard _fadeOutStoryboard;

        bool _initialized = false;

        void Initialize(FrameworkElement target,object parameters)
        {
            if (_initialized) return;
            _initialized = true;
            NameScope.SetNameScope(target, new NameScope());
            if (parameters!=null && parameters is Dictionary<string,object> dic)
            {
                if (dic.TryGetValue(MouseInOpacityParameterName, out var p1))
                    if (p1 is double d)
                        _mouseInOpacity = d;
                if (dic.TryGetValue(MouseOutOpacityParameterName, out var p2))
                    if (p2 is double d)
                        _mouseOutOpacity = d;
            }
            BuildFadeInStoryboard(target);
            BuildFadeOutStoryboard(target);
        }

        private void BuildFadeOutStoryboard(FrameworkElement target)
        {
            _fadeInStoryboard = BuildFadeAnimStoryboard(target, _mouseOutOpacity, _mouseInOpacity );
        }

        private void BuildFadeInStoryboard(FrameworkElement target)
        {
            _fadeOutStoryboard = BuildFadeAnimStoryboard(target, _mouseInOpacity, _mouseOutOpacity);
        }

        Storyboard BuildFadeAnimStoryboard(FrameworkElement target,double fromValue,double toValue)
        {
            var storyboard = new Storyboard() { FillBehavior = FillBehavior.Stop };
            var effectDuration = new Duration(TimeSpan.FromMilliseconds(300));
            var fadeAnim = new DoubleAnimation(fromValue, toValue, effectDuration) { };
            var fadeAnimName = $"fade{fromValue}{toValue}";
            target.RegisterName(fadeAnimName, target);
            Storyboard.SetTargetName(fadeAnim, fadeAnimName);
            Storyboard.SetTargetProperty(fadeAnim, new PropertyPath(FrameworkElement.OpacityProperty));
            storyboard.Children.Add(fadeAnim);
            storyboard.Completed += (o, e) =>
            {
                target.Opacity = toValue;
            };
            return storyboard;
        }

        Storyboard GetAnimation(string name)
        {
            switch (name)
            {
                case FadeInAnimationName:
                    return _fadeInStoryboard;
                case FadeOutAnimationName:
                    return _fadeOutStoryboard;
                default:
                    throw new ArgumentException(nameof(name));
            }
        }

        public void Start(FrameworkElement target, string name = null, object parameters = null, EventHandler completed = null)
        {
            Initialize(target, parameters);
            var animation = GetAnimation(name);
            if (completed != null)
            {
                void OnCompleted(object o,EventArgs e)
                {
                    animation.Completed -= OnCompleted;
                    completed(o,e);
                }
                animation.Completed += OnCompleted;
            }
            animation.Begin(target);
        }

        public void Stop(string name = null)
        {
            var animation = GetAnimation(name);
            animation.Stop();
        }
    }
}

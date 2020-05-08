
#define dbg

using DesktopPanelTool.Behaviors.FrameworkElementBehaviors;
using DesktopPanelTool.Lib;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace DesktopPanelTool.Animations
{
    public class DragAndDropWidgetAnimation
        : IMultiAnimation
    {
        Storyboard _beginDragStoryboard;
        Storyboard _endDragStoryboard;
        ScaleTransform _beginScaleTransform;
        ScaleTransform _endScaleTransform;
        double _minWidthBackup;
        double _minHeightBackup;
        FrameworkElement _target;
        UIElement _previousElement;

        void Initialize(FrameworkElement target)
        {
            _target = target ?? throw new ArgumentNullException(nameof(target));
            NameScope.SetNameScope(target, new NameScope());
            (_beginDragStoryboard, _beginScaleTransform) = InitStoryBoard(DraggableFrameworkElementBehavior.BeginDragEffectAnimationName, target, 1, 1, 0.1, 0.1);
            (_endDragStoryboard, _endScaleTransform) = InitStoryBoard(DraggableFrameworkElementBehavior.EndDragEffectAnimationName, target, 0.1, 0.1, 1, 1);
            _beginDragStoryboard.Completed += (obj, e) => 
                target.MaxWidth = target.MaxHeight = 0;
            _endDragStoryboard.Completed += (obj, e) =>
            {
                target.LayoutTransform = null;
                target.MinWidth = _minWidthBackup;
                target.MinHeight = _minHeightBackup;
                target.SetValue(FrameworkElement.MaxWidthProperty, DependencyProperty.UnsetValue);
                target.SetValue(FrameworkElement.MaxHeightProperty, DependencyProperty.UnsetValue);
            };
        }

        Storyboard GetAnimation(string name)
        {
            switch (name)
            {
                case DraggableFrameworkElementBehavior.BeginDragEffectAnimationName:
                    return _beginDragStoryboard;
                case DraggableFrameworkElementBehavior.EndDragEffectAnimationName:
                    return _endDragStoryboard;
                default:
                    throw new ArgumentException(nameof(name));
            }
        }

        public void Start(FrameworkElement target, string name = null)
        {
            if (_beginDragStoryboard == null)
                Initialize(target);
            if (name == null) throw new ArgumentNullException(nameof(name));
            var animation = GetAnimation(name);
            switch (name)
            {
                case DraggableFrameworkElementBehavior.BeginDragEffectAnimationName:
                    target.LayoutTransform = _beginScaleTransform;
                    _minWidthBackup = target.MinWidth;
                    _minHeightBackup = target.MinHeight;
                    target.MinWidth = target.MinHeight = 0d;
                    var stackPanel = WPFUtil.FindLogicalParent<StackPanel>(target);
                    if (stackPanel != null)
                    {
                        var idx = stackPanel.Children.IndexOf(target);
                        if (idx>-1)
                        {
#if dbg
                            DesktopPanelTool.Lib.Debug.WriteLine($"idx={idx}");
#endif
                            _previousElement = stackPanel.Children[idx - 1];
                            _previousElement.Visibility = Visibility.Collapsed;
                        }    
                    }
                    break;
                case DraggableFrameworkElementBehavior.EndDragEffectAnimationName:
                    _target.LayoutTransform = _endScaleTransform;
                    _previousElement.Visibility = Visibility.Visible;
                    break;
            }
            animation.Begin(target);
        }

        public void Stop(string name = null)
        {
            var animation = GetAnimation(name);
            animation.Stop();
        }

        (Storyboard storyBoard, ScaleTransform scaleTransform) InitStoryBoard(
            string name,
            FrameworkElement o,
            double initialScaleX, double initialScaleY, double finalScaleX, double finalScaleY)
        {
            var storyBoard = new Storyboard() { FillBehavior = FillBehavior.Stop };

            var effectDuration = new Duration(TimeSpan.FromMilliseconds(200d));
            var scaleTransform = new ScaleTransform(initialScaleX, initialScaleY);
            var scaleXAnim = new DoubleAnimation(initialScaleX, finalScaleX, effectDuration);
            var scaleYAnim = new DoubleAnimation(initialScaleY, finalScaleY, effectDuration);
            var maxXAnim = new DoubleAnimation(o.ActualWidth, o.ActualWidth * finalScaleX, effectDuration);
            var maxYAnim = new DoubleAnimation(o.ActualHeight, o.ActualHeight * finalScaleY, effectDuration);
            var scaleXAnimName = $"scaleXAnim{name}";
            var scaleYAnimName = $"scaleYAnim{name}";
            var maxXAnimName = $"maxXAnim{name}";
            var maxYAnimName = $"maxYAnim{name}";

            o.RegisterName(scaleXAnimName, scaleTransform);
            o.RegisterName(scaleYAnimName, scaleTransform);
            o.RegisterName(maxXAnimName, o);
            o.RegisterName(maxYAnimName, o);

            Storyboard.SetTargetName(scaleXAnim, scaleXAnimName);
            Storyboard.SetTargetProperty(scaleXAnim, new PropertyPath(ScaleTransform.ScaleXProperty));
            Storyboard.SetTargetName(scaleYAnim, scaleYAnimName);
            Storyboard.SetTargetProperty(scaleYAnim, new PropertyPath(ScaleTransform.ScaleYProperty));
            Storyboard.SetTargetName(maxXAnim, maxXAnimName);
            Storyboard.SetTargetProperty(maxXAnim, new PropertyPath(FrameworkElement.MaxWidthProperty));
            Storyboard.SetTargetName(maxYAnim, maxYAnimName);
            Storyboard.SetTargetProperty(maxYAnim, new PropertyPath(FrameworkElement.MaxHeightProperty));

            storyBoard.Children.Add(scaleXAnim);
            storyBoard.Children.Add(scaleYAnim);
            storyBoard.Children.Add(maxXAnim);
            storyBoard.Children.Add(maxYAnim);

            return (storyBoard, scaleTransform);
        }
    }
}

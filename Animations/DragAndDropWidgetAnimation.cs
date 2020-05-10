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
        : IAnimations
    {
        Storyboard _beginDragStoryboard;
        Storyboard _endDragStoryboard;
        Storyboard _cancelDragStoryboard;

        ScaleTransform _beginScaleTransform;
        ScaleTransform _endScaleTransform;
        DoubleAnimation _translateXAnim;
        DoubleAnimation _translateYAnim;

        double _minWidthBackup;
        double _minHeightBackup;
        UIElement _previousElement;
        Window _sourceWindow;

        Point _initialLocation;
        bool _initialized = false;

        void Initialize(FrameworkElement target)
        {
            target = target ?? throw new ArgumentNullException(nameof(target));
            if (_initialized) return;
            _initialized = true;
            NameScope.SetNameScope(target, new NameScope());
        }

        Window GetPreviewImageCursorWindow(object parameters)
        {
            var p = ( parameters as Window) ?? throw new ArgumentNullException("parameters");
            return p;
        }

        void InitializeBeginDragStoryBoard(FrameworkElement target)
        {
            Initialize(target);
            _sourceWindow = WPFHelper.FindLogicalParent<Window>(target);
            var rpos = target.TranslatePoint(new Point(0, 0), _sourceWindow);
            _initialLocation = _sourceWindow.PointToScreen(rpos);
            if (_beginDragStoryboard == null)
            {
                (_beginDragStoryboard, _beginScaleTransform) = InitStoryboard(DraggableFrameworkElementBehavior.BeginDragEffectAnimationName, target, 1, 1, 0.1, 0.1);
                _beginDragStoryboard.Completed += (obj, e) =>
                    target.MaxWidth = target.MaxHeight = 0;
                InitializeEndDragStoryboard(target);
            }
        }

        void InitializeEndDragStoryboard(FrameworkElement target)
        {
            Initialize(target);
            if (_endDragStoryboard == null)
            {
                (_endDragStoryboard, _endScaleTransform) = InitStoryboard(DraggableFrameworkElementBehavior.EndDragEffectAnimationName, target, 0.1, 0.1, 1, 1);
                _endDragStoryboard.Completed += (obj, e) =>
                {
                    target.LayoutTransform = null;
                    target.MinWidth = _minWidthBackup;
                    target.MinHeight = _minHeightBackup;
                    target.SetValue(FrameworkElement.MaxWidthProperty, DependencyProperty.UnsetValue);
                    target.SetValue(FrameworkElement.MaxHeightProperty, DependencyProperty.UnsetValue);
                };
            }
        }

        void InitializeCancelDragStoryboard(Window previewImageCursorWindow)
        {
            Initialize(previewImageCursorWindow);            
            (_cancelDragStoryboard, _translateXAnim, _translateYAnim) = InitStoryboardCancelDrag(previewImageCursorWindow);
        }

        Storyboard GetAnimation(string name)
        {
            switch (name)
            {
                case DraggableFrameworkElementBehavior.BeginDragEffectAnimationName:
                    return _beginDragStoryboard;
                case DraggableFrameworkElementBehavior.EndDragEffectAnimationName:
                    return _endDragStoryboard;
                case DraggableFrameworkElementBehavior.CancelDragEffectAnimationName:
                    return _cancelDragStoryboard;
                default:
                    throw new ArgumentException(nameof(name));
            }
        }

        public void Start(FrameworkElement target, string name = null, object parameters = null,EventHandler completed = null)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            switch (name)
            {
                case DraggableFrameworkElementBehavior.CancelDragEffectAnimationName:
                    var previewImageCursorWindow = GetPreviewImageCursorWindow(target);
                    InitializeCancelDragStoryboard(previewImageCursorWindow);
                    _translateXAnim.From = previewImageCursorWindow.Left;
                    _translateXAnim.To = _initialLocation.X;
                    _translateYAnim.From = previewImageCursorWindow.Top;
                    _translateYAnim.To = _initialLocation.Y;
                    break;

                case DraggableFrameworkElementBehavior.BeginDragEffectAnimationName:
                    InitializeBeginDragStoryBoard(target);
                    target.LayoutTransform = _beginScaleTransform;
                    _minWidthBackup = target.MinWidth;
                    _minHeightBackup = target.MinHeight;
                    target.MinWidth = target.MinHeight = 0d;
                    var stackPanel = WPFHelper.FindLogicalParent<StackPanel>(target);
                    if (stackPanel != null)
                    {
                        var idx = stackPanel.Children.IndexOf(target);
                        if (idx>-1)
                        {
                            _previousElement = stackPanel.Children[idx - 1];
                            _previousElement.Visibility = Visibility.Collapsed;
                        }    
                    }
                    break;

                case DraggableFrameworkElementBehavior.EndDragEffectAnimationName:
                    target.LayoutTransform = _endScaleTransform;
                    if (_previousElement!=null) // temporary fix
                        _previousElement.Visibility = Visibility.Visible;
                    break;
            }

            var animation = GetAnimation(name);
            if (completed != null)
                animation.Completed += completed;
            animation.Begin(target);
        }

        public void Stop(string name = null)
        {
            var animation = GetAnimation(name);
            animation.Stop();
        }

        (Storyboard storyboard,DoubleAnimation translateXAnim,DoubleAnimation translateYAnim) InitStoryboardCancelDrag(Window o,EasingFunctionBase easing = null)
        {
            var storyboard = new Storyboard() { FillBehavior = FillBehavior.Stop };

            var effectDuration = new Duration(TimeSpan.FromMilliseconds(300));
            var translateXAnim = new DoubleAnimation(0, 0, effectDuration) { EasingFunction = easing };
            var translateYAnim = new DoubleAnimation(0, 0, effectDuration) { EasingFunction = easing };
            var translateXAnimName = $"trXAnim";
            var translateYAnimName = $"trYAnim";
            o.RegisterName(translateXAnimName, o);
            o.RegisterName(translateYAnimName, o);
            Storyboard.SetTargetName(translateXAnim, translateXAnimName);
            Storyboard.SetTargetProperty(translateXAnim, new PropertyPath(Window.LeftProperty));
            Storyboard.SetTargetName(translateYAnim, translateYAnimName);
            Storyboard.SetTargetProperty(translateYAnim, new PropertyPath(Window.TopProperty));
            storyboard.Children.Add(translateXAnim);
            storyboard.Children.Add(translateYAnim);
            return (storyboard,translateXAnim,translateYAnim);
        }

        (Storyboard storyboard, ScaleTransform scaleTransform) InitStoryboard(
            string name,
            FrameworkElement o,
            double initialScaleX, double initialScaleY, double finalScaleX, double finalScaleY,
            EasingFunctionBase easing = null)
        {
            var storyboard = new Storyboard() { FillBehavior = FillBehavior.Stop };

            var effectDuration = new Duration(TimeSpan.FromMilliseconds(200d));
            var scaleTransform = new ScaleTransform(initialScaleX, initialScaleY);
            var scaleXAnim = new DoubleAnimation(initialScaleX, finalScaleX, effectDuration) { EasingFunction = easing };
            var scaleYAnim = new DoubleAnimation(initialScaleY, finalScaleY, effectDuration) { EasingFunction = easing };
            var maxXAnim = new DoubleAnimation(o.ActualWidth, o.ActualWidth * finalScaleX, effectDuration) { EasingFunction = easing };
            var maxYAnim = new DoubleAnimation(o.ActualHeight, o.ActualHeight * finalScaleY, effectDuration) { EasingFunction = easing };
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

            storyboard.Children.Add(scaleXAnim);
            storyboard.Children.Add(scaleYAnim);
            storyboard.Children.Add(maxXAnim);
            storyboard.Children.Add(maxYAnim);

            return (storyboard, scaleTransform);
        }
    }
}

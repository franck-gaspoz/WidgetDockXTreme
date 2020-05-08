#define dbg

using DesktopPanelTool.Lib;
using DesktopPanelTool.Models;
using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace DesktopPanelTool.Behaviors.FrameworkElementBehaviors
{

    public class DraggableFrameworkElementBehavior
        : Behavior<FrameworkElement>
    {
        Point _start;
        DataObject _dataObject;
        int _draggedElementId;
        List<string> _draggableElementTypeNames;

        public bool IsEnabled
        {
            get { return (bool)GetValue(IsEnabledProperty); }
            set { SetValue(IsEnabledProperty, value); }
        }

        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.Register("IsEnabled", typeof(bool), typeof(DraggableFrameworkElementBehavior), new PropertyMetadata(true));

        public string DraggableElementTypeNames
        {
            get { return (string)GetValue(DraggableElementTypeNamesProperty); }
            set { SetValue(DraggableElementTypeNamesProperty, value); }
        }

        public static readonly DependencyProperty DraggableElementTypeNamesProperty =
            DependencyProperty.Register("DraggableElementTypeNames", 
                typeof(string), 
                typeof(DraggableFrameworkElementBehavior), 
                new PropertyMetadata("Border,Grid,StackPanel,TextBlock"));

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewMouseLeftButtonDown += AssociatedObject_PreviewMouseLeftButtonDown;
            AssociatedObject.MouseMove += AssociatedObject_MouseMove;
            if (DraggableElementTypeNames != null)
                _draggableElementTypeNames = DraggableElementTypeNames.Split(',').ToList();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching(); 
            AssociatedObject.PreviewMouseLeftButtonDown -= AssociatedObject_PreviewMouseLeftButtonDown;
            AssociatedObject.MouseMove -= AssociatedObject_MouseMove;
        }

        private void AssociatedObject_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsEnabled && CheckIsValidPosition())
            {
                Point mpos = e.GetPosition(null);
                Vector diff = _start - mpos;

                if (e.LeftButton == MouseButtonState.Pressed &&
                    Math.Abs(diff.X) > AppSettings.MinimumHorizontalDragDistance &&
                    Math.Abs(diff.Y) > AppSettings.MinimumVerticalDragDistance)
                {
#if dbg
                    DesktopPanelTool.Lib.Debug.WriteLine($"do drag drop");
#endif
                    StartBeginDragEffect(AssociatedObject);

                    _dataObject = new DataObject(AssociatedObject.GetType(), AssociatedObject);
                    DragDrop.DoDragDrop(AssociatedObject, _dataObject, DragDropEffects.Move);

                    // remark: get here only after dropped or drop canceled
                    StartEndDragEffect(AssociatedObject);
#if dbg
                    DesktopPanelTool.Lib.Debug.WriteLine($"dropped");
#endif
                }
            }
        }

        Storyboard _beginDragStoryboard;
        Storyboard _endDragStoryboard;
        ScaleTransform _beginScaleTransform;
        ScaleTransform _endScaleTransform;
        double _minWidthBackup;
        double _minHeightBackup;

        void StartBeginDragEffect(FrameworkElement o)
        {
            if (_beginDragStoryboard == null)
            {
                NameScope.SetNameScope(o, new NameScope());
                (_beginDragStoryboard,_beginScaleTransform) = InitStoryBoard(1,o,1,1,0.1,0.1);
                (_endDragStoryboard,_endScaleTransform) = InitStoryBoard(2,o, 0.1, 0.1, 1, 1);
            }
            o.LayoutTransform = _beginScaleTransform;
            _minWidthBackup = o.MinWidth;
            _minHeightBackup = o.MinHeight;
            o.MinWidth = o.MinHeight = 0d;
            _beginDragStoryboard.Completed += (obj, e) =>
            {
                //_beginDragStoryboard.Stop();
                o.MaxWidth = o.MaxHeight = 0;
            };
            _beginDragStoryboard.Begin(o);
        }

        public void StartEndDragEffect(FrameworkElement o)
        {
            o.LayoutTransform = _endScaleTransform;
            _endDragStoryboard.FillBehavior = FillBehavior.Stop;
            _endDragStoryboard.Completed += (obj, e) =>
            {
                //_endDragStoryboard.Stop();
                o.LayoutTransform = null;
                o.MinWidth = _minWidthBackup;
                o.MinHeight = _minHeightBackup;
                o.SetValue(FrameworkElement.MaxWidthProperty, DependencyProperty.UnsetValue);
                o.SetValue(FrameworkElement.MaxHeightProperty, DependencyProperty.UnsetValue);
            };
            _endDragStoryboard.Begin(o);
        }

        (Storyboard storyBoard,ScaleTransform scaleTransform) InitStoryBoard(
            int id,
            FrameworkElement o,
            double initialScaleX,double initialScaleY,double finalScaleX,double finalScaleY) 
        {
            var storyBoard = new Storyboard() { FillBehavior = FillBehavior.Stop };
           
            var effectDuration = new Duration(TimeSpan.FromMilliseconds(200d));
            var scaleTransform = new ScaleTransform(initialScaleX, initialScaleY);
            var scaleXAnim = new DoubleAnimation(initialScaleX, finalScaleX, effectDuration);
            var scaleYAnim = new DoubleAnimation(initialScaleY, finalScaleY, effectDuration); 
            var maxXAnim = new DoubleAnimation(o.ActualWidth, o.ActualWidth*finalScaleX, effectDuration);
            var maxYAnim = new DoubleAnimation(o.ActualHeight, o.ActualHeight*finalScaleY, effectDuration);
            var scaleXAnimName = $"scaleXAnim{id}";
            var scaleYAnimName = $"scaleYAnim{id}";
            var maxXAnimName = $"maxXAnim{id}";
            var maxYAnimName = $"maxYAnim{id}";

            o.RegisterName(scaleXAnimName,scaleTransform);
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

            return (storyBoard,scaleTransform);
        }

        private void AssociatedObject_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (IsEnabled && CheckIsValidPosition())
            {
                _start = e.GetPosition(null);
#if dbg
                DesktopPanelTool.Lib.Debug.WriteLine($"init start drag position");
#endif
            }
        }

        private bool CheckIsValidPosition()
        {
            var ctrl = Mouse.DirectlyOver;

            if (ctrl is FrameworkElement e)
            {
#if false && dbg
                DesktopPanelTool.Lib.Debug.WriteLine($"ctrl={ctrl?.GetType().Name} isEnabled={IsEnabled}");
#endif
                return _draggableElementTypeNames == null || _draggableElementTypeNames.Contains(ctrl?.GetType().Name);
            }
            return false;
        }

        public static void SetIsEnabled(bool isEnabled, FrameworkElement frameworkElement)
        {
            var o = Interaction.GetBehaviors(frameworkElement)
                .OfType<DraggableFrameworkElementBehavior>()
                .FirstOrDefault();
            if (o != null)
            {
                o.IsEnabled = isEnabled;
            }
        }

        public static void SetIsEnabledDraggableDescendants(DependencyObject root,Type descendantType,bool isEnabled)
        {
#if false && dbg
            DesktopPanelTool.Lib.Debug.WriteLine($"set isEnable {descendantType.Name} isEnabled={isEnabled}");
#endif
            if (descendantType != null)
            {
                var draggableElements = WPFUtil.FindChilds(descendantType, root);
                foreach (var de in draggableElements)
                    DraggableFrameworkElementBehavior.SetIsEnabled(isEnabled, (FrameworkElement)de);
            }
        }

    }
}

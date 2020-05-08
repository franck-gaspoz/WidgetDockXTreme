//#define dbg

using DesktopPanelTool.Lib;
using DesktopPanelTool.Models;
using DesktopPanelTool.Views;
using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using static DesktopPanelTool.Lib.NativeMethods;

namespace DesktopPanelTool.Behaviors.FrameworkElementBehaviors
{
    public class DraggableFrameworkElementBehavior
        : Behavior<FrameworkElement>
    {
        DragImage _cursorWindow = null;
        double _cursorWindowXHotSpot = 4;
        double _cursorWindowYHotSpot = 4;

        public bool IsDropPreviewEnabled
        {
            get { return (bool)GetValue(IsDropPreviewEnabledProperty); }
            set { SetValue(IsDropPreviewEnabledProperty, value); }
        }

        public static readonly DependencyProperty IsDropPreviewEnabledProperty =
            DependencyProperty.Register("IsDropPreviewEnabled", typeof(bool), typeof(DraggableFrameworkElementBehavior), new PropertyMetadata(true));

        public IMultiAnimation DragDropAnimation
        {
            get { return (IMultiAnimation)GetValue(DragDropAnimationProperty); }
            set { SetValue(DragDropAnimationProperty, value); }
        }

        public static readonly DependencyProperty DragDropAnimationProperty =
            DependencyProperty.Register("DragDropAnimation", typeof(IMultiAnimation), typeof(DraggableFrameworkElementBehavior), new PropertyMetadata(null));

        Point _start;
        DataObject _dataObject;
        List<string> _draggableElementTypeNames;

        public const string BeginDragEffectAnimationName = "BeginDragEffectAnimationName";
        public const string EndDragEffectAnimationName = "EndDragEffectAnimationName";

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
            if (IsEnabled && CheckIsValidDrag())
            {
                Point mpos = e.GetPosition(null);
                Vector diff = _start - mpos;

                if (e.LeftButton == MouseButtonState.Pressed &&
                    Math.Abs(diff.X) > AppSettings.MinimumHorizontalDragDistance &&
                    Math.Abs(diff.Y) > AppSettings.MinimumVerticalDragDistance)
                {
                    DragDropAnimation?.Start(AssociatedObject,BeginDragEffectAnimationName);
                    AssociatedObject.GiveFeedback += AssociatedObject_GiveFeedback;

                    _dataObject = new DataObject(AssociatedObject.GetType(), AssociatedObject);
                    DragDrop.DoDragDrop(AssociatedObject, _dataObject, DragDropEffects.Move);

                    AssociatedObject.GiveFeedback -= AssociatedObject_GiveFeedback;
                    _cursorWindow?.Close();
                    _cursorWindow = null;
                    DragDropAnimation?.Start(AssociatedObject,EndDragEffectAnimationName);
                }
            }
        }

        private void AssociatedObject_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            if (!IsDropPreviewEnabled) return;                

            if (_cursorWindow == null)
            {
                if (e.Source is UIElement element && element!=null)
                {
                    var rtb = WPFHelper.GetRenderTargetBitmap(element);
                    _cursorWindow = new DragImage()
                    {
                        Title = $"{AppSettings.AppTitle} cursor",
                        Width = rtb.Width,
                        Height = rtb.Height
                    };
                    _cursorWindow.IMG.Source = rtb;
                }
            }
            if (_cursorWindow!=null)
            {
                var p = new NativeTypes.POINT();
                GetCursorPos(ref p);
                var x = p.X - _cursorWindow.ActualWidth - _cursorWindowXHotSpot;
                var y = p.Y - _cursorWindow.ActualHeight - _cursorWindowYHotSpot;
                _cursorWindow.Left = x;
                _cursorWindow.Top = y;
                _cursorWindow.Topmost = false;
                _cursorWindow.Topmost = true;
                if (!_cursorWindow.IsVisible) _cursorWindow.Show();
            }
        }

        private void AssociatedObject_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (IsEnabled && CheckIsValidDrag())
            {
                _start = e.GetPosition(null);
#if dbg
                DesktopPanelTool.Lib.Debug.WriteLine($"init start drag position");
#endif
            }
        }

        private bool CheckIsValidDrag()
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
                var draggableElements = WPFHelper.FindChilds(descendantType, root);
                foreach (var de in draggableElements)
                    DraggableFrameworkElementBehavior.SetIsEnabled(isEnabled, (FrameworkElement)de);
            }
        }

    }
}

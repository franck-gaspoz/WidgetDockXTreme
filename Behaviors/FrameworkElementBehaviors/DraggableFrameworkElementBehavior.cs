﻿//#define dbg

using DesktopPanelTool.Behaviors.WindowBehaviors;
using DesktopPanelTool.Lib;
using DesktopPanelTool.Models;
using DesktopPanelTool.Views;
using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using static DesktopPanelTool.Lib.NativeMethods;

namespace DesktopPanelTool.Behaviors.FrameworkElementBehaviors
{
    public class DraggableFrameworkElementBehavior
        : Behavior<FrameworkElement>
    {
        DragImage _cursorWindow = null;
        IntPtr _cursorWindowHandle = (IntPtr)0;
        double _cursorWindowXHotSpot = 9;   // TODO: may depend on system cursor size, depends on shadow size (depth+softness)
        double _cursorWindowYHotSpot = 9;

        public bool IsDropPreviewEnabled
        {
            get { return (bool)GetValue(IsDropPreviewEnabledProperty); }
            set { SetValue(IsDropPreviewEnabledProperty, value); }
        }

        public static readonly DependencyProperty IsDropPreviewEnabledProperty =
            DependencyProperty.Register("IsDropPreviewEnabled", typeof(bool), typeof(DraggableFrameworkElementBehavior), new PropertyMetadata(true));

        public IAnimations DragDropAnimation
        {
            get { return (IAnimations)GetValue(DragDropAnimationProperty); }
            set { SetValue(DragDropAnimationProperty, value); }
        }

        public static readonly DependencyProperty DragDropAnimationProperty =
            DependencyProperty.Register("DragDropAnimation", typeof(IAnimations), typeof(DraggableFrameworkElementBehavior), new PropertyMetadata(null));

        Point _start;
        DataObject _dataObject;
        List<string> _draggableElementTypeNames;

        public const string BeginDragEffectAnimationName = "BeginDragEffectAnimationName";
        public const string EndDragEffectAnimationName = "EndDragEffectAnimationName";
        public const string CancelDragEffectAnimationName = "CancelDragEffectAnimationName";

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
                    var r = DragDrop.DoDragDrop(AssociatedObject, _dataObject, DragDropEffects.Move);

                    AssociatedObject.GiveFeedback -= AssociatedObject_GiveFeedback;

                    if (r == DragDropEffects.None)
                        DragDropAnimation?.Start(_cursorWindow, CancelDragEffectAnimationName, null, (o,e)=>DestroyCursorWindow());

                    DragDropAnimation?.Start(AssociatedObject,EndDragEffectAnimationName);

                    if (DragDropAnimation == null || r!=DragDropEffects.None)
                        DestroyCursorWindow();
                }
            }
        }

        void DestroyCursorWindow()
        {
            _cursorWindow?.Close();
            _cursorWindow = null;
            _cursorWindowHandle = (IntPtr)0;
        }

        private void AssociatedObject_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            if (!IsDropPreviewEnabled) return;

            if (_cursorWindow == null)
            {
                if (e.Source is FrameworkElement element && element!=null)
                {
                    var rtb = WPFHelper.GetRenderTargetBitmap(element);
                    var shadowAreaSize = (double)AssociatedObject.FindResource("DragWindowShadowAreaSize");
                    _cursorWindow = new DragImage()
                    {
                        Title = $"{AppSettings.AppTitle} cursor",
                        Width = rtb.Width+ shadowAreaSize * 2d,
                        Height = rtb.Height+ shadowAreaSize * 2d
                    };
                    _cursorWindowXHotSpot = -shadowAreaSize / 2d + 2d;
                    _cursorWindowYHotSpot = -shadowAreaSize / 2d + 2d;
                    _cursorWindow.IMG.Source = rtb;
                    _cursorWindow.BackImg.Source = WPFHelper.GetImageMask(rtb);
                }
            }
            if (_cursorWindow!=null)
            {
                var p = new NativeTypes.POINT();
                GetCursorPos(ref p);
                var x = p.X - _cursorWindow.ActualWidth - _cursorWindowXHotSpot ;
                var y = p.Y - _cursorWindow.ActualHeight - _cursorWindowYHotSpot ;

                _cursorWindow.SetPos(x, y, (IntPtr)SpecialWindowHandles.HWND_TOP);

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

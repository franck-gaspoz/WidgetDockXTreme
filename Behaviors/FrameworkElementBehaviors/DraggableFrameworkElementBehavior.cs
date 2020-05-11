#define dbg

using DesktopPanelTool.ComponentModels;
using DesktopPanelTool.Lib;
using DesktopPanelTool.Models;
using DesktopPanelTool.Views;
using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using static DesktopPanelTool.Lib.NativeMethods;
using static DesktopPanelTool.Models.NativeTypes;
using dr = System.Drawing;

namespace DesktopPanelTool.Behaviors.FrameworkElementBehaviors
{
    public class DraggableFrameworkElementBehavior
        : Behavior<FrameworkElement>
    {
        DragImage _cursorWindow = null;
        double _cursorWindowXHotSpot = 9;   // TODO: may depend on system cursor size, depends on shadow size (depth+softness)
        double _cursorWindowYHotSpot = 9;

        public ICommand DropOnDesktopHandlerCommand
        {
            get { return (ICommand)GetValue(DropOnDesktopHandlerCommandProperty); }
            set { SetValue(DropOnDesktopHandlerCommandProperty, value); }
        }

        public static readonly DependencyProperty DropOnDesktopHandlerCommandProperty =
            DependencyProperty.Register("DropOnDesktopHandlerCommand", typeof(ICommand), typeof(DraggableFrameworkElementBehavior), new PropertyMetadata(null));

        public bool CanDropOnDesktop
        {
            get { return (bool)GetValue(CanDropOnDesktopProperty); }
            set { SetValue(CanDropOnDesktopProperty, value); }
        }

        public static readonly DependencyProperty CanDropOnDesktopProperty =
            DependencyProperty.Register("CanDropOnDesktop", typeof(bool), typeof(DraggableFrameworkElementBehavior), new PropertyMetadata(false));

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

        Point? _start;
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
            AssociatedObject.PreviewMouseLeftButtonUp += AssociatedObject_PreviewMouseLeftButtonUp;
            AssociatedObject.MouseMove += AssociatedObject_MouseMove;
            if (DraggableElementTypeNames != null)
                _draggableElementTypeNames = DraggableElementTypeNames.Split(',').ToList();
        }

        private void AssociatedObject_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _start = null;
#if dbg
            DesktopPanelTool.Lib.Debug.WriteLine($"button up");
#endif
        }

        protected override void OnDetaching()
        {
            base.OnDetaching(); 
            AssociatedObject.PreviewMouseLeftButtonDown -= AssociatedObject_PreviewMouseLeftButtonDown;
            AssociatedObject.PreviewMouseLeftButtonUp -= AssociatedObject_PreviewMouseLeftButtonUp;
            AssociatedObject.MouseMove -= AssociatedObject_MouseMove;
        }

        private void AssociatedObject_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsEnabled && CheckIsValidDrag() && _start.HasValue )
            {
                //Point mpos = e.GetPosition(null);
                var mp = new POINT();
                GetCursorPos(ref mp);
                var mpos = new Point(mp.X, mp.Y);
                Vector diff = _start.Value - mpos;
                
                if (e.LeftButton == MouseButtonState.Pressed &&
                    Math.Abs(diff.X) > AppSettings.MinimumHorizontalDragDistance ||
                    Math.Abs(diff.Y) > AppSettings.MinimumVerticalDragDistance)
                {
                    _start = null;

                    if (AssociatedObject is IAnimatableElement ae)
                    {
                        ae.WidthBackup = AssociatedObject.ActualWidth;
                        ae.HeightBackup = AssociatedObject.ActualHeight;
                    }

                    DragDropAnimation?.Start(AssociatedObject,BeginDragEffectAnimationName);
                    AssociatedObject.GiveFeedback += AssociatedObject_GiveFeedback;

#if dbg
                    DesktopPanelTool.Lib.Debug.WriteLine($"do drag drop (mpos={mpos}) (diff.X={diff.X} diff.Y={diff.Y})");
#endif
                    _dataObject = new DataObject(AssociatedObject.GetType(), AssociatedObject);
                    var r = DragDrop.DoDragDrop(AssociatedObject, _dataObject, DragDropEffects.Move);

                    var p = new POINT();
                    GetCursorPos(ref p);
                    AssociatedObject.GiveFeedback -= AssociatedObject_GiveFeedback;
                    var dropAcceptedInApp = r != DragDropEffects.None;
                    var dropAcceptedOnDesktop = CanDropOnDesktop && (!dropAcceptedInApp && CheckIsOverDesktop(p.X, p.Y));
                    var dropAccepted = dropAcceptedInApp | dropAcceptedOnDesktop;

                    if (!dropAccepted)
                        DragDropAnimation?.Start(_cursorWindow, CancelDragEffectAnimationName, null, (o,e)=>DestroyCursorWindow());

                    if (!dropAcceptedOnDesktop)
                        DragDropAnimation?.Start(AssociatedObject,EndDragEffectAnimationName);

                    if (DragDropAnimation == null || dropAccepted)
                        DestroyCursorWindow();

                    if (dropAcceptedOnDesktop && DropOnDesktopHandlerCommand != null)
                    {
                        var dragComponentData = new DragData(_dataObject, null, AssociatedObject);
                        if (DropOnDesktopHandlerCommand.CanExecute(dragComponentData))
                            DropOnDesktopHandlerCommand.Execute(dragComponentData);
                        DragDropAnimation?.Start(AssociatedObject, EndDragEffectAnimationName);
                    }
                }
            }
        }

        void DestroyCursorWindow()
        {
            _cursorWindow?.Close();
            _cursorWindow = null;
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
                    _cursorWindowXHotSpot = -shadowAreaSize / 2d + 2d;  // TODO: 2d gap could depends on system cursor size
                    _cursorWindowYHotSpot = -shadowAreaSize / 2d + 2d;
                    _cursorWindow.IMG.Source = rtb;
                    _cursorWindow.BackImg.Source = WPFHelper.GetImageMask(rtb);
                }
            }
            
            var p = new NativeTypes.POINT();
            GetCursorPos(ref p);

            if (_cursorWindow!=null)
            {
                var x = p.X - _cursorWindow.ActualWidth - _cursorWindowXHotSpot ;
                var y = p.Y - _cursorWindow.ActualHeight - _cursorWindowYHotSpot ;

                _cursorWindow.SetPos(x, y, (IntPtr)SpecialWindowHandles.HWND_TOP);

                if (!_cursorWindow.IsVisible) _cursorWindow.Show();
            }

            if (CanDropOnDesktop && CheckIsOverDesktop(p.X,p.Y))
            {
                e.UseDefaultCursors = false;
                Mouse.SetCursor(Cursors.Hand);
                e.Handled = true;
            }
        }

        bool CheckIsOverDesktop(int x,int y)
        {
            var overHandle = WindowFromPoint(new dr.Point(x, y));
            StringBuilder sb3 = new StringBuilder(1024);
            GetClassName(overHandle, sb3, sb3.Capacity);
            var className = sb3.ToString();
            WINDOWINFO wininfo = new WINDOWINFO();
            StringBuilder sb4 = new StringBuilder(1024);
            GetWindowText(overHandle, sb4, 1024);
            var hTitle = sb4.ToString();
            if (GetWindowInfo(overHandle, ref wininfo))
            {
#if false && dbg
                DesktopPanelTool.Lib.Debug.WriteLine($"drag over: pos={x},{y} title={hTitle} clName={className} wininfo: atom={wininfo.atomWindowType} status={wininfo.dwWindowStatus}");
#endif
                return (hTitle == "FolderView" && className == "SysListView32");
            }
            return false;
        }

        private void AssociatedObject_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (IsEnabled && CheckIsValidDrag())
            {
                //_start = e.GetPosition(null);
                var p = new POINT();
                GetCursorPos(ref p);
                _start = new Point(p.X, p.Y);
#if dbg
                DesktopPanelTool.Lib.Debug.WriteLine($"init start drag position (start={_start})");
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

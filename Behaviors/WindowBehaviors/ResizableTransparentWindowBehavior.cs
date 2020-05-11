//#define dbg

using DesktopPanelTool.Lib;
using Microsoft.Xaml.Behaviors;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shell;

namespace DesktopPanelTool.Behaviors.WindowBehaviors
{
    internal class ResizableTransparentWindowBehavior
        : Behavior<Window>
    {
        public bool IsResizable = true;

        public static double ResizeGripEdgeSize = 4;

        public static Cursor CursorDefault = Cursors.Arrow;
        public static Cursor CursorResizeWE = Cursors.SizeWE;
        public static Cursor CursorResizeNS = Cursors.SizeNS;
        public static Cursor CursorResizeNESW = Cursors.SizeNESW;
        public static Cursor CursorResizeNWSE = Cursors.SizeNWSE;

        /// <summary>
        /// no more than 1/MaximumMoveEventRate*1000 window size changed per second
        /// </summary>
        public static double MaximumMoveEventRate = 25;
        public static bool LimitFrameRate = false;

        DateTime _lastDragMoveTime;

        public delegate (bool validatedWidth,bool validatedHeight) ValidateResizeDelegate(double width, double height);

        public ValidateResizeDelegate ValidateResize;

        public bool CanResizeLeft
        {
            get { return (bool)GetValue(CanResizeLeftProperty); }
            set { SetValue(CanResizeLeftProperty, value); }
        }

        public static readonly DependencyProperty CanResizeLeftProperty =
            DependencyProperty.Register("CanResizeLeft", typeof(bool), typeof(ResizableTransparentWindowBehavior), new PropertyMetadata(true));

        public bool CanResizeTop
        {
            get { return (bool)GetValue(CanResizeTopProperty); }
            set { SetValue(CanResizeTopProperty, value); }
        }

        public static readonly DependencyProperty CanResizeTopProperty =
            DependencyProperty.Register("CanResizeTop", typeof(bool), typeof(ResizableTransparentWindowBehavior), new PropertyMetadata(true));

        public bool CanResizeRight
        {
            get { return (bool)GetValue(CanResizeRightProperty); }
            set { SetValue(CanResizeRightProperty, value); }
        }

        public static readonly DependencyProperty CanResizeRightProperty =
            DependencyProperty.Register("CanResizeRight", typeof(bool), typeof(ResizableTransparentWindowBehavior), new PropertyMetadata(true));

        public bool CanResizeBottom
        {
            get { return (bool)GetValue(CanResizeBottomProperty); }
            set { SetValue(CanResizeBottomProperty, value); }
        }

        public static readonly DependencyProperty CanResizeBottomProperty =
            DependencyProperty.Register("CanResizeBottom", typeof(bool), typeof(ResizableTransparentWindowBehavior), new PropertyMetadata(true));

        public bool CanResizeTopLeft
        {
            get { return (bool)GetValue(CanResizeTopLeftProperty); }
            set { SetValue(CanResizeTopLeftProperty, value); }
        }

        public static readonly DependencyProperty CanResizeTopLeftProperty =
            DependencyProperty.Register("CanResizeTopLeft", typeof(bool), typeof(ResizableTransparentWindowBehavior), new PropertyMetadata(true));

        public bool CanResizeTopRight
        {
            get { return (bool)GetValue(CanResizeTopRightProperty); }
            set { SetValue(CanResizeTopRightProperty, value); }
        }

        public static readonly DependencyProperty CanResizeTopRightProperty =
            DependencyProperty.Register("CanResizeTopRight", typeof(bool), typeof(ResizableTransparentWindowBehavior), new PropertyMetadata(true));

        public bool CanResizeBottomRight
        {
            get { return (bool)GetValue(CanResizeBottomRightProperty); }
            set { SetValue(CanResizeBottomRightProperty, value); }
        }

        public static readonly DependencyProperty CanResizeBottomRightProperty =
            DependencyProperty.Register("CanResizeBottomRight", typeof(bool), typeof(ResizableTransparentWindowBehavior), new PropertyMetadata(true));

        public bool CanResizeBottomLeft
        {
            get { return (bool)GetValue(CanResizeBottomLeftProperty); }
            set { SetValue(CanResizeBottomLeftProperty, value); }
        }

        public static readonly DependencyProperty CanResizeBottomLeftProperty =
            DependencyProperty.Register("CanResizeBottomLeft", typeof(bool), typeof(ResizableTransparentWindowBehavior), new PropertyMetadata(true));

        ResizeGripDirection _currentResizeGripDirection = ResizeGripDirection.None;
        double _initialDx = 0;
        double _initialDy = 0;

        ResizeGripDirection[,] _gridDrs =
            new ResizeGripDirection[,]
            {
                { ResizeGripDirection.TopLeft , ResizeGripDirection.Top , ResizeGripDirection.TopRight },
                { ResizeGripDirection.Left , ResizeGripDirection.None , ResizeGripDirection.Right },
                { ResizeGripDirection.BottomLeft , ResizeGripDirection.Bottom , ResizeGripDirection.BottomRight }
            };

        Cursor[,] _curDrs =
            new Cursor[,]
            {
                { CursorResizeNWSE , CursorResizeNS , CursorResizeNESW },
                { CursorResizeWE , CursorDefault , CursorResizeWE },
                { CursorResizeNESW , CursorResizeNS , CursorResizeNWSE }
            };

        bool _isResizing = false;

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.MouseMove += AssociatedObject_MouseMove;
            AssociatedObject.MouseDown += AssociatedObject_MouseDown;
            AssociatedObject.MouseUp += AssociatedObject_MouseUp;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.MouseMove -= AssociatedObject_MouseMove;
            AssociatedObject.MouseDown -= AssociatedObject_MouseDown;
            AssociatedObject.MouseUp -= AssociatedObject_MouseUp;
        }

        private void AssociatedObject_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!IsResizable)
                return;

            if (_currentResizeGripDirection != ResizeGripDirection.None
                && e.ChangedButton==MouseButton.Left)
            {
                _isResizing = true;
                var p = e.GetPosition(AssociatedObject);
                _initialDx = _initialDx = 0;
                switch (_currentResizeGripDirection)
                {
                    case ResizeGripDirection.Right:
                        _initialDx = AssociatedObject.ActualWidth - p.X;
                        break;
                    case ResizeGripDirection.Left:
                        _initialDx = -p.X;
                        break;
                    case ResizeGripDirection.Top:
                        _initialDy = -p.Y;
                        break;
                    case ResizeGripDirection.Bottom:
                        _initialDy = AssociatedObject.ActualHeight - p.Y;
                        break;
                    case ResizeGripDirection.TopRight:
                        _initialDx = AssociatedObject.ActualWidth - p.X;
                        _initialDy = -p.Y;
                        break;
                    case ResizeGripDirection.BottomRight:
                        _initialDx = AssociatedObject.ActualWidth - p.X;
                        _initialDy = AssociatedObject.ActualHeight - p.Y;
                        break;
                    case ResizeGripDirection.BottomLeft:
                        _initialDy = AssociatedObject.ActualHeight - p.Y;
                        _initialDx = -p.X;
                        break;
                    case ResizeGripDirection.TopLeft:
                        _initialDy = -p.Y;
                        _initialDx = -p.X;
                        break;
                }
                DockablePanelWindowBehavior.DisableDockBehavior(AssociatedObject);
                Mouse.Capture(AssociatedObject);
#if alldbg || dbg
                DesktopPanelTool.Lib.Debug.WriteLine($"is resizing dx={InitialDx},dy={InitialDy} x={p.X},y={p.Y} (rgd={CurrentResizeGripDirection})");
#endif
            }
        }

        private void AssociatedObject_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton==MouseButton.Left)
            {
                _currentResizeGripDirection = ResizeGripDirection.None;
                if (_isResizing)
                {
                    _isResizing = false;
                    _lastDragMoveTime = DateTime.Now;
                    Mouse.Capture(null);
                    AssociatedObject.Cursor = CursorDefault;
                    DockablePanelWindowBehavior.EnableDockBehavior(AssociatedObject);
#if alldbg || dbg
                    DesktopPanelTool.Lib.Debug.WriteLine($"not resizing");
#endif
                }
            }
        }

        private void AssociatedObject_MouseMove(
            object sender, MouseEventArgs e)
        {
            if (!IsResizable)
                return;

            var p = e.GetPosition(AssociatedObject);
            if (_isResizing)
            {
                var elapsed = DateTime.Now - _lastDragMoveTime;
                if (LimitFrameRate &&
                    elapsed.TotalMilliseconds < MaximumMoveEventRate)
                    return;

#if alldbg || dbg
                DesktopPanelTool.Lib.Debug.WriteLine($"mouse move x={p.X},y={p.Y}");
#endif
                p.X += _initialDx;
                p.Y += _initialDy;

                double newLeft = AssociatedObject.Left;
                double newTop = AssociatedObject.Top;
                double newWidth = AssociatedObject.ActualWidth;
                double newHeight = AssociatedObject.ActualHeight;

                double calcLeft, calcTop, calcWidth, calcHeight;

                switch (_currentResizeGripDirection)
                {
                    case ResizeGripDirection.Left:
                        calcLeft = AssociatedObject.Left + p.X;
                        calcWidth = AssociatedObject.ActualWidth - p.X;
                        if (calcWidth < AssociatedObject.MinWidth) calcLeft = AssociatedObject.Left;
                        calcWidth = Math.Max(calcWidth, AssociatedObject.MinWidth);
                        newLeft = calcLeft;
                        newWidth = calcWidth;
                        break;
                    case ResizeGripDirection.Right:
                        calcWidth = p.X;
                        calcWidth = Math.Max(calcWidth, AssociatedObject.MinWidth);
                        newWidth = calcWidth;
                        break;
                    case ResizeGripDirection.Top:
                        calcTop = AssociatedObject.Top + p.Y;
                        calcHeight = AssociatedObject.ActualHeight - p.Y;
                        if (calcHeight < AssociatedObject.MinHeight) calcTop = AssociatedObject.Top;
                        calcHeight = Math.Max(calcHeight, AssociatedObject.MinHeight);
                        newTop = calcTop;
                        newHeight = calcHeight;
                        break;
                    case ResizeGripDirection.Bottom:
                        calcHeight = p.Y;
                        calcHeight = Math.Max(calcHeight, AssociatedObject.MinHeight);
                        newHeight = calcHeight;
                        break;
                    case ResizeGripDirection.BottomRight:
                        calcWidth = p.X;
                        calcHeight = p.Y;
                        calcHeight = Math.Max(calcHeight, AssociatedObject.MinHeight);
                        newHeight = calcHeight;
                        calcWidth = Math.Max(calcWidth, AssociatedObject.MinWidth);
                        newWidth = calcWidth;
                        break;
                    case ResizeGripDirection.BottomLeft:
                        calcLeft = AssociatedObject.Left + p.X;
                        calcHeight = p.Y;
                        calcWidth = AssociatedObject.ActualWidth - p.X;
                        calcHeight = Math.Max(calcHeight, AssociatedObject.MinHeight);
                        if (calcWidth < AssociatedObject.MinWidth) calcLeft = AssociatedObject.Left;
                        calcWidth = Math.Max(calcWidth, AssociatedObject.MinWidth);
                        newHeight = calcHeight;
                        newLeft = calcLeft;
                        newWidth = calcWidth;
                        break;
                    case ResizeGripDirection.TopRight:
                        calcTop = AssociatedObject.Top + p.Y;
                        calcHeight = AssociatedObject.ActualHeight - p.Y;
                        calcWidth = p.X;
                        if (calcHeight < AssociatedObject.MinHeight) calcTop = AssociatedObject.Top;
                        calcHeight = Math.Max(calcHeight, AssociatedObject.MinHeight);
                        calcWidth = Math.Max(calcWidth, AssociatedObject.MinWidth);
                        newWidth = calcWidth;
                        newTop = calcTop;
                        newHeight = calcHeight;
                        break;
                    case ResizeGripDirection.TopLeft:
                        calcTop = AssociatedObject.Top + p.Y;
                        calcLeft = AssociatedObject.Left + p.X;
                        calcHeight = AssociatedObject.ActualHeight - p.Y;
                        calcWidth = AssociatedObject.ActualWidth - p.X;
                        if (calcWidth < AssociatedObject.MinWidth) calcLeft = AssociatedObject.Left;
                        calcWidth = Math.Max(calcWidth, AssociatedObject.MinWidth);
                        if (calcHeight < AssociatedObject.MinHeight) calcTop = AssociatedObject.Top;
                        calcHeight = Math.Max(calcHeight, AssociatedObject.MinHeight);
                        newTop = calcTop;
                        newHeight = calcHeight;
                        newLeft = calcLeft;
                        newWidth = calcWidth;
                        break;
                }

                if (ValidateResize==null)
                    AssociatedObject.SetPosAndSize(newLeft, newTop, newWidth, newHeight);
                else
                {
                    var (validatedWidth, validatedHeight) = ValidateResize(newWidth, newHeight);
                    if (validatedWidth && validatedHeight)
                        AssociatedObject.SetPosAndSize(newLeft, newTop, newWidth, newHeight);
                    else
                    {
                        if (validatedWidth)
                            AssociatedObject.SetPosAndSize(newLeft, AssociatedObject.Top, newWidth, AssociatedObject.Height);
                        if (validatedHeight)
                            AssociatedObject.SetPosAndSize(AssociatedObject.Left, newTop, AssociatedObject.ActualWidth, newHeight);
                    }
                }

                _lastDragMoveTime = DateTime.Now;
            }
            else
            {
                var rgd = CheckRGD(p.X, p.Y);
                AssociatedObject.Cursor = _curDrs[rgd.hy, rgd.hx];
                _currentResizeGripDirection = rgd.rgd;
#if alldbg || dbg
                DesktopPanelTool.Lib.Debug.WriteLine($"mouse move x={p.X},y={p.Y} (rgd={CurrentResizeGripDirection}) (resizing={IsResizing})");
#endif
            }
        }

        (ResizeGripDirection rgd,int hx,int hy) CheckRGD(double x,double y)
        {
            var nodr = (ResizeGripDirection.None, 1, 1);
            if (AssociatedObject.ResizeMode == ResizeMode.NoResize)
                return nodr;

            var m = AssociatedObject.Margin;
            var hx = 1; var hy=1; 
            if (InInterval(y, m.Top,ResizeGripEdgeSize))
                hy = 0;
            if (InInterval(y,AssociatedObject.ActualHeight - ResizeGripEdgeSize - m.Bottom, ResizeGripEdgeSize + m.Bottom))
                hy = 2;
            if (InInterval(x, m.Left, ResizeGripEdgeSize))
                hx = 0;
            if (InInterval(x, AssociatedObject.ActualWidth - ResizeGripEdgeSize - m.Right, ResizeGripEdgeSize + m.Right))
                hx = 2;
            if (InInterval(y,m.Top+ResizeGripEdgeSize,AssociatedObject.ActualHeight-m.Top-m.Bottom-2d*ResizeGripEdgeSize))
                hy = 1;
            if (InInterval(x,m.Left+ResizeGripEdgeSize,AssociatedObject.ActualWidth-m.Left-m.Right-2d*ResizeGripEdgeSize))
                hx = 1;
            var dr = _gridDrs[hy, hx];
            
            switch (dr)
            {
                case ResizeGripDirection.Left:
                    if (!CanResizeLeft) return nodr;
                    break;
                case ResizeGripDirection.Top:
                    if (!CanResizeTop) return nodr;
                    break;
                case ResizeGripDirection.Right:
                    if (!CanResizeRight) return nodr;
                    break;
                case ResizeGripDirection.Bottom:
                    if (!CanResizeTop) return nodr;
                    break;
                case ResizeGripDirection.TopLeft:
                    if (!CanResizeTopLeft) return nodr;
                    break;
                case ResizeGripDirection.TopRight:
                    if (!CanResizeTopRight) return nodr;
                    break;
                case ResizeGripDirection.BottomLeft:
                    if (!CanResizeBottomLeft) return nodr;
                    break;
                case ResizeGripDirection.BottomRight:
                    if (!CanResizeBottomRight) return nodr;
                    break;
            }
            return (dr,hx,hy);
        }

        bool InInterval(double v, double min, double length) => (v >= min) && (v <= (min+length));

        public static void DisableResizableBehavior(Window win)
        {
            var o = Interaction.GetBehaviors(win)
                .OfType<ResizableTransparentWindowBehavior>()
                .FirstOrDefault();
            if (o != null)
                o.IsResizable = false;
        }

        public static void EnableResizableBehavior(
            Window win,
            Point? setUpIsMovingFrom = null)
        {
            var o = Interaction.GetBehaviors(win)
                .OfType<ResizableTransparentWindowBehavior>()
                .FirstOrDefault();
            if (o != null)
                o.IsResizable = true;
        }
    }
}

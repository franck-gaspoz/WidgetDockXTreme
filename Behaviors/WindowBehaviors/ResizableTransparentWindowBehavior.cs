﻿//#define dbg

using DesktopPanelTool.Behaviors.WindowBehaviors.DockableBehavior;
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

        DateTime LastDragMoveTime;

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

        ResizeGripDirection CurrentResizeGripDirection = ResizeGripDirection.None;
        double InitialDx = 0;
        double InitialDy = 0;

        ResizeGripDirection[,] GridDrs =
            new ResizeGripDirection[,]
            {
                { ResizeGripDirection.TopLeft , ResizeGripDirection.Top , ResizeGripDirection.TopRight },
                { ResizeGripDirection.Left , ResizeGripDirection.None , ResizeGripDirection.Right },
                { ResizeGripDirection.BottomLeft , ResizeGripDirection.Bottom , ResizeGripDirection.BottomRight }
            };

        Cursor[,] CurDrs =
            new Cursor[,]
            {
                { CursorResizeNWSE , CursorResizeNS , CursorResizeNESW },
                { CursorResizeWE , CursorDefault , CursorResizeWE },
                { CursorResizeNESW , CursorResizeNS , CursorResizeNWSE }
            };

        bool IsResizing = false;

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

            if (CurrentResizeGripDirection != ResizeGripDirection.None
                && e.ChangedButton==MouseButton.Left)
            {
                IsResizing = true;
                var p = e.GetPosition(AssociatedObject);
                InitialDx = InitialDx = 0;
                switch (CurrentResizeGripDirection)
                {
                    case ResizeGripDirection.Right:
                        InitialDx = AssociatedObject.ActualWidth - p.X;
                        break;
                    case ResizeGripDirection.Left:
                        InitialDx = -p.X;
                        break;
                    case ResizeGripDirection.Top:
                        InitialDy = -p.Y;
                        break;
                    case ResizeGripDirection.Bottom:
                        InitialDy = AssociatedObject.ActualHeight - p.Y;
                        break;
                    case ResizeGripDirection.TopRight:
                        InitialDx = AssociatedObject.ActualWidth - p.X;
                        InitialDy = -p.Y;
                        break;
                    case ResizeGripDirection.BottomRight:
                        InitialDx = AssociatedObject.ActualWidth - p.X;
                        InitialDy = AssociatedObject.ActualHeight - p.Y;
                        break;
                    case ResizeGripDirection.BottomLeft:
                        InitialDy = AssociatedObject.ActualHeight - p.Y;
                        InitialDx = -p.X;
                        break;
                    case ResizeGripDirection.TopLeft:
                        InitialDy = -p.Y;
                        InitialDx = -p.X;
                        break;
                }
                DockablePanelWindowBehavior.DisableDockBehavior(AssociatedObject);
                Mouse.Capture(AssociatedObject);
#if dbg
                DesktopPanelTool.Lib.Debug.WriteLine($"is resizing dx={InitialDx},dy={InitialDy} x={p.X},y={p.Y} (rgd={CurrentResizeGripDirection})");
#endif
            }
        }

        private void AssociatedObject_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton==MouseButton.Left)
            {
                CurrentResizeGripDirection = ResizeGripDirection.None;
                if (IsResizing)
                {
                    IsResizing = false;
                    LastDragMoveTime = DateTime.Now;
                    Mouse.Capture(null);
                    AssociatedObject.Cursor = CursorDefault;
                    DockablePanelWindowBehavior.EnableDockBehavior(AssociatedObject);
#if dbg
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
            if (IsResizing)
            {
                var elapsed = DateTime.Now - LastDragMoveTime;
                if (LimitFrameRate &&
                    elapsed.TotalMilliseconds < MaximumMoveEventRate)
                    return;

#if dbg
                DesktopPanelTool.Lib.Debug.WriteLine($"mouse move x={p.X},y={p.Y}");
#endif
                p.X += InitialDx;
                p.Y += InitialDy;

                switch (CurrentResizeGripDirection)
                {
                    case ResizeGripDirection.Left:
                        var nl = AssociatedObject.Left + p.X;
                        var nw = AssociatedObject.ActualWidth - p.X;
                        if (nw>=0 &&
                            nw>AssociatedObject.MinWidth)
                        {
                            AssociatedObject.Left = nl;
                            AssociatedObject.Width = nw;
                        }
                        break;
                    case ResizeGripDirection.Right:
                        if (p.X >= 0)
                            AssociatedObject.Width = p.X;
                        break;
                    case ResizeGripDirection.Top:
                        var nt = AssociatedObject.Top + p.Y;
                        var nh = AssociatedObject.ActualHeight - p.Y;
                        if (nh >= 0 && nh > AssociatedObject.MinHeight)
                        {
                            AssociatedObject.Top = nt;
                            AssociatedObject.Height = nh;
                        }
                        break;
                    case ResizeGripDirection.Bottom:
                        if (p.Y > 0)
                            AssociatedObject.Height = p.Y;
                        break;
                    case ResizeGripDirection.BottomRight:
                        if (p.Y > 0 && p.X>0)
                        {
                            AssociatedObject.Width = p.X;
                            AssociatedObject.Height = p.Y;
                        }
                        break;
                    case ResizeGripDirection.BottomLeft:
                        var nl2 = AssociatedObject.Left + p.X;
                        var nh2 = p.Y;
                        var nw2 = AssociatedObject.ActualWidth - p.X;
                        if (nh2 >= 0 && nw2>0
                            && nw2 > AssociatedObject.MinWidth
                            && nh2 > AssociatedObject.MinHeight)
                        {
                            AssociatedObject.Left = nl2;
                            AssociatedObject.Height = nh2;
                            AssociatedObject.Width = nw2;
                        }
                        break;
                    case ResizeGripDirection.TopRight:
                        var nt2 = AssociatedObject.Top + p.Y;
                        var nh3 = AssociatedObject.ActualHeight - p.Y;
                        var nw3 = p.X;
                        if (nh3 >= 0 && nw3 > 0
                            && nh3 > AssociatedObject.MinHeight
                            && nw3 > AssociatedObject.MinWidth)
                        {
                            AssociatedObject.Top = nt2;
                            AssociatedObject.Height = nh3;
                            AssociatedObject.Width = nw3;
                        }
                        break;
                    case ResizeGripDirection.TopLeft:
                        var nt3 = AssociatedObject.Top + p.Y;
                        var nl3 = AssociatedObject.Left + p.X;
                        var nh4 = AssociatedObject.ActualHeight - p.Y;
                        var nw4 = AssociatedObject.ActualWidth - p.X;
                        if (nh4 > 0 && nw4>=0
                            && nh4 > AssociatedObject.MinHeight
                            && nw4 > AssociatedObject.MinWidth)
                        {
                            AssociatedObject.Top = nt3;
                            AssociatedObject.Left = nl3;
                            AssociatedObject.Height = nh4;
                            AssociatedObject.Width = nw4;
                        }
                        break;
                }

                LastDragMoveTime = DateTime.Now;
            }
            else
            {
                var rgd = CheckRGD(p.X, p.Y);
                AssociatedObject.Cursor = CurDrs[rgd.hy, rgd.hx];
                CurrentResizeGripDirection = rgd.rgd;
#if dbg
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
            if (InInterval(y,AssociatedObject.Height - ResizeGripEdgeSize - m.Bottom, ResizeGripEdgeSize + m.Bottom))
                hy = 2;
            if (InInterval(x, m.Left, ResizeGripEdgeSize))
                hx = 0;
            if (InInterval(x, AssociatedObject.Width - ResizeGripEdgeSize - m.Right, ResizeGripEdgeSize + m.Right))
                hx = 2;
            if /*(y >= ResizeGripEdgeSize &&
                y <= AssociatedObject.Height - ResizeGripEdgeSize)*/
                (InInterval(y,m.Top+ResizeGripEdgeSize,AssociatedObject.Height-m.Top-m.Bottom-2d*ResizeGripEdgeSize))
                hy = 1;
            if (/*x >= ResizeGripEdgeSize &&
                x <= AssociatedObject.Width - ResizeGripEdgeSize)*/
                InInterval(x,m.Left+ResizeGripEdgeSize,AssociatedObject.Width-m.Left-m.Right-2d*ResizeGripEdgeSize))
                hx = 1;
            var dr = GridDrs[hy, hx];
            
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
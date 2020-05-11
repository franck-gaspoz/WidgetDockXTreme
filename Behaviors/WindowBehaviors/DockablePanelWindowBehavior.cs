//#define dbg

using DesktopPanelTool.Behaviors.FrameworkElementBehaviors;
using DesktopPanelTool.Controls;
using DesktopPanelTool.Lib;
using DesktopPanelTool.Lib.Models;
using DesktopPanelTool.Models;
using DesktopPanelTool.ViewModels;
using DesktopPanelTool.Views;
using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using static DesktopPanelTool.Lib.NativeMethods;
using static DesktopPanelTool.Models.NativeTypes;
using w = System.Windows;

namespace DesktopPanelTool.Behaviors.WindowBehaviors
{
    public class DockablePanelWindowBehavior
        : Behavior<Window>
    {
        public bool IsDockableEnabled = true;
        public bool IsDockHitEnabled = true;

        public static double ScreenMouseSensitiveAreaEdgeSize = 10;
        public static int ScreenMouseSensitiveTriggerScrutationDelay = 100;

        public static double ScreenDockingAreaEdgeSize = 8;
        public static double UndockingMouseMoveDistance = 32;
        public static double UndockedMousePaddingLeft = 16;
        public static double UndockedMousePaddingTop = 16;

        double _ScreenMouseSensitiveAreaTriggerDelay;

        /// <summary>
        /// TODO: not used!
        /// </summary>
        public double ScreenMouseSensitiveAreaTriggerDelay
        {
            get { return (double)GetValue(ScreenMouseSensitiveAreaTriggerDelayProperty); }
            set { SetValue(ScreenMouseSensitiveAreaTriggerDelayProperty, value); }
        }

        public static readonly DependencyProperty ScreenMouseSensitiveAreaTriggerDelayProperty =
            DependencyProperty.Register("ScreenMouseSensitiveAreaTriggerDelay", typeof(double), typeof(DockablePanelWindowBehavior), new PropertyMetadata(800d));

        public bool AutoHideDocked
        {
            get { return (bool)GetValue(AutoHideDockedProperty); }
            set { SetValue(AutoHideDockedProperty, value); }
        }

        public static readonly DependencyProperty AutoHideDockedProperty =
            DependencyProperty.Register("AutoHideDocked", typeof(bool), typeof(DockablePanelWindowBehavior), new PropertyMetadata(true));

        public bool HideDockedWhenDeactivated
        {
            get { return (bool)GetValue(HideDockedWhenDeactivatedProperty); }
            set { SetValue(HideDockedWhenDeactivatedProperty, value); }
        }

        public static readonly DependencyProperty HideDockedWhenDeactivatedProperty =
            DependencyProperty.Register("HideDockedWhenDeactivated", typeof(bool), typeof(DockablePanelWindowBehavior), new PropertyMetadata(true));

        public double AutoHideDelay
        {
            get { return (double)GetValue(AutoHideDelayProperty); }
            set { SetValue(AutoHideDelayProperty, value); }
        }

        public static readonly DependencyProperty AutoHideDelayProperty =
            DependencyProperty.Register("AutoHideDelay", typeof(double), typeof(DockablePanelWindowBehavior), new PropertyMetadata(1000d));

        public double HideAnimationDuration
        {
            get { return (double)GetValue(HideAnimationDurationProperty); }
            set { SetValue(HideAnimationDurationProperty, value); }
        }

        public static readonly DependencyProperty HideAnimationDurationProperty =
            DependencyProperty.Register("HideAnimationDuration", typeof(double), typeof(DockablePanelWindowBehavior), new PropertyMetadata(250d));

        public double ShowAnimationDuration
        {
            get { return (double)GetValue(ShowAnimationDurationProperty); }
            set { SetValue(ShowAnimationDurationProperty, value); }
        }

        public static readonly DependencyProperty ShowAnimationDurationProperty =
            DependencyProperty.Register("ShowAnimationDuration", typeof(double), typeof(DockablePanelWindowBehavior), new PropertyMetadata(200d));

        public double ShowHitAnimationDuration
        {
            get { return (double)GetValue(ShowHitAnimationDurationProperty); }
            set { SetValue(ShowHitAnimationDurationProperty, value); }
        }

        public static readonly DependencyProperty ShowHitAnimationDurationProperty =
            DependencyProperty.Register("ShowHitAnimationDuration", typeof(double), typeof(DockablePanelWindowBehavior), new PropertyMetadata(200d));

        public double HideHitAnimationDuration
        {
            get { return (double)GetValue(HideHitAnimationDurationProperty); }
            set { SetValue(HideHitAnimationDurationProperty, value); }
        }

        public static readonly DependencyProperty HideHitAnimationDurationProperty =
            DependencyProperty.Register("HideHitAnimationDuration", typeof(double), typeof(DockablePanelWindowBehavior), new PropertyMetadata(150d));

        public IDesktopPanelBaseViewModel ViewModel
        {
            get { return (IDesktopPanelBaseViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(IDesktopPanelBaseViewModel), typeof(DockablePanelWindowBehavior), new PropertyMetadata(null));

        public ScreenInfo DockScreen { get; protected set; }
        public DockName Dock { get; protected set; } = DockName.None;

        public List<DockName> AllowedDocks
            = new List<DockName> { DockName.Left, DockName.Right, DockName.Top, DockName.Bottom };

        DockName DockingDock;
        ScreenInfo DockingScreen;
        public bool IsDocked { get; protected set; } = false;
        public bool IsPined { get; protected set; } = false;
        bool IsDockable = false;
        bool IsUndocking = false;
        public bool IsHidden { get; protected set; } = false;
        bool IsWaitingMouseSensitiveTrigger = false;
        private Point InitialUndockingPosition;
        List<ScreenInfo> ScreenInfos;
        ResizeMode ResizeModeBackup;
        double WidthBackup;
        double HeightBackup;
        double MinWidthBackup;
        double MinHeightBackup;
        w.Rect DockedSensitiveArea;

        DockHit DockHit;

        public DockablePanelWindowBehavior()
        {
            ScreenInfos = DisplayDevices.GetScreensInfos();
#if alldbg || dbg
            foreach (var scr in ScreenInfos)
                DesktopPanelTool.Lib.Debug.WriteLine(scr);            
#endif
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            _ScreenMouseSensitiveAreaTriggerDelay = ScreenMouseSensitiveAreaTriggerDelay;
            HideHitAnimationDuration = (double)AssociatedObject.FindResource("HideHitAnimationDuration");
            ShowHitAnimationDuration = (double)AssociatedObject.FindResource("ShowHitAnimationDuration");
            AssociatedObject.MouseMove += AssociatedObject_MouseMove;
            AssociatedObject.MouseUp += AssociatedObject_MouseUp;
            AssociatedObject.MouseDown += AssociatedObject_MouseDown;
            AssociatedObject.Deactivated += AssociatedObject_Deactivated;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.MouseMove -= AssociatedObject_MouseMove;
            AssociatedObject.MouseUp -= AssociatedObject_MouseUp;
            AssociatedObject.MouseDown -= AssociatedObject_MouseDown;
            AssociatedObject.Deactivated -= AssociatedObject_Deactivated;
        }

        private void AssociatedObject_Deactivated(object sender, EventArgs e)
        {
            if (!IsHidden && IsDocked && HideDockedWhenDeactivated)
            {
                CollapsePanel();
            }
        }

        public void CollapsePanel()
        {
            if (IsPined)
                return;

            var dockDir = GetDockDirection(Dock);
            if (dockDir != DockDirection.None)
            {
                IsHidden = true;
                var area = DockScreen.MonitorArea;
                var mustCollapseExpandWithMove = MustCollapseExpandWithMove(Dock);
                DockedSensitiveArea = new w.Rect(
                    Dock == DockName.Right ? area.Right - ScreenMouseSensitiveAreaEdgeSize : AssociatedObject.Left,
                    Dock == DockName.Bottom ? area.Bottom - ScreenMouseSensitiveAreaEdgeSize : AssociatedObject.Top,
                    dockDir == DockDirection.Horizontal ? ScreenMouseSensitiveAreaEdgeSize : AssociatedObject.Width,
                    dockDir == DockDirection.Vertical ? ScreenMouseSensitiveAreaEdgeSize : AssociatedObject.Height
                    );

                var dp = dockDir == DockDirection.Horizontal ? FrameworkElement.WidthProperty : FrameworkElement.HeightProperty;
                var finalValue = 0d;

                MinWidthBackup = AssociatedObject.MinWidth;
                MinHeightBackup = AssociatedObject.MinHeight;
                AssociatedObject.MinWidth = 0;
                AssociatedObject.MinHeight = 0;

                var an = new DoubleAnimation(
                    dockDir == DockDirection.Horizontal ? AssociatedObject.Width : AssociatedObject.Height,
                    finalValue,
                    new Duration(TimeSpan.FromMilliseconds(HideAnimationDuration)),
                    FillBehavior.Stop
                    );
                an.Completed += (o, e) =>
                {
                    AssociatedObject.SetValue(dp, finalValue);
                    AssociatedObject.Hide();
                    var t = new Thread(ScreenMouseSensitiveAreaWatcher);
                    t.Start();
                };

                DoubleAnimation an2=null;
                DependencyProperty dp2 = null;
                if (mustCollapseExpandWithMove)
                {
                    double finalValue2 = dockDir == DockDirection.Horizontal ? area.Right : area.Bottom;
                    dp2 = dockDir == DockDirection.Horizontal ? Window.LeftProperty : Window.TopProperty;
                    an2 = new DoubleAnimation(
                        dockDir == DockDirection.Horizontal ? AssociatedObject.Left : AssociatedObject.Top,
                        finalValue2,
                        new Duration(TimeSpan.FromMilliseconds(HideAnimationDuration)),
                        FillBehavior.Stop
                        );
                    an2.Completed += (o, e) =>
                    {
                        AssociatedObject.SetValue(dp2, finalValue2);
                    };
                }

                if (an2!=null)
                    AssociatedObject.BeginAnimation( dp2, an2 );
                AssociatedObject.BeginAnimation(dp, an);

                ViewModel?.NotifyPropertyUpdated(nameof(ViewModel.IsCollapsed));
            }
        }

        public void ExpandPanel()
        {
            var dockDir = GetDockDirection(Dock);
            if (dockDir != DockDirection.None)
            {
                IsHidden = false;
                ViewModel?.NotifyPropertyUpdated(nameof(ViewModel.IsCollapsed));
                var area = DockScreen.MonitorArea;
                var mustCollapseExpandWithMove = MustCollapseExpandWithMove(Dock);

                var dp = dockDir == DockDirection.Horizontal ? FrameworkElement.WidthProperty : FrameworkElement.HeightProperty;
                var finalValue = dockDir == DockDirection.Horizontal ? MinWidthBackup : MinHeightBackup;

                var an = new DoubleAnimation(
                    0,
                    finalValue,
                    new Duration(TimeSpan.FromMilliseconds(ShowAnimationDuration)),
                    FillBehavior.Stop
                    );

                an.Completed += (o,e) =>
                {
                    AssociatedObject.SetValue(dp, finalValue);
                    AssociatedObject.MinWidth = MinWidthBackup;
                    AssociatedObject.MinHeight = MinHeightBackup;
                    AssociatedObject.Activate();
                };

                DoubleAnimation an2 = null;
                DependencyProperty dp2 = null;
                if (mustCollapseExpandWithMove)
                {
                    double originalValue2 = dockDir == DockDirection.Horizontal ? area.Right : area.Bottom;
                    double finalValue2 = dockDir == DockDirection.Horizontal ? area.Right-MinWidthBackup : area.Bottom-MinHeightBackup;
                    dp2 = dockDir == DockDirection.Horizontal ? Window.LeftProperty : Window.TopProperty;
                    an2 = new DoubleAnimation(
                        originalValue2,
                        finalValue2,
                        new Duration(TimeSpan.FromMilliseconds(HideAnimationDuration)),
                        FillBehavior.Stop
                        );
                    an2.Completed += (o, e) =>
                    {
                        AssociatedObject.SetValue(dp2, finalValue2);
                    };
                }

                AssociatedObject.Show();
                if (an2 != null)
                    AssociatedObject.BeginAnimation(dp2, an2);
                AssociatedObject.BeginAnimation(dp, an);
                AssociatedObject.Activate();
            }
        }

        bool MustCollapseExpandWithMove(DockName dock)
        {
            return (dock == DockName.Right || dock == DockName.Bottom);
        }

        public DockDirection GetDockDirection(DockName dock)
        {
            switch (dock)
            {
                case DockName.Left:
                case DockName.Right:
                    return DockDirection.Horizontal;
                case DockName.Top:
                case DockName.Bottom:
                    return DockDirection.Vertical;
                case DockName.None:
                    return DockDirection.None;
            }
            return DockDirection.None;
        }

        bool CheckMouseInSensitiveArea(double x,double y)
        {
            return DockedSensitiveArea.Contains(new Point(x, y));
        }

        void ScreenMouseSensitiveAreaWatcher()
        {
            try
            {
                while (IsHidden)
                {
                    if (!IsWaitingMouseSensitiveTrigger)
                    {
                        POINT p = new POINT();
                        GetCursorPos(ref p);
                        if (CheckMouseInSensitiveArea(p.X, p.Y))
                        {
                            IsWaitingMouseSensitiveTrigger = true;
                            var t = new Thread(WaitMouseSensitiveTrigger);
                            t.Start();
                            t.Join();
                        }
                    }

                    if (!IsHidden)
                        break;

                    Thread.Sleep(ScreenMouseSensitiveTriggerScrutationDelay);
                }
            }
            catch (ThreadInterruptedException) { }
        }

        void WaitMouseSensitiveTrigger()
        {
            try
            {
                var beginTime = DateTime.Now;
                var outArea = false;
                while ((DateTime.Now - beginTime) <= TimeSpan.FromMilliseconds(_ScreenMouseSensitiveAreaTriggerDelay)
                    && !outArea)
                {
                    POINT p = new POINT();
                    GetCursorPos(ref p);
                    outArea = !CheckMouseInSensitiveArea(p.X, p.Y);
                    if (outArea)
                        break;

                    Thread.Sleep(ScreenMouseSensitiveTriggerScrutationDelay);
                }
                if (!outArea)
                {
                    var elapsed = DateTime.Now - beginTime;
#if alldbg || dbg
                DesktopPanelTool.Lib.Debug.WriteLine($"elapsed={elapsed.TotalMilliseconds} ms (ScreenMouseSensitiveAreaTriggerDelay={_ScreenMouseSensitiveAreaTriggerDelay})");
#endif
                    IsHidden = false;
                    Dispatcher.Invoke(ExpandPanel);
                }
                IsWaitingMouseSensitiveTrigger = false;
            }
            catch (ThreadInterruptedException) {
                IsWaitingMouseSensitiveTrigger = false;
            }
        }

        private void AssociatedObject_MouseMove(
            object sender, 
            MouseEventArgs e)
        {
            POINT p = new POINT();
            GetCursorPos(ref p);

            if (e.LeftButton != MouseButtonState.Pressed)
                return;

            if (!IsDocked && !IsUndocking)
                CheckIsDocking(p);
#if alldbg || dbg
            DesktopPanelTool.Lib.Debug.WriteLine($"dock={DockingDock} isDockable={IsDockable} isDocked={IsDocked} isUndocking={IsUndocking}");
#endif

            if (IsDockHitEnabled)
            {
                if (DockingDock != DockName.None && IsDockable)
                    ShowDockHit(new Point(p.X, p.Y));
                else
                    HideDockHit();
            }

            if (IsUndocking)
            {
                var dst = Math.Sqrt(
                    Math.Pow(p.X-InitialUndockingPosition.X,2) +
                    Math.Pow(p.Y-InitialUndockingPosition.Y,2)
                    );
                if (dst >= UndockingMouseMoveDistance)
                {
                    EndUndocking();
                    UnDockWindow();
                }
            }
        }

        bool IsDockHitVisible = false;

        void ShowDockHit(Point p)
        {
            if (DockHit == null)
                DockHit = new DockHit();
            var left = p.X - DockHit.Width / 2;
            var top = p.Y - DockHit.Height / 2;

            if (IsDockHitVisible)
            {
                DockHit.Left = left;
                DockHit.Top = top;
                return;
            }

            IsDockHitVisible = true;
            
            //var (x, y, w, h) = GetDockedBounds(DockingDock, DockingScreen);
            
            DockHit.GR.Opacity = 0;
            DockHit.Show();
            var an =
                new DoubleAnimation(
                    0,
                    1, 
                    new Duration(TimeSpan.FromMilliseconds(ShowHitAnimationDuration))
                    );
            /*an.EasingFunction = new BounceEase
            {
                Bounces=2,
                Bounciness=2,
                EasingMode=EasingMode.EaseOut
            };*/
            DockHit.GR.BeginAnimation(
                FrameworkElement.OpacityProperty, 
                an);
        }

        void HideDockHit()
        {
            if (IsDockHitVisible)
            {
                IsDockHitVisible = false;
                var an =
                new DoubleAnimation(
                    1,
                    0,
                    new Duration(TimeSpan.FromMilliseconds(HideHitAnimationDuration))
                    );
                an.Completed += (o, e) =>
                {
                    if (!IsDockHitVisible)
                        DockHit.Hide();
                };
                DockHit.GR.BeginAnimation(
                    FrameworkElement.OpacityProperty,
                    an);
            }
        }

        private void AssociatedObject_MouseDown(object sender, MouseButtonEventArgs e)
        {
#if alldbg || dbg
            DesktopPanelTool.Lib.Debug.WriteLine($"mouse down");
#endif
            if (IsDocked && e.ChangedButton==MouseButton.Left && !IsPined && AcceptableUndockingStartPoint())
            {
#if alldbg || dbg
                DesktopPanelTool.Lib.Debug.WriteLine($"start undocking");
#endif
                DraggableFrameworkElementBehavior.SetIsEnabledDraggableDescendants(AssociatedObject, typeof(WidgetControl), false);
                IsUndocking = true;
                POINT p = new POINT();
                GetCursorPos(ref p);
                InitialUndockingPosition = new Point(p.X, p.Y);
                Mouse.Capture(AssociatedObject);
            }
        }

        bool AcceptableUndockingStartPoint()
        {
#if alldbg || dbg
                DesktopPanelTool.Lib.Debug.WriteLine($"acceptable start point: has parent={WPFHelper.HasParent<WidgetControl>(Mouse.DirectlyOver as DependencyObject)}");
#endif
            return !WPFHelper.HasParent<WidgetControl>(Mouse.DirectlyOver as DependencyObject);
        }

        void EndUndocking()
        {
            IsUndocking = false;
            Mouse.Capture(null);
        }

        void CheckIsDocking(POINT p)
        {
            if (!IsDockableEnabled)
            {
                DockingScreen = null;
                IsDockable = false;
                return;
            }

            var (dock, scr) = GetDockInfos(p);

            if (!IsDocked && (AllowedDocks.Contains(dock) || dock==DockName.None))
            {
                DockingDock = dock;
                if (dock != DockName.None)
                {
                    IsDockable = true;
                    DockingScreen = scr;                    
                } else
                {
                    DockingScreen = null;
                    IsDockable = false;
                }
            }
        }

        // TODO: change by left mouse up or check left button
        private void AssociatedObject_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!IsDockableEnabled)
                return;

            if (IsDockable)
                AttachToDock(DockingDock,DockingScreen);
            if (IsUndocking)
            {
                EndUndocking();
                DraggableFrameworkElementBehavior.SetIsEnabledDraggableDescendants(AssociatedObject, typeof(WidgetControl), true);
            }
        }

        (DockName dock,ScreenInfo scr) GetDockInfos(POINT p)
        {
            if (IsDocked)
                return (DockingDock, DockingScreen);

            var dockingDock = DockName.None;
            foreach (var scr in ScreenInfos)
            {                
                var area = scr.MonitorArea;
                if (p.X >= area.Left && 
                    p.Y >= area.Top && 
                    p.X <= area.Right && 
                    p.Y <= area.Bottom )
                {
#if alldbg || dbg
                    DesktopPanelTool.Lib.Debug.WriteLine($"x={p.X},y={p.Y} area={area}");
#endif
                    if (p.X >= area.Left && p.X < area.Left+ScreenDockingAreaEdgeSize)
                        dockingDock = DockName.Left;
                    if (p.X <= area.Right &&
                        p.X > area.Right - ScreenDockingAreaEdgeSize )
                        dockingDock = DockName.Right;
                    if (p.Y >= area.Top &&
                        p.Y < area.Top + ScreenDockingAreaEdgeSize )
                        dockingDock = DockName.Top;
                    if (p.Y <= area.Bottom
                        && p.Y > area.Bottom - ScreenDockingAreaEdgeSize )
                        dockingDock = DockName.Bottom;

                    if (dockingDock != DockName.None)
                        return (dockingDock, scr);
                }
            }
            return (DockName.None, null);
        }

        public void TogglePin()
        {
            IsPined = !IsPined;
            ViewModel?.NotifyPropertyUpdated(nameof(ViewModel.IsPined));
        }

        public void Pin()
        {
            if (!IsPined)
            {
                IsPined = !IsPined;
                ViewModel?.NotifyPropertyUpdated(nameof(ViewModel.IsPined));
            }
        }

        public void UnPin()
        {
            if (IsPined)
            {
                IsPined = !IsPined;
                ViewModel?.NotifyPropertyUpdated(nameof(ViewModel.IsPined));
            }
        }

        public void AttachToDock(DockName dock,ScreenInfo dockScreen)
        {
            if (IsDockHitEnabled)
                HideDockHit();
            IsDocked = true;
            IsDockable = false;
#if alldbg || dbg
            DesktopPanelTool.Lib.Debug.WriteLine($"attach to dock: dock={DockingDock} scr={DockingScreen}");
#endif
            DockWindow(dock, dockScreen);
        }

        (double x,double y,double w,double h) GetDockedBounds(DockName dock, ScreenInfo screen)
        {
            var area = screen.MonitorArea;
            double x = 0, y = 0, w = 0, h = 0;
            switch (dock)
            {
                case DockName.Left:
                    x = area.Left;
                    y = area.Top;
                    h = area.Height;
                    w = AssociatedObject.MinWidth;
                    break;
                case DockName.Right:
                    x = area.Right - AssociatedObject.MinWidth;
                    y = area.Top;
                    h = area.Height;
                    w = AssociatedObject.MinWidth;
                    break;
                case DockName.Top:
                    x = area.Left;
                    y = area.Top;
                    w = area.Width;
                    h = AssociatedObject.MinHeight;
                    break;
                case DockName.Bottom:
                    x = area.Left;
                    y = area.Bottom - AssociatedObject.MinHeight;
                    w = area.Width;
                    h = AssociatedObject.MinHeight;
                    break;
            }
            return (x, y, w, h);
        }

        void DockWindow(DockName dock,ScreenInfo screen)
        { 
            MovableTransparentWindowBehavior.DisableMovableBehavior(AssociatedObject);
            ResizeModeBackup = AssociatedObject.ResizeMode;
            WidthBackup = AssociatedObject.Width;
            HeightBackup = AssociatedObject.Height;
            AssociatedObject.ResizeMode = ResizeMode.NoResize;
            Dock = dock;
            DockScreen = screen;

            var (x, y, w, h) = GetDockedBounds(dock, screen);

            AssociatedObject.Left = x;
            AssociatedObject.Top = y;
            AssociatedObject.Width = w;
            AssociatedObject.Height = h;

            ViewModel?.NotifyPropertyUpdated(nameof(ViewModel.IsDocked));
        }

        public void UnDockWindow()
        {
            AssociatedObject.ResizeMode = ResizeModeBackup;
            AssociatedObject.Width = WidthBackup;
            AssociatedObject.Height = HeightBackup;
            AutoLocateRelatingToCursor();

            Dispatcher.BeginInvoke(() => { }, DispatcherPriority.ApplicationIdle).Wait();
            Dock = DockName.None;
            DockScreen = null;
            IsDocked = false;
            MovableTransparentWindowBehavior.EnableMovableBehavior(
                AssociatedObject,
                Mouse.GetPosition(AssociatedObject));

            ViewModel?.NotifyPropertyUpdated(nameof(ViewModel.IsDocked));
        }

        void AutoLocateRelatingToCursor()
        {
            POINT p = new POINT();
            GetCursorPos(ref p);

            /*
            var x = p.X - UndockedMousePaddingLeft;
            var y = p.Y - UndockedMousePaddingTop;
            */

            var x = p.X - AssociatedObject.Width / 2;
            var y = p.Y - AssociatedObject.Height / 2;

            /*
            var area = DockScreen.MonitorArea;
            var monrect = new RECT(area.Left, area.Top, area.Left + area.Width - 1, area.Top + Heigh - 1);
            var winrect = new RECT((int)AssociatedObject.Left, (int)AssociatedObject.Top, (int)(AssociatedObject.Left + AssociatedObject.Width - 1), (int)(AssociatedObject.Top + AssociatedObject.Height - 1));
            var intrsrect = new RECT();
            IntersectRect(ref intrsrect, ref monrect, ref winrect);
            if (intrsrect.IsEmpty)
            {
                // ...
            }
            */
            AssociatedObject.Left = x;
            AssociatedObject.Top = y;
        }

        public static void DisableDockBehavior(Window win)
        {
            var o = Interaction.GetBehaviors(win)
                .OfType<DockablePanelWindowBehavior>()
                .FirstOrDefault();
            if (o != null)
                o.IsDockableEnabled = false;
        }

        public static void EnableDockBehavior(
            Window win,
            Point? setUpIsMovingFrom = null)
        {
            var o = Interaction.GetBehaviors(win)
                .OfType<DockablePanelWindowBehavior>()
                .FirstOrDefault();
            if (o != null)
                o.IsDockableEnabled = true;
        }
    }
}

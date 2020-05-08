using Microsoft.Xaml.Behaviors;
using System;
using System.Windows;
using System.Windows.Interop;
using static DesktopPanelTool.Lib.NativeMethods;

namespace DesktopPanelTool.Behaviors.WindowBehaviors
{
    public class WindowBehindWindowBehavior
        : Behavior<Window>
    {
        public Window ShadowLayer { get; protected set; }
        double _dx = 16;
        double _dy = 16;
        IntPtr _blHdle;
        IntPtr _aoHdle;

        public Type LayerWindowType
        {
            get { return (Type)GetValue(LayerWindowTypeProperty); }
            set { SetValue(LayerWindowTypeProperty, value); }
        }

        public static readonly DependencyProperty LayerWindowTypeProperty =
            DependencyProperty.Register("LayerWindowType", typeof(Type), typeof(WindowBehindWindowBehavior), new PropertyMetadata(null));

        public bool IsEnabled
        {
            get { return (bool)GetValue(IsEnabledProperty); }
            set { SetValue(IsEnabledProperty, value); }
        }

        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.Register("IsEnabled", typeof(bool), typeof(WindowBehindWindowBehavior), new PropertyMetadata(true));

        protected override void OnAttached()
        {
            base.OnAttached();
            ShadowLayer = (Window)Activator.CreateInstance(LayerWindowType);
            AssociatedObject.SizeChanged += AssociatedObject_SizeChanged;
            AssociatedObject.LocationChanged += AssociatedObject_LocationChanged;
            AssociatedObject.IsVisibleChanged += AssociatedObject_IsVisibleChanged;
            AssociatedObject.Activated += AssociatedObject_Activated;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.SizeChanged -= AssociatedObject_SizeChanged;
            AssociatedObject.LocationChanged -= AssociatedObject_LocationChanged;
            AssociatedObject.IsVisibleChanged -= AssociatedObject_IsVisibleChanged;
            AssociatedObject.Activated -= AssociatedObject_Activated;
        }

        private void AssociatedObject_Activated(object sender, EventArgs e)
        {
            GetHandles();
            SetZOrder();
        }

        void GetHandles()
        {
            _blHdle = new WindowInteropHelper(ShadowLayer).Handle;
            _aoHdle = new WindowInteropHelper(AssociatedObject).Handle;
        }

        private void AssociatedObject_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!IsEnabled) return;
            if (!AssociatedObject.IsVisible)
            {
                ShadowLayer.Hide();
            }
            else
            {
                SetZOrder();
                ShadowLayer.Show();
            }
        }

        public void SetZOrder(int? x=null,int? y=null)
        {
#if NO
            if (_blHdle == (IntPtr)0) GetHandles();

            var flg = SetWindowPosFlags.DoNotActivate
                | SetWindowPosFlags.DoNotSendChangingEvent
                | SetWindowPosFlags.IgnoreResize
                /*| SetWindowPosFlags.IgnoreMove*/;
            if (!x.HasValue && !y.HasValue)
                flg |= SetWindowPosFlags.DoNotReposition;
            var xv = x.HasValue ? x.Value : 0;
            var yv = y.HasValue ? y.Value : 0;

            SetWindowPos(_blHdle,
                (IntPtr)SpecialWindowHandles.HWND_TOP,
                xv, yv, 0, 0,flg
                );

            SetWindowPos(_aoHdle,
                (IntPtr)SpecialWindowHandles.HWND_TOP,
                xv, yv, 0, 0,flg | SetWindowPosFlags.DoNotReposition);

            return;
#endif

            ShadowLayer.Topmost = true;
            AssociatedObject.Topmost = false;

            //ShadowLayer.Topmost = true;
            AssociatedObject.Topmost = true;
        }

        private void AssociatedObject_LocationChanged(object sender, EventArgs e)
        {
            if (!IsEnabled) return;
            ShadowLayer.Left = AssociatedObject.Left-_dx;
            ShadowLayer.Top = AssociatedObject.Top-_dy;
        }

        private void AssociatedObject_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!IsEnabled) return;
            ShadowLayer.Width = AssociatedObject.ActualWidth + 2d*_dx;
            ShadowLayer.Height = AssociatedObject.ActualHeight + 2d*_dy;
        }
    }
}

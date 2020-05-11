//#define dbg

using DesktopPanelTool.Lib;
using Microsoft.Xaml.Behaviors;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using static DesktopPanelTool.Lib.NativeMethods;
using static DesktopPanelTool.Models.NativeTypes;
using static DesktopPanelTool.Models.WindowMessages;

namespace DesktopPanelTool.Behaviors.WindowBehaviors
{
    public class ClickOutsideHideOrCloseWindowBehavior
        : Behavior<Window>
    {
        double _left, _top, _right, _bottom;
        bool _canMove;
        HookProc _callbackDelegate = null;
        IntPtr _hookHandle;

        public bool IsEnabled = true; 

        public bool CloseOnClick
        {
            get { return (bool)GetValue(CloseOnClickProperty); }
            set { SetValue(CloseOnClickProperty, value); }
        }

        public static readonly DependencyProperty CloseOnClickProperty =
            DependencyProperty.Register("CloseOnClick", typeof(bool), typeof(ClickOutsideHideOrCloseWindowBehavior), new PropertyMetadata(false));

        public bool CanMove
        {
            get { return (bool)GetValue(CanMoveProperty); }
            set { SetValue(CanMoveProperty, value); }
        }

        public static readonly DependencyProperty CanMoveProperty =
            DependencyProperty.Register("CanMove", typeof(bool), typeof(ClickOutsideHideOrCloseWindowBehavior), new PropertyMetadata(false));

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.IsVisibleChanged += AssociatedObject_IsVisibleChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.IsVisibleChanged -= AssociatedObject_IsVisibleChanged;
        }

        private void AssociatedObject_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (AssociatedObject.IsVisible)
            {
#if alldbg || dbg
                DesktopPanelTool.Lib.Debug.WriteLine($"is visible");
#endif
                GetWindowCoordinates();
                _canMove = CanMove;

                _callbackDelegate = new HookProc(CallbackFunction);
                _hookHandle = (IntPtr)SetWindowsHookEx(
                    HookType.WH_MOUSE_LL,
                    _callbackDelegate,
                    IntPtr.Zero,
                    0);
            } 
            else
                UnhookWindowsHookEx(_hookHandle);
        }

        private int CallbackFunction(int code, IntPtr wParam, IntPtr lParam)
        {
            if (code < 0)
                return CallNextHookEx(IntPtr.Zero, code, wParam, lParam);

            var r = wParam.ToInt32();
#if false && dbg
            DesktopPanelTool.Lib.Debug.WriteLine($"{wParam}");
#endif

            if ((r== WM_LBUTTONDOWN || r==WM_RBUTTONDOWN)
                && IsEnabled
                && CheckMouseOutWindow())
            {
#if alldbg || dbg
                DesktopPanelTool.Lib.Debug.WriteLine($"wParam={wParam} lParam={lParam}");
#endif
                UnhookWindowsHookEx(_hookHandle);
                if (CloseOnClick)
                    AssociatedObject.Close();
                else
                    AssociatedObject.Hide();
            }

            return CallNextHookEx(IntPtr.Zero, code, wParam, lParam);
        }

        bool CheckMouseOutWindow()
        {
#if alldbg || dbg
            DesktopPanelTool.Lib.Debug.WriteLine($"mouse directlyOver={Mouse.DirectlyOver}");
#endif
            if (Mouse.DirectlyOver is DependencyObject o && o != null)
            {
                var ancestor = WPFHelper.FindLogicalAncestor(o);
#if alldbg || dbg
                DesktopPanelTool.Lib.Debug.WriteLine($"ancestor = {ancestor}");
#endif
                if ( !( ancestor is Window w && AssociatedObject!=w) )
                    return false;   
            }

            POINT p = new POINT();
            GetCursorPos(ref p);
            var x = p.X;
            var y = p.Y;

            if (_canMove)
                GetWindowCoordinates();

            return (x < _left || x > _right || y <= _top || y >= _bottom);
        }

        void GetWindowCoordinates()
        {
            _left = AssociatedObject.Left;
            _top = AssociatedObject.Top;
            _right = AssociatedObject.Left + AssociatedObject.Width - 1;
            _bottom = AssociatedObject.Top + AssociatedObject.Height - 1;
        }

        public static void SetIsEnabled(Window window,bool isEnabled)
        {
            if (window == null) return;
            var o = Interaction.GetBehaviors(window)
                .OfType<ClickOutsideHideOrCloseWindowBehavior>()
                .FirstOrDefault();
            if (o != null)
                o.IsEnabled = isEnabled;
        }
    }
}

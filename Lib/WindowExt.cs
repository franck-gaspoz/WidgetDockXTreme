using System;
using System.Windows;
using System.Windows.Interop;
using static DesktopPanelTool.Lib.NativeMethods;

namespace DesktopPanelTool.Lib
{
    internal static class WindowExt
    {
        internal static bool SetPos(this Window win,double x,double y)
        {
            int ix = (int)x;
            int iy = (int)y;
            return SetWindowPos(new WindowInteropHelper(win).Handle, ix, iy);
        }

        internal static bool SetPos(this Window win, double x, double y,IntPtr z)
        {
            int ix = (int)x;
            int iy = (int)y;
            return SetWindowPos(new WindowInteropHelper(win).Handle, ix, iy,z);
        }
    }
}

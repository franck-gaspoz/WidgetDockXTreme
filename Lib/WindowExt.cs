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

        internal static bool SetPosAndSize(this Window win, double x, double y, double width, double height )
        {
            int ix = (int)x;
            int iy = (int)y;
            int iw = (int)width;
            int ih = (int)height;
            return SetWindowPosAndSize(new WindowInteropHelper(win).Handle, ix, iy, iw, ih);
        }

        internal static bool SetPosAndSize(this Window win, double x, double y, IntPtr z, double width, double height)
        {
            int ix = (int)x;
            int iy = (int)y;
            int iw = (int)width;
            int ih = (int)height;
            return SetWindowPosAndSize(new WindowInteropHelper(win).Handle, ix, iy, z, iw, ih);
        }
    }
}

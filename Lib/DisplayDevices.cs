using System;
using System.Collections.Generic;
using static DesktopPanelTool.Models.NativeTypes;
using static DesktopPanelTool.Lib.NativeMethods;
using System.Runtime.InteropServices;
using DesktopPanelTool.Models;

namespace DesktopPanelTool.Lib
{
    internal static class DisplayDevices
    {
        internal static List<ScreenInfo> GetScreensInfos()
        {
            var scrInfos = new List<ScreenInfo>();
            EnumDisplayMonitors(
                    IntPtr.Zero,
                    IntPtr.Zero,
                    delegate (
                        IntPtr hMonitor,
                        IntPtr hdcMonitor,
                        ref Rect lprcMonitor,
                        IntPtr dwData)
                    {
                        var mi = new MonitorInfoEx();
                        mi.Size = (int)Marshal.SizeOf(mi);
                        bool success = GetMonitorInfo(hMonitor, ref mi);
                        if (success)
                        {
                            var di = new ScreenInfo
                            {
                                Width = (mi.Monitor.Right - mi.Monitor.Left),
                                Height = (mi.Monitor.Bottom - mi.Monitor.Top),
                                IsPrimary = mi.Flags == MONITORINFOF_PRIMARY,
                                DeviceName = mi.DeviceName
                            };
                            di.MonitorArea = Area.FromRect(di, mi.Monitor);
                            di.WorkArea = Area.FromRect(di, mi.WorkArea);
                            scrInfos.Add(di);
                        }
                        return true;
                    }, IntPtr.Zero);
            return scrInfos;
        }

        internal static ScreenInfo GetCurrentScreenInfo(double x,double y)
        {
            foreach (var scrInfo in GetScreensInfos())
                if (scrInfo.MonitorArea.ContainsPoint(x, y))
                    return scrInfo;
            return null;
        }
    }
}

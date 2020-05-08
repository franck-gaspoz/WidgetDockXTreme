//#define dbg

using DesktopPanelTool.Models;
using DesktopPanelTool.Views;
using System.Windows.Forms;
using static DesktopPanelTool.Lib.WPFHelper;
using static DesktopPanelTool.Models.NativeTypes;
using static DesktopPanelTool.Lib.NativeMethods;
using DesktopPanelTool.Lib;
using DesktopPanelTool.ViewModels;

namespace DesktopPanelTool.Services
{
    public static class NotificationBarService
    {
        static NotifyIcon _notifyIcon;
        static string _toolTipTitle;
        static NotifyIconContextMenu _contextMenu;
        public static bool IsContextMenuOpened => _contextMenu.IsVisible;
        static DesktopPanelToolViewModel _toolViewModel;

        internal static void Initialize(DesktopPanelToolViewModel toolViewModel)
        {
            _toolViewModel = toolViewModel;
            _toolTipTitle = $"{AppSettings.AppTitle} {AppSettings.AppVersionExternal}";

            /*var about = new ToolStripMenuItem("About...");
            var exit = new ToolStripMenuItem("Exit...");
            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add(about);
            contextMenu.Items.Add(exit);
            NotifyIcon.ContextMenuStrip = contextMenu;
            */

            _notifyIcon = new NotifyIcon();
            _notifyIcon.MouseClick += NotifyIcon_MouseClick;
            var icon = GetIcon(AppSettings.AppIconPath);
            _notifyIcon.Icon = icon;
            _notifyIcon.Text = _toolTipTitle;
            _notifyIcon.Visible = true;
        }

        private static void NotifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (_contextMenu != null)
                _contextMenu.Close();
            _contextMenu = new NotifyIconContextMenu(_toolViewModel);

            POINT p = new POINT();
            GetCursorPos(ref p);
            double x = p.X;
            double y = p.Y;
            var w = _contextMenu.Width;
            var h = _contextMenu.Height;
            var scrInfo = DisplayDevices.GetCurrentScreenInfo(x, y);
            var r = scrInfo.WorkArea;
            if (w + x > r.Right)
                x += r.Right - (w + x) + AppSettings.NotifyIconContextMenuDx;
            if (h + y > r.Bottom)
                y += r.Bottom - (h + y) + AppSettings.NotifyIconContextMenuDy;
            _contextMenu.Left = x;
            _contextMenu.Top = y;
#if dbg
            DesktopPanelTool.Lib.Debug.WriteLine($"scr={scrInfo}");
            DesktopPanelTool.Lib.Debug.WriteLine($"x={x} y={y} w={w} h={h}");
#endif
            _contextMenu.Show();
        }

        internal static void HideNotifyIcon()
        {
            _notifyIcon.Visible = false;
        }

        public static void Notify(
            string content,
            ToolTipIcon toolTipIcon = ToolTipIcon.Info
            )
        {
            _notifyIcon.BalloonTipIcon = toolTipIcon;
            _notifyIcon.BalloonTipTitle = _toolTipTitle;
            _notifyIcon.BalloonTipText = content;
            _notifyIcon.ShowBalloonTip(500);
        }

    }
}

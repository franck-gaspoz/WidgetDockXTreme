namespace DesktopPanelTool.Models
{
    public static class WindowMessages
    {
        public const int WM_LBUTTONDOWN = 0x0201;
        public const int WM_RBUTTONDOWN = 0x0204;
        public const int WM_LBUTTONUP = 0x0202;
        public const int WM_RBUTTONUP = 0x0205;
        public const int WM_MOUSEMOVE = 0x0200;
        public const int WM_MOUSEWHEEL = 0x020A;
        public const int WM_MOUSEHWHEEL = 0x020E;

        public const int WM_DESTROY = 0x0002;
        public const int WM_CLOSE = 0x0010;

        public const int WM_SETCURSOR = 0x0020;

        public const int WM_NCACTIVATE = 0x86;
        public const int WM_GETTEXT = 0xD;
        public const int WM_ACTIVATE = 0x6;
        public const int WM_KILLFOCUS = 0x8;
        public const int WM_IME_SETCONTEXT = 0x281;
        public const int WM_IME_NOTIFY = 0x282;

        public const int WM_SYSCOMMAND = 0x0112;
        public const int WM_CREATE = 0x0001;
        public const int WM_NCCALCSIZE = 0x0083;
        public const int WM_NCHITTEST = 0x0084;
        public const int WM_MOVING = 0x0216;
        public const int WM_ENTERSIZEMOVE = 0x0231;
        public const int WM_EXITSIZEMOVE = 0x0232;
        public const int WM_MOVE = 0X0003;
        public const int WM_ERASEBKGND = 0x0014;
        public const int WM_PAINT = 0x000F;
        public const int WM_SETFOCUS = 0x007;

        public const int WM_SPAWN_WORKER = 0x052C;  // force desktop layers composition

        public const int HTNOWHERE = 0;
        public const int HTTOPLEFT = 13;
        public const int HTTOP = 12;
        public const int HTCAPTION = 2;
        public const int HTTOPRIGHT = 14;
        public const int HTLEFT = 10;
        public const int HTRIGHT = 11;
        public const int HTBOTTOMLEFT = 16;
        public const int HTBOTTOM = 15;
        public const int HTBOTTOMRIGHT = 17;
    }
}

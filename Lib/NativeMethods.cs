using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Interop;
using static DesktopPanelTool.Models.NativeTypes;

namespace DesktopPanelTool.Lib
{
    public static class NativeMethods
    {
        [DllImport("user32.dll")]
        public static extern SafeIconHandle CreateIconIndirect(ref IconInfo icon);

        [DllImport("user32.dll")]
        public static extern bool DestroyIcon(IntPtr hIcon);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetIconInfo(IntPtr hIcon, ref IconInfo pIconInfo);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        public static extern int SetWindowsHookEx(HookType hookType, HookProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll")]
        public static extern int CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        public delegate int HookProc(int code, IntPtr wParam, IntPtr lParam);

        /// <summary>
        ///     Retrieves a handle to the Shell's desktop window.
        ///     <para>
        ///     Go to https://msdn.microsoft.com/en-us/library/windows/desktop/ms633512%28v=vs.85%29.aspx for more
        ///     information
        ///     </para>
        /// </summary>
        /// <returns>
        ///     C++ ( Type: HWND )<br />The return value is the handle of the Shell's desktop window. If no Shell process is
        ///     present, the return value is NULL.
        /// </returns>
        [DllImport("user32.dll")]
        public static extern IntPtr GetShellWindow();

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(
            IntPtr hWnd,
            IntPtr hWndInsertAfter,
            int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);

        [return: MarshalAs(UnmanagedType.Bool)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        [DllImport("user32", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool CloseWindowStation(IntPtr hWinsta);

        /// <summary>
        /// The DestroyWindow function destroys the specified window. The function sends WM_DESTROY and WM_NCDESTROY messages to the window to deactivate it and remove the keyboard focus from it. The function also destroys the window's menu, flushes the thread message queue, destroys timers, removes clipboard ownership, and breaks the clipboard viewer chain (if the window is at the top of the viewer chain)
        /// <para>If the specified window is a parent or owner window, DestroyWindow automatically destroys the associated child or owned windows when it destroys the parent or owner window. The function first destroys child or owned windows, and then it destroys the parent or owner window.</para>
        /// <para>DestroyWindow also destroys modeless dialog boxes created by the CreateDialog function.</para>
        /// <para>A thread cannot use DestroyWindow to destroy a window created by a different thread.</para>
        /// </summary>
        [DllImport("user32.dll")]
        public static extern bool DestroyWindow(IntPtr hwnd);

        /// <summary>
        /// close the window but not destroy it (minimize only)
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern bool CloseWindow(IntPtr hWnd);

        /// <summary>
        ///     Special window handles
        /// </summary>
        public enum SpecialWindowHandles
        {
            /// <summary>
            ///     Places the window at the bottom of the Z order. If the hWnd parameter identifies a topmost window, the window loses its topmost status and is placed at the bottom of all other windows.
            /// </summary>
            HWND_TOP = 0,
            /// <summary>
            ///     Places the window above all non-topmost windows (that is, behind all topmost windows). This flag has no effect if the window is already a non-topmost window.
            /// </summary>
            HWND_BOTTOM = 1,
            /// <summary>
            ///     Places the window at the top of the Z order.
            /// </summary>
            HWND_TOPMOST = -1,
            /// <summary>
            ///     Places the window above all non-topmost windows. The window maintains its topmost position even when it is deactivated.
            /// </summary>
            HWND_NOTOPMOST = -2
        }

        [Flags()]
        public enum SetWindowPosFlags : uint
        {
            /// <summary>If the calling thread and the thread that owns the window are attached to different input queues,
            /// the system posts the request to the thread that owns the window. This prevents the calling thread from
            /// blocking its execution while other threads process the request.</summary>
            /// <remarks>SWP_ASYNCWINDOWPOS</remarks>
            AsynchronousWindowPosition = 0x4000,
            /// <summary>Prevents generation of the WM_SYNCPAINT message.</summary>
            /// <remarks>SWP_DEFERERASE</remarks>
            DeferErase = 0x2000,
            /// <summary>Draws a frame (defined in the window's class description) around the window.</summary>
            /// <remarks>SWP_DRAWFRAME</remarks>
            DrawFrame = 0x0020,
            /// <summary>Applies new frame styles set using the SetWindowLong function. Sends a WM_NCCALCSIZE message to
            /// the window, even if the window's size is not being changed. If this flag is not specified, WM_NCCALCSIZE
            /// is sent only when the window's size is being changed.</summary>
            /// <remarks>SWP_FRAMECHANGED</remarks>
            FrameChanged = 0x0020,
            /// <summary>Hides the window.</summary>
            /// <remarks>SWP_HIDEWINDOW</remarks>
            HideWindow = 0x0080,
            /// <summary>Does not activate the window. If this flag is not set, the window is activated and moved to the
            /// top of either the topmost or non-topmost group (depending on the setting of the hWndInsertAfter
            /// parameter).</summary>
            /// <remarks>SWP_NOACTIVATE</remarks>
            DoNotActivate = 0x0010,
            /// <summary>Discards the entire contents of the client area. If this flag is not specified, the valid
            /// contents of the client area are saved and copied back into the client area after the window is sized or
            /// repositioned.</summary>
            /// <remarks>SWP_NOCOPYBITS</remarks>
            DoNotCopyBits = 0x0100,
            /// <summary>Retains the current position (ignores X and Y parameters).</summary>
            /// <remarks>SWP_NOMOVE</remarks>
            IgnoreMove = 0x0002,
            /// <summary>Does not change the owner window's position in the Z order.</summary>
            /// <remarks>SWP_NOOWNERZORDER</remarks>
            DoNotChangeOwnerZOrder = 0x0200,
            /// <summary>Does not redraw changes. If this flag is set, no repainting of any kind occurs. This applies to
            /// the client area, the nonclient area (including the title bar and scroll bars), and any part of the parent
            /// window uncovered as a result of the window being moved. When this flag is set, the application must
            /// explicitly invalidate or redraw any parts of the window and parent window that need redrawing.</summary>
            /// <remarks>SWP_NOREDRAW</remarks>
            DoNotRedraw = 0x0008,
            /// <summary>Same as the SWP_NOOWNERZORDER flag.</summary>
            /// <remarks>SWP_NOREPOSITION</remarks>
            DoNotReposition = 0x0200,
            /// <summary>Prevents the window from receiving the WM_WINDOWPOSCHANGING message.</summary>
            /// <remarks>SWP_NOSENDCHANGING</remarks>
            DoNotSendChangingEvent = 0x0400,
            /// <summary>Retains the current size (ignores the cx and cy parameters).</summary>
            /// <remarks>SWP_NOSIZE</remarks>
            IgnoreResize = 0x0001,
            /// <summary>Retains the current Z order (ignores the hWndInsertAfter parameter).</summary>
            /// <remarks>SWP_NOZORDER</remarks>
            IgnoreZOrder = 0x0004,
            /// <summary>Displays the window.</summary>
            /// <remarks>SWP_SHOWWINDOW</remarks>
            ShowWindow = 0x0040,
        }

        /// <summary>
        /// Retrieves the show state and the restored, minimized, and maximized positions of the specified window.
        /// </summary>
        /// <param name="hWnd">
        /// A handle to the window.
        /// </param>
        /// <param name="lpwndpl">
        /// A pointer to the WINDOWPLACEMENT structure that receives the show state and position information.
        /// <para>
        /// Before calling GetWindowPlacement, set the length member to sizeof(WINDOWPLACEMENT). GetWindowPlacement fails if lpwndpl-> length is not set correctly.
        /// </para>
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero.
        /// <para>
        /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
        /// </para>
        /// </returns>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        /// <summary>
        /// Contains information about the placement of a window on the screen.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOWPLACEMENT
        {
            /// <summary>
            /// The length of the structure, in bytes. Before calling the GetWindowPlacement or SetWindowPlacement functions, set this member to sizeof(WINDOWPLACEMENT).
            /// <para>
            /// GetWindowPlacement and SetWindowPlacement fail if this member is not set correctly.
            /// </para>
            /// </summary>
            public int Length;

            /// <summary>
            /// Specifies flags that control the position of the minimized window and the method by which the window is restored.
            /// </summary>
            public int Flags;

            /// <summary>
            /// The current show state of the window.
            /// </summary>
            public ShowWindowCommands ShowCmd;

            /// <summary>
            /// The coordinates of the window's upper-left corner when the window is minimized.
            /// </summary>
            public POINT MinPosition;

            /// <summary>
            /// The coordinates of the window's upper-left corner when the window is maximized.
            /// </summary>
            public POINT MaxPosition;

            /// <summary>
            /// The window's coordinates when the window is in the restored position.
            /// </summary>
            public RECT NormalPosition;

            /// <summary>
            /// Gets the default (empty) value.
            /// </summary>
            public static WINDOWPLACEMENT Default
            {
                get
                {
                    WINDOWPLACEMENT result = new WINDOWPLACEMENT();
                    result.Length = Marshal.SizeOf(result);
                    return result;
                }
            }
        }

        public enum ShowWindowCommands
        {
            /// <summary>
            /// Hides the window and activates another window.
            /// </summary>
            Hide = 0,
            /// <summary>
            /// Activates and displays a window. If the window is minimized or
            /// maximized, the system restores it to its original size and position.
            /// An application should specify this flag when displaying the window
            /// for the first time.
            /// </summary>
            Normal = 1,
            /// <summary>
            /// Activates the window and displays it as a minimized window.
            /// </summary>
            ShowMinimized = 2,
            /// <summary>
            /// Maximizes the specified window.
            /// </summary>
            Maximize = 3, // is this the right value?
            /// <summary>
            /// Activates the window and displays it as a maximized window.
            /// </summary>      
            ShowMaximized = 3,
            /// <summary>
            /// Displays a window in its most recent size and position. This value
            /// is similar to <see cref="Win32.ShowWindowCommand.Normal"/>, except
            /// the window is not activated.
            /// </summary>
            ShowNoActivate = 4,
            /// <summary>
            /// Activates the window and displays it in its current size and position.
            /// </summary>
            Show = 5,
            /// <summary>
            /// Minimizes the specified window and activates the next top-level
            /// window in the Z order.
            /// </summary>
            Minimize = 6,
            /// <summary>
            /// Displays the window as a minimized window. This value is similar to
            /// <see cref="Win32.ShowWindowCommand.ShowMinimized"/>, except the
            /// window is not activated.
            /// </summary>
            ShowMinNoActive = 7,
            /// <summary>
            /// Displays the window in its current size and position. This value is
            /// similar to <see cref="Win32.ShowWindowCommand.Show"/>, except the
            /// window is not activated.
            /// </summary>
            ShowNA = 8,
            /// <summary>
            /// Activates and displays the window. If the window is minimized or
            /// maximized, the system restores it to its original size and position.
            /// An application should specify this flag when restoring a minimized window.
            /// </summary>
            Restore = 9,
            /// <summary>
            /// Sets the show state based on the SW_* value specified in the
            /// STARTUPINFO structure passed to the CreateProcess function by the
            /// program that started the application.
            /// </summary>
            ShowDefault = 10,
            /// <summary>
            ///  <b>Windows 2000/XP:</b> Minimizes a window, even if the thread
            /// that owns the window is not responding. This flag should only be
            /// used when minimizing windows from a different thread.
            /// </summary>
            ForceMinimize = 11
        }

        /// <summary>
        ///     Retrieves a handle to the top-level window whose class name and window name match the specified strings. This
        ///     function does not search child windows. This function does not perform a case-sensitive search. To search child
        ///     windows, beginning with a specified child window, use the
        ///     <see cref="!:https://msdn.microsoft.com/en-us/library/windows/desktop/ms633500%28v=vs.85%29.aspx">FindWindowEx</see>
        ///     function.
        ///     <para>
        ///     Go to https://msdn.microsoft.com/en-us/library/windows/desktop/ms633499%28v=vs.85%29.aspx for FindWindow
        ///     information or https://msdn.microsoft.com/en-us/library/windows/desktop/ms633500%28v=vs.85%29.aspx for
        ///     FindWindowEx
        ///     </para>
        /// </summary>
        /// <param name="lpClassName">
        ///     C++ ( lpClassName [in, optional]. Type: LPCTSTR )<br />The class name or a class atom created by a previous call to
        ///     the RegisterClass or RegisterClassEx function. The atom must be in the low-order word of lpClassName; the
        ///     high-order word must be zero.
        ///     <para>
        ///     If lpClassName points to a string, it specifies the window class name. The class name can be any name
        ///     registered with RegisterClass or RegisterClassEx, or any of the predefined control-class names.
        ///     </para>
        ///     <para>If lpClassName is NULL, it finds any window whose title matches the lpWindowName parameter.</para>
        /// </param>
        /// <param name="lpWindowName">
        ///     C++ ( lpWindowName [in, optional]. Type: LPCTSTR )<br />The window name (the window's
        ///     title). If this parameter is NULL, all window names match.
        /// </param>
        /// <returns>
        ///     C++ ( Type: HWND )<br />If the function succeeds, the return value is a handle to the window that has the
        ///     specified class name and window name. If the function fails, the return value is NULL.
        ///     <para>To get extended error information, call GetLastError.</para>
        /// </returns>
        /// <remarks>
        ///     If the lpWindowName parameter is not NULL, FindWindow calls the <see cref="M:GetWindowText" /> function to
        ///     retrieve the window name for comparison. For a description of a potential problem that can arise, see the Remarks
        ///     for <see cref="M:GetWindowText" />.
        /// </remarks>
        // For Windows Mobile, replace user32.dll with coredll.dll
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        // Find window by Caption only. Note you must pass IntPtr.Zero as the first parameter.

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        public static extern IntPtr FindWindowByCaption(
            IntPtr ZeroOnly,
            string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        // You can also call FindWindow(default(string), lpWindowName) or FindWindow((string)null, lpWindowName)

        #region Send Message API

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessageTimeout(
            IntPtr hWnd,
            uint Msg,
            UIntPtr wParam,
            IntPtr lParam,
            SendMessageTimeoutFlags fuFlags,
            uint uTimeout,
            out UIntPtr lpdwResult);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessageTimeout(
            IntPtr windowHandle,
            uint Msg,
            IntPtr wParam,
            IntPtr lParam,
            SendMessageTimeoutFlags flags,
            uint timeout,
            out IntPtr result);

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        #endregion

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [Flags]
        public enum SendMessageTimeoutFlags : uint
        {
            SMTO_NORMAL = 0x0,
            SMTO_BLOCK = 0x1,
            SMTO_ABORTIFHUNG = 0x2,
            SMTO_NOTIMEOUTIFNOTHUNG = 0x8,
            SMTO_ERRORONEXIT = 0x20
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        public static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
        public const UInt32 SWP_NOSIZE = 0x0001;
        public const UInt32 SWP_NOMOVE = 0x0002;
        public const UInt32 SWP_NOACTIVATE = 0x0010;

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        /// <summary>
        /// Retrieves the handle to the ancestor of the specified window.
        /// </summary>
        /// <param name="hwnd">A handle to the window whose ancestor is to be retrieved.
        /// If this parameter is the desktop window, the function returns NULL. </param>
        /// <param name="flags">The ancestor to be retrieved.</param>
        /// <returns>The return value is the handle to the ancestor window.</returns>
        [DllImport("user32.dll", ExactSpelling = true)]
        public static extern IntPtr GetAncestor(IntPtr hwnd, GetAncestorFlags flags);

        public enum GetAncestorFlags : uint
        {
            /// <summary>
            /// Retrieves the parent window.This does not include the owner, as it does with the GetParent function.
            /// </summary>
            GA_PARENT = 1,

            /// <summary>
            /// Retrieves the root window by walking the chain of parent windows.
            /// </summary>
            GA_ROOT = 2,

            /// <summary>
            /// Retrieves the owned root window by walking the chain of parent and owner windows returned by GetParent.
            /// </summary>
            GA_ROOTOWNER = 3
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetDCEx(IntPtr hWnd, IntPtr hrgnClip, DeviceContextValues flags);

        [DllImport("user32.dll")]
        public static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll", EntryPoint = "DeleteDC")]
        public static extern bool DeleteDC([In] IntPtr hdc);

        [DllImport("user32.dll")]
        public static extern bool IsIconic(IntPtr hWnd);

        [DllImport("core.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("core.dll")]
        public static extern IntPtr GetDesktopWindow();

        [DllImport("gdi32.dll", EntryPoint = "SelectObject")]
        public static extern IntPtr SelectObject([In] IntPtr hdc, [In] IntPtr hgdiobj);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool ClientToScreen(IntPtr hWnd, ref Point lpPoint);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleDC", SetLastError = true)]
        public static extern IntPtr CreateCompatibleDC([In] IntPtr hdc);

        [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleBitmap")]
        public static extern IntPtr CreateCompatibleBitmap([In] IntPtr hdc, int nWidth, int nHeight);

        [DllImport("gdi32.dll", EntryPoint = "BitBlt", SetLastError = true)]
        public static extern bool BitBlt([In] IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, [In] IntPtr hdcSrc, int nXSrc, int nYSrc, TernaryRasterOperations dwRop);

        /// <summary>
        ///     Specifies a raster-operation code. These codes define how the color data for the
        ///     source rectangle is to be combined with the color data for the destination
        ///     rectangle to achieve the final color.
        /// </summary>
        public enum TernaryRasterOperations : uint
        {
            /// <summary>dest = source</summary>
            SRCCOPY = 0x00CC0020,
            /// <summary>dest = source OR dest</summary>
            SRCPAINT = 0x00EE0086,
            /// <summary>dest = source AND dest</summary>
            SRCAND = 0x008800C6,
            /// <summary>dest = source XOR dest</summary>
            SRCINVERT = 0x00660046,
            /// <summary>dest = source AND (NOT dest)</summary>
            SRCERASE = 0x00440328,
            /// <summary>dest = (NOT source)</summary>
            NOTSRCCOPY = 0x00330008,
            /// <summary>dest = (NOT src) AND (NOT dest)</summary>
            NOTSRCERASE = 0x001100A6,
            /// <summary>dest = (source AND pattern)</summary>
            MERGECOPY = 0x00C000CA,
            /// <summary>dest = (NOT source) OR dest</summary>
            MERGEPAINT = 0x00BB0226,
            /// <summary>dest = pattern</summary>
            PATCOPY = 0x00F00021,
            /// <summary>dest = DPSnoo</summary>
            PATPAINT = 0x00FB0A09,
            /// <summary>dest = pattern XOR dest</summary>
            PATINVERT = 0x005A0049,
            /// <summary>dest = (NOT dest)</summary>
            DSTINVERT = 0x00550009,
            /// <summary>dest = BLACK</summary>
            BLACKNESS = 0x00000042,
            /// <summary>dest = WHITE</summary>
            WHITENESS = 0x00FF0062,
            /// <summary>
            /// Capture window as seen on screen.  This includes layered windows
            /// such as WPF windows with AllowsTransparency="true"
            /// </summary>
            CAPTUREBLT = 0x40000000
        }

        /// <summary>Values to pass to the GetDCEx method.</summary>
        [Flags()]
        public enum DeviceContextValues : uint
        {
            /// <summary>DCX_WINDOW: Returns a DC that corresponds to the window rectangle rather
            /// than the client rectangle.</summary>
            Window = 0x00000001,
            /// <summary>DCX_CACHE: Returns a DC from the cache, rather than the OWNDC or CLASSDC
            /// window. Essentially overrides CS_OWNDC and CS_CLASSDC.</summary>
            Cache = 0x00000002,
            /// <summary>DCX_NORESETATTRS: Does not reset the attributes of this DC to the
            /// default attributes when this DC is released.</summary>
            NoResetAttrs = 0x00000004,
            /// <summary>DCX_CLIPCHILDREN: Excludes the visible regions of all child windows
            /// below the window identified by hWnd.</summary>
            ClipChildren = 0x00000008,
            /// <summary>DCX_CLIPSIBLINGS: Excludes the visible regions of all sibling windows
            /// above the window identified by hWnd.</summary>
            ClipSiblings = 0x00000010,
            /// <summary>DCX_PARENTCLIP: Uses the visible region of the parent window. The
            /// parent's WS_CLIPCHILDREN and CS_PARENTDC style bits are ignored. The origin is
            /// set to the upper-left corner of the window identified by hWnd.</summary>
            ParentClip = 0x00000020,
            /// <summary>DCX_EXCLUDERGN: The clipping region identified by hrgnClip is excluded
            /// from the visible region of the returned DC.</summary>
            ExcludeRgn = 0x00000040,
            /// <summary>DCX_INTERSECTRGN: The clipping region identified by hrgnClip is
            /// intersected with the visible region of the returned DC.</summary>
            IntersectRgn = 0x00000080,
            /// <summary>DCX_EXCLUDEUPDATE: Unknown...Undocumented</summary>
            ExcludeUpdate = 0x00000100,
            /// <summary>DCX_INTERSECTUPDATE: Unknown...Undocumented</summary>
            IntersectUpdate = 0x00000200,
            /// <summary>DCX_LOCKWINDOWUPDATE: Allows drawing even if there is a LockWindowUpdate
            /// call in effect that would otherwise exclude this window. Used for drawing during
            /// tracking.</summary>
            LockWindowUpdate = 0x00000400,
            /// <summary>DCX_USESTYLE: Undocumented, something related to WM_NCPAINT message.</summary>
            UseStyle = 0x00010000,
            /// <summary>DCX_VALIDATE When specified with DCX_INTERSECTUPDATE, causes the DC to
            /// be completely validated. Using this function with both DCX_INTERSECTUPDATE and
            /// DCX_VALIDATE is identical to using the BeginPaint function.</summary>
            Validate = 0x00200000,
        }

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public const int SW_FORCEMINIMIZE = 11;
        public const int SW_HIDE = 0;
        public const int SW_MAXIMIZE = 3;
        public const int SW_MINIMIZE = 6;
        public const int SW_RESTORE = 9;
        public const int SW_SHOW = 5;
        public const int SW_SHOWDEFAULT = 10;
        public const int SW_SHOWMAXIMIZED = 3;
        public const int SW_SHOWMINIMIZED = 2;
        public const int SW_SHOWMINNOACTIVE = 7;
        public const int SW_SHOWNA = 8;
        public const int SW_SHOWNOACTIVATE = 4;
        public const int SW_SHOWNORMAL = 1;

        [DllImport("user32.dll")]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        public static List<(IntPtr winHandle, WINDOWINFO windowInfo, string windowInfoDescription)> GetVisibleWindowsOnDesktop()
        {
            int n = 0;
            var monitors = new List<RECT>();
            var unwished = new List<string> {
                "WorkerW",
                "Progman",
                "Shell_TrayWnd",
                "BaseBar",
                "DV2ControlHost",
                "SysShadow",
                "TaskListThumbnailWnd"
            };
            var res = new List<(IntPtr winHandle, WINDOWINFO windowInfo, string windowInfoDescription)>();

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
                        var mon = new RECT(
                            mi.Monitor.Left,
                            mi.Monitor.Top,
                            Math.Abs(mi.Monitor.Right - mi.Monitor.Left),
                            mi.Monitor.Bottom - mi.Monitor.Top
                        );
                        monitors.Add(mon);
                        //DesktopPanelTool.Lib.Debug.WriteLine(mon);
                    }
                    return true;
                }, IntPtr.Zero);

            EnumWindows(
                new EnumWindowsProc(
                    (tophandle, topparamhandle) =>
                    {
                        StringBuilder sb3 = new StringBuilder(1024);
                        GetClassName(tophandle, sb3, sb3.Capacity);
                        var clName = sb3.ToString();

                        StringBuilder sb4 = new StringBuilder(1024);
                        GetWindowText(tophandle, sb4, sb4.Capacity);
                        WINDOWINFO twi = new WINDOWINFO();

                        if (IsWindowVisible(tophandle))
                        {
                            if (GetWindowInfo(tophandle, ref twi))
                            {
                                if (!twi.rcClient.IsEmpty)
                                {
                                    bool outView = true;
                                    foreach (var monitor in monitors)
                                    {
                                        RECT its = new RECT();
                                        RECT mon = new RECT(
                                            monitor.Left,
                                            monitor.Top,
                                            monitor.Left + monitor.Right,
                                            monitor.Top + monitor.Height
                                            );
                                        if (IntersectRect(ref its, ref twi.rcWindow, ref mon) == 1)
                                            outView = false;
                                    }
                                    if (!outView)
                                    {
                                        var anc1 = GetAncestor(tophandle, GetAncestorFlags.GA_PARENT);  // shell ?
                                        var anc2 = GetAncestor(tophandle, GetAncestorFlags.GA_ROOT);
                                        var anc3 = GetAncestor(tophandle, GetAncestorFlags.GA_ROOTOWNER);
                                        if (anc3 == tophandle)
                                        {
                                            if (!clName.StartsWith("#") &&
                                                !unwished.Contains(clName))
                                            {
                                                var infText = $"{n++}: wt='{sb4.ToString()}' wh={tophandle.ToString("X")} anw={anc1.ToString("X")} wiPos={twi.rcWindow.ToString()} clPos={twi.rcClient.ToString()} status={twi.dwWindowStatus} clN={clName}";
                                                //DesktopPanelTool.Lib.Debug.WriteLine(infText);
                                                res.Add((tophandle, twi, infText));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        return true;
                    }),
                    IntPtr.Zero);
            return res;
        }

        public static WINDOWINFO? IsHandleOfVisibleWindowOnDesktop(IntPtr winHdl)
        {
            var wi = new WINDOWINFO();
            if (GetWindowInfo(winHdl, ref wi))
            {
                var visOnDsk = GetVisibleWindowsOnDesktop();
                foreach (var (winHandle, windowInfo, windowInfoDescription) in visOnDsk)
                {
                    if (winHandle == winHdl)
                    {
                        //DesktopPanelTool.Lib.Debug.WriteLine(windowInfoDescription);
                        return windowInfo;
                    }
                }
            }
            return null;
        }

        public static (WINDOWINFO? winInfo, IntPtr winHandle, string winDesc)
            GetVisibleWindowOnDesktopFromPoint(POINT point, IntPtr excludedWHdl)
        {
            return GetVisibleWindowOnDesktopFromPoint(point, new List<IntPtr> { excludedWHdl });
        }

        public static (WINDOWINFO? winInfo, IntPtr winHandle, string winDesc)
            GetVisibleWindowOnDesktopFromPoint(POINT point, List<IntPtr> excludedWHdls)
        {
            //DesktopPanelTool.Lib.Debug.WriteLine($"({point.X},{point.Y})");

            var wint = new List<(WINDOWINFO winInfo, IntPtr winHandle, string winDesc)>();
            var dwint = new Dictionary<IntPtr, (WINDOWINFO winInfo, IntPtr winHandle, string winDesc)>();

            var visOnDsk = GetVisibleWindowsOnDesktop();
            foreach (var (winHandle, windowInfo, windowInfoDescription) in visOnDsk)
            {
                if (windowInfo.rcWindow.Contains(point) && !excludedWHdls.Contains(winHandle))
                    dwint.Add(winHandle, (windowInfo, winHandle, windowInfoDescription));
            }

            if (dwint.Count > 0)
            {
                var shWndHdl = GetShellWindow();
                if (shWndHdl != IntPtr.Zero)
                {
                    var rootwhdl = GetAncestor(shWndHdl, GetAncestorFlags.GA_PARENT);
                    //var rootwhdl = GetAncestor(dwint.Values.First().winHandle, GetAncestorFlags.GA_PARENT);   // bad ancestor
                    var orderedWinHandle = GetTopWindow(rootwhdl);
                    while (orderedWinHandle != IntPtr.Zero)
                    {
                        orderedWinHandle = GetWindow(orderedWinHandle, GetWindowType.GW_HWNDNEXT);
                        if (orderedWinHandle != IntPtr.Zero
                            && dwint.TryGetValue(orderedWinHandle, out var owininf))
                            wint.Add(owininf);
                    }
                    if (wint.Count > 0)
                        return wint.First();    // upper!
                }
            }

            return (null, IntPtr.Zero, string.Empty);
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetTopWindow(IntPtr hWnd);

        /// <summary>
        /// Retrieves a handle to a window that has the specified relationship (Z-Order or owner) to the specified window.
        /// </summary>
        /// <remarks>The EnumChildWindows function is more reliable than calling GetWindow in a loop. An application that
        /// calls GetWindow to perform this task risks being caught in an infinite loop or referencing a handle to a window
        /// that has been destroyed.</remarks>
        /// <param name="hWnd">A handle to a window. The window handle retrieved is relative to this window, based on the
        /// value of the uCmd parameter.</param>
        /// <param name="uCmd">The relationship between the specified window and the window whose handle is to be
        /// retrieved.</param>
        /// <returns>
        /// If the function succeeds, the return value is a window handle. If no window exists with the specified relationship
        /// to the specified window, the return value is NULL. To get extended error information, call GetLastError.
        /// </returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetWindow(IntPtr hWnd, GetWindowType uCmd);

        public enum GetWindowType : uint
        {
            /// <summary>
            /// The retrieved handle identifies the window of the same type that is highest in the Z order.
            /// <para/>
            /// If the specified window is a topmost window, the handle identifies a topmost window.
            /// If the specified window is a top-level window, the handle identifies a top-level window.
            /// If the specified window is a child window, the handle identifies a sibling window.
            /// </summary>
            GW_HWNDFIRST = 0,
            /// <summary>
            /// The retrieved handle identifies the window of the same type that is lowest in the Z order.
            /// <para />
            /// If the specified window is a topmost window, the handle identifies a topmost window.
            /// If the specified window is a top-level window, the handle identifies a top-level window.
            /// If the specified window is a child window, the handle identifies a sibling window.
            /// </summary>
            GW_HWNDLAST = 1,
            /// <summary>
            /// The retrieved handle identifies the window below the specified window in the Z order.
            /// <para />
            /// If the specified window is a topmost window, the handle identifies a topmost window.
            /// If the specified window is a top-level window, the handle identifies a top-level window.
            /// If the specified window is a child window, the handle identifies a sibling window.
            /// </summary>
            GW_HWNDNEXT = 2,
            /// <summary>
            /// The retrieved handle identifies the window above the specified window in the Z order.
            /// <para />
            /// If the specified window is a topmost window, the handle identifies a topmost window.
            /// If the specified window is a top-level window, the handle identifies a top-level window.
            /// If the specified window is a child window, the handle identifies a sibling window.
            /// </summary>
            GW_HWNDPREV = 3,
            /// <summary>
            /// The retrieved handle identifies the specified window's owner window, if any.
            /// </summary>
            GW_OWNER = 4,
            /// <summary>
            /// The retrieved handle identifies the child window at the top of the Z order,
            /// if the specified window is a parent window; otherwise, the retrieved handle is NULL.
            /// The function examines only child windows of the specified window. It does not examine descendant windows.
            /// </summary>
            GW_CHILD = 5,
            /// <summary>
            /// The retrieved handle identifies the enabled popup window owned by the specified window (the
            /// search uses the first such window found using GW_HWNDNEXT); otherwise, if there are no enabled
            /// popup windows, the retrieved handle is that of the specified window.
            /// </summary>
            GW_ENABLEDPOPUP = 6
        }

        public const int GWL_STYLE = -16;
        public const int GWL_HINSTANCE = -6;
        public const int GWL_ID = -12;
        public const int GWL_USERDATA = -21;
        public const int GWL_WNDPROC = -4;
        public const int GWL_EXSTYLE = -20;

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        public const long LWA_COLORKEY = 1, LWA_ALPHA = 2;

        [DllImport("user32")]
        public static extern int SetLayeredWindowAttributes(IntPtr hWnd, byte crey, byte alpha, int flags);

        // This helper static method is required because the 32-bit version of user32.dll does not contain this API
        // (on any versions of Windows), so linking the method will fail at run-time. The bridge dispatches the request
        // to the correct function (GetWindowLong in 32-bit mode and GetWindowLongPtr in 64-bit mode)
        public static IntPtr SetWindowLongPtr(HandleRef hWnd, int nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size == 8)
                return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
            else
                return new IntPtr(SetWindowLong32(hWnd, nIndex, dwNewLong.ToInt32()));
        }

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        private static extern int SetWindowLong32(HandleRef hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLongPtr64(HandleRef hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32")]
        private static extern IntPtr GetParent(IntPtr hWnd);

        public const int SC_MAXIMIZE = 0xF030;
        public const int SC_MINIMIZE = 0xF020;
        public const int SC_RESTORE = 0xF120;

        public static List<string> DwStyleText(uint dwStyle)
        {
            var r = new List<string>();
            if ((dwStyle & WS_ACTIVECAPTION) != 0) r.Add("WS_ACTIVECAPTION");
            if ((dwStyle & WS_BORDER) != 0) r.Add("WS_BORDER");
            if ((dwStyle & WS_CAPTION) != 0) r.Add("WS_CAPTION");
            if ((dwStyle & WS_SYSMENU) != 0) r.Add("WS_SYSMENU");
            if ((dwStyle & WS_CHILD) != 0) r.Add("WS_CHILD");
            if ((dwStyle & WS_CHILDWINDOW) != 0) r.Add("WS_CHILDWINDOW");
            if ((dwStyle & WS_CLIPCHILDREN) != 0) r.Add("WS_CLIPCHILDREN");
            if ((dwStyle & WS_CLIPSIBLINGS) != 0) r.Add("WS_CLIPSIBLINGS");
            if ((dwStyle & WS_DLGFRAME) != 0) r.Add("WS_DLGFRAME");
            if ((dwStyle & WS_GROUP) != 0) r.Add("WS_GROUP");
            if ((dwStyle & WS_DISABLED) != 0) r.Add("WS_DISABLED");
            if ((dwStyle & WS_HSCROLL) != 0) r.Add("WS_HSCROLL");
            if ((dwStyle & WS_ICONIC) != 0) r.Add("WS_ICONIC");
            if ((dwStyle & WS_MAXIMIZE) != 0) r.Add("WS_MAXIMIZE");
            if ((dwStyle & WS_MINIMIZE) != 0) r.Add("WS_MINIMIZE");
            if ((dwStyle & WS_POPUP) != 0) r.Add("WS_POPUP");
            if ((dwStyle & WS_SIZEBOX) != 0) r.Add("WS_SIZEBOX");
            if ((dwStyle & WS_TABSTOP) != 0) r.Add("WS_TABSTOP");
            if ((dwStyle & WS_TILED) != 0) r.Add("WS_TILED");
            if ((dwStyle & WS_VISIBLE) != 0) r.Add("WS_VISIBLE");
            if ((dwStyle & WS_VSCROLL) != 0) r.Add("WS_VSCROLL");
            if ((dwStyle & WS_THICKFRAME) != 0) r.Add("WS_THICKFRAME");
            if ((dwStyle & WS_MINIMIZEBOX) != 0) r.Add("WS_MINIMIZEBOX");
            if ((dwStyle & WS_MAXIMIZEBOX) != 0) r.Add("WS_MAXIMIZEBOX");
            if ((dwStyle & WS_OVERLAPPED) != 0) r.Add("WS_OVERLAPPED");
            return r;
        }

        public const long WS_ACTIVECAPTION = 0x0001;    // ?
        public const long WS_BORDER = 0x00800000L;
        public const long WS_CAPTION = 0x00C00000L;
        public const long WS_SYSMENU = 0x00080000L;
        public const long WS_CHILD = 0x40000000L;
        public const long WS_CHILDWINDOW = 0x40000000L;
        public const long WS_CLIPCHILDREN = 0x02000000L;
        public const long WS_CLIPSIBLINGS = 0x04000000L;
        public const long WS_DLGFRAME = 0x00400000L;
        public const long WS_GROUP = 0x00020000L;
        public const long WS_DISABLED = 0x08000000L;
        public const long WS_HSCROLL = 0x00100000L;
        public const long WS_ICONIC = 0x20000000L;
        public const long WS_MAXIMIZE = 0x01000000L;
        public const long WS_MINIMIZE = 0x00010000L;
        public const long WS_POPUP = 0x80000000L;
        public const long WS_SIZEBOX = 0x00040000L;
        public const long WS_TABSTOP = 0x00010000L;
        public const long WS_TILED = 0x00000000L;
        public const long WS_VISIBLE = 0x10000000L;
        public const long WS_VSCROLL = 0x00200000L;
        public const long WS_TILEDWINDOW = (WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX);
        public const long WS_POPUPWINDOW = (WS_POPUP | WS_BORDER | WS_SYSMENU);

        public const long WS_THICKFRAME = 0x00040000L;
        public const long WS_MINIMIZEBOX = 0x00020000L;
        public const long WS_MAXIMIZEBOX = 0x00010000L;
        public const long WS_OVERLAPPED = 0x00000000L;
        public const long WS_OVERLAPPEDWINDOW = (WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX);

        public static List<string> DwExStyleText(uint dwExStyle)
        {
            var r = new List<string>();
            if ((dwExStyle & WS_EX_ACCEPTFILES) != 0) r.Add("WS_EX_ACCEPTFILES");
            if ((dwExStyle & WS_EX_APPWINDOW) != 0) r.Add("WS_EX_APPWINDOW");
            if ((dwExStyle & WS_EX_CLIENTEDGE) != 0) r.Add("WS_EX_CLIENTEDGE");
            if ((dwExStyle & WS_EX_COMPOSITED) != 0) r.Add("WS_EX_COMPOSITED");
            if ((dwExStyle & WS_EX_CONTEXTHELP) != 0) r.Add("WS_EX_CONTEXTHELP");
            if ((dwExStyle & WS_EX_CONTROLPARENT) != 0) r.Add("WS_EX_CONTROLPARENT");
            if ((dwExStyle & WS_EX_LAYERED) != 0) r.Add("WS_EX_LAYERED");
            if ((dwExStyle & WS_EX_LAYOUTRTL) != 0) r.Add("WS_EX_LAYOUTRTL");
            if ((dwExStyle & WS_EX_LEFT) != 0) r.Add("WS_EX_LEFT");
            if ((dwExStyle & WS_EX_LEFTSCROLLBAR) != 0) r.Add("WS_EX_LEFTSCROLLBAR");
            if ((dwExStyle & WS_EX_LTRREADING) != 0) r.Add("WS_EX_LTRREADING");
            if ((dwExStyle & WS_EX_MDICHILD) != 0) r.Add("WS_EX_MDICHILD");
            if ((dwExStyle & WS_EX_NOACTIVATE) != 0) r.Add("WS_EX_NOACTIVATE");
            if ((dwExStyle & WS_EX_NOINHERITLAYOUT) != 0) r.Add("WS_EX_NOINHERITLAYOUT");
            if ((dwExStyle & WS_EX_NOPARENTNOTIFY) != 0) r.Add("WS_EX_NOPARENTNOTIFY");
            if ((dwExStyle & WS_EX_NOREDIRECTIONBITMAP) != 0) r.Add("WS_EX_NOREDIRECTIONBITMAP");
            if ((dwExStyle & WS_EX_RIGHT) != 0) r.Add("WS_EX_RIGHT");
            if ((dwExStyle & WS_EX_RIGHTSCROLLBAR) != 0) r.Add("WS_EX_RIGHTSCROLLBAR");
            if ((dwExStyle & WS_EX_RTLREADING) != 0) r.Add("WS_EX_RTLREADING");
            if ((dwExStyle & WS_EX_STATICEDGE) != 0) r.Add("WS_EX_STATICEDGE");
            if ((dwExStyle & WS_EX_TOOLWINDOW) != 0) r.Add("WS_EX_TOOLWINDOW");
            if ((dwExStyle & WS_EX_TOPMOST) != 0) r.Add("WS_EX_TOPMOST");
            if ((dwExStyle & WS_EX_TRANSPARENT) != 0) r.Add("WS_EX_TRANSPARENT");
            if ((dwExStyle & WS_EX_WINDOWEDGE) != 0) r.Add("WS_EX_WINDOWEDGE");
            return r;
        }

        public const long WS_EX_ACCEPTFILES = 0x00000010L;
        public const long WS_EX_APPWINDOW = 0x00040000L;
        public const long WS_EX_CLIENTEDGE = 0x00000200L;
        public const long WS_EX_COMPOSITED = 0x02000000L;
        public const long WS_EX_CONTEXTHELP = 0x00000400L;
        public const long WS_EX_CONTROLPARENT = 0x00010000L;
        public const long WS_EX_LAYERED = 0x00080000;
        public const long WS_EX_LAYOUTRTL = 0x00400000L;
        public const long WS_EX_LEFT = 0x00000000L;
        public const long WS_EX_LEFTSCROLLBAR = 0x00004000L;
        public const long WS_EX_LTRREADING = 0x00000000L;
        public const long WS_EX_MDICHILD = 0x00000040L;
        public const long WS_EX_NOACTIVATE = 0x08000000L;
        public const long WS_EX_NOINHERITLAYOUT = 0x00100000L;
        public const long WS_EX_NOPARENTNOTIFY = 0x00000004L;
        public const long WS_EX_NOREDIRECTIONBITMAP = 0x00200000L;
        public const long WS_EX_OVERLAPPEDWINDOW = (WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE);
        public const long WS_EX_PALETTEWINDOW = (WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST);
        public const long WS_EX_RIGHT = 0x00001000L;
        public const long WS_EX_RIGHTSCROLLBAR = 0x00000000L;
        public const long WS_EX_RTLREADING = 0x00002000L;
        public const long WS_EX_STATICEDGE = 0x00020000L;
        public const long WS_EX_TOOLWINDOW = 0x00000080L;
        public const long WS_EX_TOPMOST = 0x00000008L;
        public const long WS_EX_TRANSPARENT = 0x00000020L;
        public const long WS_EX_WINDOWEDGE = 0x00000100L;

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

        public enum HitTestValues
        {
            /// <summary>
            /// In the border of a window that does not have a sizing border.
            /// </summary>
            HTBORDER = 18,

            /// <summary>
            /// In the lower-horizontal border of a resizable window (the user can click the mouse to resize the window vertically).
            /// </summary>
            HTBOTTOM = 15,

            /// <summary>
            /// In the lower-left corner of a border of a resizable window (the user can click the mouse to resize the window diagonally).
            /// </summary>
            HTBOTTOMLEFT = 16,

            /// <summary>
            /// In the lower-right corner of a border of a resizable window (the user can click the mouse to resize the window diagonally).
            /// </summary>
            HTBOTTOMRIGHT = 17,

            /// <summary>
            /// In a title bar.
            /// </summary>
            HTCAPTION = 2,

            /// <summary>
            /// In a client area.
            /// </summary>
            HTCLIENT = 1,

            /// <summary>
            /// In a Close button.
            /// </summary>
            HTCLOSE = 20,

            /// <summary>
            /// On the screen background or on a dividing line between windows (same as HTNOWHERE, except that the DefWindowProc function produces a system beep to indicate an error).
            /// </summary>
            HTERROR = -2,

            /// <summary>
            /// In a size box (same as HTSIZE).
            /// </summary>
            HTGROWBOX = 4,

            /// <summary>
            /// In a Help button.
            /// </summary>
            HTHELP = 21,

            /// <summary>
            /// In a horizontal scroll bar.
            /// </summary>
            HTHSCROLL = 6,

            /// <summary>
            /// In the left border of a resizable window (the user can click the mouse to resize the window horizontally).
            /// </summary>
            HTLEFT = 10,

            /// <summary>
            /// In a menu.
            /// </summary>
            HTMENU = 5,

            /// <summary>
            /// In a Maximize button.
            /// </summary>
            HTMAXBUTTON = 9,

            /// <summary>
            /// In a Minimize button.
            /// </summary>
            HTMINBUTTON = 8,

            /// <summary>
            /// On the screen background or on a dividing line between windows.
            /// </summary>
            HTNOWHERE = 0,

            /// <summary>
            /// Not implemented.
            /// </summary>
            /* HTOBJECT = 19, */

            /// <summary>
            /// In a Minimize button.
            /// </summary>
            HTREDUCE = HTMINBUTTON,

            /// <summary>
            /// In the right border of a resizable window (the user can click the mouse to resize the window horizontally).
            /// </summary>
            HTRIGHT = 11,

            /// <summary>
            /// In a size box (same as HTGROWBOX).
            /// </summary>
            HTSIZE = HTGROWBOX,

            /// <summary>
            /// In a window menu or in a Close button in a child window.
            /// </summary>
            HTSYSMENU = 3,

            /// <summary>
            /// In the upper-horizontal border of a window.
            /// </summary>
            HTTOP = 12,

            /// <summary>
            /// In the upper-left corner of a window border.
            /// </summary>
            HTTOPLEFT = 13,

            /// <summary>
            /// In the upper-right corner of a window border.
            /// </summary>
            HTTOPRIGHT = 14,

            /// <summary>
            /// In a window currently covered by another window in the same thread (the message will be sent to underlying windows in the same thread until one of them returns a code that is not HTTRANSPARENT).
            /// </summary>
            HTTRANSPARENT = -1,

            /// <summary>
            /// In the vertical scroll bar.
            /// </summary>
            HTVSCROLL = 7,

            /// <summary>
            /// In a Maximize button.
            /// </summary>
            HTZOOM = HTMAXBUTTON,
        }

        [DllImport("user32.dll")]
        public static extern short GetKeyState(System.Windows.Forms.Keys vKey);

        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(System.Windows.Forms.Keys vKey);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetCursorPos(ref POINT pt);

        [DllImport("user32.dll")]
        public static extern bool GetCursorInfo(ref CURSORINFO pci);

        [DllImport("user32.dll")]
        public static extern bool AdjustWindowRectEx(ref RECT lpRect, uint dwStyle,
            bool bMenu, uint dwExStyle);

        [DllImport("dwmapi.dll")]
        public static extern bool DwmDefWindowProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref IntPtr plResult);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [DllImport("User32")]
        public static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);

        public const Int32 MONITOR_DEFAULT_TO_NULL = 0x00000000;
        public const Int32 MONITOR_DEFAULT_TO_PRIMARY = 0x00000001;
        public const Int32 MONITOR_DEFAULT_TO_NEAREST = 0x00000002;

        [DllImport("user32.dll")]
        public static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll", EntryPoint = "GetDC")]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("gdi32.dll")]
        public static extern bool FillRgn(IntPtr hdc, IntPtr hrgn, IntPtr hbr);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateRectRgn(int nLeftRect, int nTopRect, int nRightRect,
            int nBottomRect);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateSolidBrush(uint crColor);

        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        [DllImport("user32.dll")]
        public static extern int FillRect(IntPtr hDC, [In] ref RECT lprc, IntPtr hbr);

        [DllImport("user32.dll")]
        public static extern bool IsWindow(IntPtr hWnd);

        /// <summary>
        /// returns 1 if intersects
        /// </summary>
        /// <param name="lpDestRect"></param>
        /// <param name="lpSrc1Rect"></param>
        /// <param name="lpSrc2Rect"></param>
        /// <returns></returns>
        [DllImport("user32")]
        public static extern int IntersectRect(ref RECT lpDestRect, ref RECT lpSrc1Rect, ref RECT lpSrc2Rect);

        /// <summary>
        /// returns 1 if empty
        /// </summary>
        /// <param name="lpRect"></param>
        /// <returns></returns>
        [DllImport("user32")]
        public static extern int IsRectEmpty(ref RECT lpRect);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SystemParametersInfo(SPI uiAction, uint uiParam, ref ANIMATIONINFO pvParam, SPIF fWinIni);

        [DllImport("user32.dll", EntryPoint = "SystemParametersInfo")]
        public static extern bool SystemParametersInfo2(SPI uiAction, uint uiParam, uint? pvParam, SPIF fWinIni);

        [StructLayout(LayoutKind.Sequential)]
        public struct ANIMATIONINFO
        {

            public ANIMATIONINFO(bool iMinAnimate)
            {
                this.cbSize = GetSize();
                if (iMinAnimate) this.iMinAnimate = 1;
                else this.iMinAnimate = 0;
            }

            public uint cbSize;
            private int iMinAnimate;

            public bool IMinAnimate
            {
                get
                {
                    if (this.iMinAnimate == 0) return false;
                    else return true;
                }
                set
                {
                    if (value == true) this.iMinAnimate = 1;
                    else this.iMinAnimate = 0;
                }
            }

            public static uint GetSize()
            {
                return (uint)Marshal.SizeOf(typeof(ANIMATIONINFO));
            }
        }

        #region SPI

        /// <summary>
        /// SPI_ System-wide parameter - Used in SystemParametersInfo function
        /// </summary>
        [Description("SPI_(System-wide parameter - Used in SystemParametersInfo function )")]
        public enum SPI : uint
        {
            /// <summary>
            /// Determines whether the warning beeper is on.
            /// The pvParam parameter must point to a BOOL variable that receives TRUE if the beeper is on, or FALSE if it is off.
            /// </summary>
            SPI_GETBEEP = 0x0001,

            /// <summary>
            /// Turns the warning beeper on or off. The uiParam parameter specifies TRUE for on, or FALSE for off.
            /// </summary>
            SPI_SETBEEP = 0x0002,

            /// <summary>
            /// Retrieves the two mouse threshold values and the mouse speed.
            /// </summary>
            SPI_GETMOUSE = 0x0003,

            /// <summary>
            /// Sets the two mouse threshold values and the mouse speed.
            /// </summary>
            SPI_SETMOUSE = 0x0004,

            /// <summary>
            /// Retrieves the border multiplier factor that determines the width of a window's sizing border.
            /// The pvParam parameter must point to an integer variable that receives this value.
            /// </summary>
            SPI_GETBORDER = 0x0005,

            /// <summary>
            /// Sets the border multiplier factor that determines the width of a window's sizing border.
            /// The uiParam parameter specifies the new value.
            /// </summary>
            SPI_SETBORDER = 0x0006,

            /// <summary>
            /// Retrieves the keyboard repeat-speed setting, which is a value in the range from 0 (approximately 2.5 repetitions per second)
            /// through 31 (approximately 30 repetitions per second). The actual repeat rates are hardware-dependent and may vary from
            /// a linear scale by as much as 20%. The pvParam parameter must point to a DWORD variable that receives the setting
            /// </summary>
            SPI_GETKEYBOARDSPEED = 0x000A,

            /// <summary>
            /// Sets the keyboard repeat-speed setting. The uiParam parameter must specify a value in the range from 0
            /// (approximately 2.5 repetitions per second) through 31 (approximately 30 repetitions per second).
            /// The actual repeat rates are hardware-dependent and may vary from a linear scale by as much as 20%.
            /// If uiParam is greater than 31, the parameter is set to 31.
            /// </summary>
            SPI_SETKEYBOARDSPEED = 0x000B,

            /// <summary>
            /// Not implemented.
            /// </summary>
            SPI_LANGDRIVER = 0x000C,

            /// <summary>
            /// Sets or retrieves the width, in pixels, of an icon cell. The system uses this rectangle to arrange icons in large icon view.
            /// To set this value, set uiParam to the new value and set pvParam to null. You cannot set this value to less than SM_CXICON.
            /// To retrieve this value, pvParam must point to an integer that receives the current value.
            /// </summary>
            SPI_ICONHORIZONTALSPACING = 0x000D,

            /// <summary>
            /// Retrieves the screen saver time-out value, in seconds. The pvParam parameter must point to an integer variable that receives the value.
            /// </summary>
            SPI_GETSCREENSAVETIMEOUT = 0x000E,

            /// <summary>
            /// Sets the screen saver time-out value to the value of the uiParam parameter. This value is the amount of time, in seconds,
            /// that the system must be idle before the screen saver activates.
            /// </summary>
            SPI_SETSCREENSAVETIMEOUT = 0x000F,

            /// <summary>
            /// Determines whether screen saving is enabled. The pvParam parameter must point to a bool variable that receives TRUE
            /// if screen saving is enabled, or FALSE otherwise.
            /// </summary>
            SPI_GETSCREENSAVEACTIVE = 0x0010,

            /// <summary>
            /// Sets the state of the screen saver. The uiParam parameter specifies TRUE to activate screen saving, or FALSE to deactivate it.
            /// </summary>
            SPI_SETSCREENSAVEACTIVE = 0x0011,

            /// <summary>
            /// Retrieves the current granularity value of the desktop sizing grid. The pvParam parameter must point to an integer variable
            /// that receives the granularity.
            /// </summary>
            SPI_GETGRIDGRANULARITY = 0x0012,

            /// <summary>
            /// Sets the granularity of the desktop sizing grid to the value of the uiParam parameter.
            /// </summary>
            SPI_SETGRIDGRANULARITY = 0x0013,

            /// <summary>
            /// Sets the desktop wallpaper. The value of the pvParam parameter determines the new wallpaper. To specify a wallpaper bitmap,
            /// set pvParam to point to a null-terminated string containing the name of a bitmap file. Setting pvParam to "" removes the wallpaper.
            /// Setting pvParam to SETWALLPAPER_DEFAULT or null reverts to the default wallpaper.
            /// </summary>
            SPI_SETDESKWALLPAPER = 0x0014,

            /// <summary>
            /// Sets the current desktop pattern by causing Windows to read the Pattern= setting from the WIN.INI file.
            /// </summary>
            SPI_SETDESKPATTERN = 0x0015,

            /// <summary>
            /// Retrieves the keyboard repeat-delay setting, which is a value in the range from 0 (approximately 250 ms delay) through 3
            /// (approximately 1 second delay). The actual delay associated with each value may vary depending on the hardware. The pvParam parameter must point to an integer variable that receives the setting.
            /// </summary>
            SPI_GETKEYBOARDDELAY = 0x0016,

            /// <summary>
            /// Sets the keyboard repeat-delay setting. The uiParam parameter must specify 0, 1, 2, or 3, where zero sets the shortest delay
            /// (approximately 250 ms) and 3 sets the longest delay (approximately 1 second). The actual delay associated with each value may
            /// vary depending on the hardware.
            /// </summary>
            SPI_SETKEYBOARDDELAY = 0x0017,

            /// <summary>
            /// Sets or retrieves the height, in pixels, of an icon cell.
            /// To set this value, set uiParam to the new value and set pvParam to null. You cannot set this value to less than SM_CYICON.
            /// To retrieve this value, pvParam must point to an integer that receives the current value.
            /// </summary>
            SPI_ICONVERTICALSPACING = 0x0018,

            /// <summary>
            /// Determines whether icon-title wrapping is enabled. The pvParam parameter must point to a bool variable that receives TRUE
            /// if enabled, or FALSE otherwise.
            /// </summary>
            SPI_GETICONTITLEWRAP = 0x0019,

            /// <summary>
            /// Turns icon-title wrapping on or off. The uiParam parameter specifies TRUE for on, or FALSE for off.
            /// </summary>
            SPI_SETICONTITLEWRAP = 0x001A,

            /// <summary>
            /// Determines whether pop-up menus are left-aligned or right-aligned, relative to the corresponding menu-bar item.
            /// The pvParam parameter must point to a bool variable that receives TRUE if left-aligned, or FALSE otherwise.
            /// </summary>
            SPI_GETMENUDROPALIGNMENT = 0x001B,

            /// <summary>
            /// Sets the alignment value of pop-up menus. The uiParam parameter specifies TRUE for right alignment, or FALSE for left alignment.
            /// </summary>
            SPI_SETMENUDROPALIGNMENT = 0x001C,

            /// <summary>
            /// Sets the width of the double-click rectangle to the value of the uiParam parameter.
            /// The double-click rectangle is the rectangle within which the second click of a double-click must fall for it to be registered
            /// as a double-click.
            /// To retrieve the width of the double-click rectangle, call GetSystemMetrics with the SM_CXDOUBLECLK flag.
            /// </summary>
            SPI_SETDOUBLECLKWIDTH = 0x001D,

            /// <summary>
            /// Sets the height of the double-click rectangle to the value of the uiParam parameter.
            /// The double-click rectangle is the rectangle within which the second click of a double-click must fall for it to be registered
            /// as a double-click.
            /// To retrieve the height of the double-click rectangle, call GetSystemMetrics with the SM_CYDOUBLECLK flag.
            /// </summary>
            SPI_SETDOUBLECLKHEIGHT = 0x001E,

            /// <summary>
            /// Retrieves the logical font information for the current icon-title font. The uiParam parameter specifies the size of a LOGFONT structure,
            /// and the pvParam parameter must point to the LOGFONT structure to fill in.
            /// </summary>
            SPI_GETICONTITLELOGFONT = 0x001F,

            /// <summary>
            /// Sets the double-click time for the mouse to the value of the uiParam parameter. The double-click time is the maximum number
            /// of milliseconds that can occur between the first and second clicks of a double-click. You can also call the SetDoubleClickTime
            /// function to set the double-click time. To get the current double-click time, call the GetDoubleClickTime function.
            /// </summary>
            SPI_SETDOUBLECLICKTIME = 0x0020,

            /// <summary>
            /// Swaps or restores the meaning of the left and right mouse buttons. The uiParam parameter specifies TRUE to swap the meanings
            /// of the buttons, or FALSE to restore their original meanings.
            /// </summary>
            SPI_SETMOUSEBUTTONSWAP = 0x0021,

            /// <summary>
            /// Sets the font that is used for icon titles. The uiParam parameter specifies the size of a LOGFONT structure,
            /// and the pvParam parameter must point to a LOGFONT structure.
            /// </summary>
            SPI_SETICONTITLELOGFONT = 0x0022,

            /// <summary>
            /// This flag is obsolete. Previous versions of the system use this flag to determine whether ALT+TAB fast task switching is enabled.
            /// For Windows 95, Windows 98, and Windows NT version 4.0 and later, fast task switching is always enabled.
            /// </summary>
            SPI_GETFASTTASKSWITCH = 0x0023,

            /// <summary>
            /// This flag is obsolete. Previous versions of the system use this flag to enable or disable ALT+TAB fast task switching.
            /// For Windows 95, Windows 98, and Windows NT version 4.0 and later, fast task switching is always enabled.
            /// </summary>
            SPI_SETFASTTASKSWITCH = 0x0024,

            //#if(WINVER >= 0x0400)
            /// <summary>
            /// Sets dragging of full windows either on or off. The uiParam parameter specifies TRUE for on, or FALSE for off.
            /// Windows 95:  This flag is supported only if Windows Plus! is installed. See SPI_GETWINDOWSEXTENSION.
            /// </summary>
            SPI_SETDRAGFULLWINDOWS = 0x0025,

            /// <summary>
            /// Determines whether dragging of full windows is enabled. The pvParam parameter must point to a BOOL variable that receives TRUE
            /// if enabled, or FALSE otherwise.
            /// Windows 95:  This flag is supported only if Windows Plus! is installed. See SPI_GETWINDOWSEXTENSION.
            /// </summary>
            SPI_GETDRAGFULLWINDOWS = 0x0026,

            /// <summary>
            /// Retrieves the metrics associated with the nonclient area of nonminimized windows. The pvParam parameter must point
            /// to a NONCLIENTMETRICS structure that receives the information. Set the cbSize member of this structure and the uiParam parameter
            /// to sizeof(NONCLIENTMETRICS).
            /// </summary>
            SPI_GETNONCLIENTMETRICS = 0x0029,

            /// <summary>
            /// Sets the metrics associated with the nonclient area of nonminimized windows. The pvParam parameter must point
            /// to a NONCLIENTMETRICS structure that contains the new parameters. Set the cbSize member of this structure
            /// and the uiParam parameter to sizeof(NONCLIENTMETRICS). Also, the lfHeight member of the LOGFONT structure must be a negative value.
            /// </summary>
            SPI_SETNONCLIENTMETRICS = 0x002A,

            /// <summary>
            /// Retrieves the metrics associated with minimized windows. The pvParam parameter must point to a MINIMIZEDMETRICS structure
            /// that receives the information. Set the cbSize member of this structure and the uiParam parameter to sizeof(MINIMIZEDMETRICS).
            /// </summary>
            SPI_GETMINIMIZEDMETRICS = 0x002B,

            /// <summary>
            /// Sets the metrics associated with minimized windows. The pvParam parameter must point to a MINIMIZEDMETRICS structure
            /// that contains the new parameters. Set the cbSize member of this structure and the uiParam parameter to sizeof(MINIMIZEDMETRICS).
            /// </summary>
            SPI_SETMINIMIZEDMETRICS = 0x002C,

            /// <summary>
            /// Retrieves the metrics associated with icons. The pvParam parameter must point to an ICONMETRICS structure that receives
            /// the information. Set the cbSize member of this structure and the uiParam parameter to sizeof(ICONMETRICS).
            /// </summary>
            SPI_GETICONMETRICS = 0x002D,

            /// <summary>
            /// Sets the metrics associated with icons. The pvParam parameter must point to an ICONMETRICS structure that contains
            /// the new parameters. Set the cbSize member of this structure and the uiParam parameter to sizeof(ICONMETRICS).
            /// </summary>
            SPI_SETICONMETRICS = 0x002E,

            /// <summary>
            /// Sets the size of the work area. The work area is the portion of the screen not obscured by the system taskbar
            /// or by application desktop toolbars. The pvParam parameter is a pointer to a RECT structure that specifies the new work area rectangle,
            /// expressed in virtual screen coordinates. In a system with multiple display monitors, the function sets the work area
            /// of the monitor that contains the specified rectangle.
            /// </summary>
            SPI_SETWORKAREA = 0x002F,

            /// <summary>
            /// Retrieves the size of the work area on the primary display monitor. The work area is the portion of the screen not obscured
            /// by the system taskbar or by application desktop toolbars. The pvParam parameter must point to a RECT structure that receives
            /// the coordinates of the work area, expressed in virtual screen coordinates.
            /// To get the work area of a monitor other than the primary display monitor, call the GetMonitorInfo function.
            /// </summary>
            SPI_GETWORKAREA = 0x0030,

            /// <summary>
            /// Windows Me/98/95:  Pen windows is being loaded or unloaded. The uiParam parameter is TRUE when loading and FALSE
            /// when unloading pen windows. The pvParam parameter is null.
            /// </summary>
            SPI_SETPENWINDOWS = 0x0031,

            /// <summary>
            /// Retrieves information about the HighContrast accessibility feature. The pvParam parameter must point to a HIGHCONTRAST structure
            /// that receives the information. Set the cbSize member of this structure and the uiParam parameter to sizeof(HIGHCONTRAST).
            /// For a general discussion, see remarks.
            /// Windows NT:  This value is not supported.
            /// </summary>
            /// <remarks>
            /// There is a difference between the High Contrast color scheme and the High Contrast Mode. The High Contrast color scheme changes
            /// the system colors to colors that have obvious contrast; you switch to this color scheme by using the Display Options in the control panel.
            /// The High Contrast Mode, which uses SPI_GETHIGHCONTRAST and SPI_SETHIGHCONTRAST, advises applications to modify their appearance
            /// for visually-impaired users. It involves such things as audible warning to users and customized color scheme
            /// (using the Accessibility Options in the control panel). For more information, see HIGHCONTRAST on MSDN.
            /// For more information on general accessibility features, see Accessibility on MSDN.
            /// </remarks>
            SPI_GETHIGHCONTRAST = 0x0042,

            /// <summary>
            /// Sets the parameters of the HighContrast accessibility feature. The pvParam parameter must point to a HIGHCONTRAST structure
            /// that contains the new parameters. Set the cbSize member of this structure and the uiParam parameter to sizeof(HIGHCONTRAST).
            /// Windows NT:  This value is not supported.
            /// </summary>
            SPI_SETHIGHCONTRAST = 0x0043,

            /// <summary>
            /// Determines whether the user relies on the keyboard instead of the mouse, and wants applications to display keyboard interfaces
            /// that would otherwise be hidden. The pvParam parameter must point to a BOOL variable that receives TRUE
            /// if the user relies on the keyboard; or FALSE otherwise.
            /// Windows NT:  This value is not supported.
            /// </summary>
            SPI_GETKEYBOARDPREF = 0x0044,

            /// <summary>
            /// Sets the keyboard preference. The uiParam parameter specifies TRUE if the user relies on the keyboard instead of the mouse,
            /// and wants applications to display keyboard interfaces that would otherwise be hidden; uiParam is FALSE otherwise.
            /// Windows NT:  This value is not supported.
            /// </summary>
            SPI_SETKEYBOARDPREF = 0x0045,

            /// <summary>
            /// Determines whether a screen reviewer utility is running. A screen reviewer utility directs textual information to an output device,
            /// such as a speech synthesizer or Braille display. When this flag is set, an application should provide textual information
            /// in situations where it would otherwise present the information graphically.
            /// The pvParam parameter is a pointer to a BOOL variable that receives TRUE if a screen reviewer utility is running, or FALSE otherwise.
            /// Windows NT:  This value is not supported.
            /// </summary>
            SPI_GETSCREENREADER = 0x0046,

            /// <summary>
            /// Determines whether a screen review utility is running. The uiParam parameter specifies TRUE for on, or FALSE for off.
            /// Windows NT:  This value is not supported.
            /// </summary>
            SPI_SETSCREENREADER = 0x0047,

            /// <summary>
            /// Retrieves the animation effects associated with user actions. The pvParam parameter must point to an ANIMATIONINFO structure
            /// that receives the information. Set the cbSize member of this structure and the uiParam parameter to sizeof(ANIMATIONINFO).
            /// </summary>
            SPI_GETANIMATION = 0x0048,

            /// <summary>
            /// Sets the animation effects associated with user actions. The pvParam parameter must point to an ANIMATIONINFO structure
            /// that contains the new parameters. Set the cbSize member of this structure and the uiParam parameter to sizeof(ANIMATIONINFO).
            /// </summary>
            SPI_SETANIMATION = 0x0049,

            /// <summary>
            /// Determines whether the font smoothing feature is enabled. This feature uses font antialiasing to make font curves appear smoother
            /// by painting pixels at different gray levels.
            /// The pvParam parameter must point to a BOOL variable that receives TRUE if the feature is enabled, or FALSE if it is not.
            /// Windows 95:  This flag is supported only if Windows Plus! is installed. See SPI_GETWINDOWSEXTENSION.
            /// </summary>
            SPI_GETFONTSMOOTHING = 0x004A,

            /// <summary>
            /// Enables or disables the font smoothing feature, which uses font antialiasing to make font curves appear smoother
            /// by painting pixels at different gray levels.
            /// To enable the feature, set the uiParam parameter to TRUE. To disable the feature, set uiParam to FALSE.
            /// Windows 95:  This flag is supported only if Windows Plus! is installed. See SPI_GETWINDOWSEXTENSION.
            /// </summary>
            SPI_SETFONTSMOOTHING = 0x004B,

            /// <summary>
            /// Sets the width, in pixels, of the rectangle used to detect the start of a drag operation. Set uiParam to the new value.
            /// To retrieve the drag width, call GetSystemMetrics with the SM_CXDRAG flag.
            /// </summary>
            SPI_SETDRAGWIDTH = 0x004C,

            /// <summary>
            /// Sets the height, in pixels, of the rectangle used to detect the start of a drag operation. Set uiParam to the new value.
            /// To retrieve the drag height, call GetSystemMetrics with the SM_CYDRAG flag.
            /// </summary>
            SPI_SETDRAGHEIGHT = 0x004D,

            /// <summary>
            /// Used internally; applications should not use this value.
            /// </summary>
            SPI_SETHANDHELD = 0x004E,

            /// <summary>
            /// Retrieves the time-out value for the low-power phase of screen saving. The pvParam parameter must point to an integer variable
            /// that receives the value. This flag is supported for 32-bit applications only.
            /// Windows NT, Windows Me/98:  This flag is supported for 16-bit and 32-bit applications.
            /// Windows 95:  This flag is supported for 16-bit applications only.
            /// </summary>
            SPI_GETLOWPOWERTIMEOUT = 0x004F,

            /// <summary>
            /// Retrieves the time-out value for the power-off phase of screen saving. The pvParam parameter must point to an integer variable
            /// that receives the value. This flag is supported for 32-bit applications only.
            /// Windows NT, Windows Me/98:  This flag is supported for 16-bit and 32-bit applications.
            /// Windows 95:  This flag is supported for 16-bit applications only.
            /// </summary>
            SPI_GETPOWEROFFTIMEOUT = 0x0050,

            /// <summary>
            /// Sets the time-out value, in seconds, for the low-power phase of screen saving. The uiParam parameter specifies the new value.
            /// The pvParam parameter must be null. This flag is supported for 32-bit applications only.
            /// Windows NT, Windows Me/98:  This flag is supported for 16-bit and 32-bit applications.
            /// Windows 95:  This flag is supported for 16-bit applications only.
            /// </summary>
            SPI_SETLOWPOWERTIMEOUT = 0x0051,

            /// <summary>
            /// Sets the time-out value, in seconds, for the power-off phase of screen saving. The uiParam parameter specifies the new value.
            /// The pvParam parameter must be null. This flag is supported for 32-bit applications only.
            /// Windows NT, Windows Me/98:  This flag is supported for 16-bit and 32-bit applications.
            /// Windows 95:  This flag is supported for 16-bit applications only.
            /// </summary>
            SPI_SETPOWEROFFTIMEOUT = 0x0052,

            /// <summary>
            /// Determines whether the low-power phase of screen saving is enabled. The pvParam parameter must point to a BOOL variable
            /// that receives TRUE if enabled, or FALSE if disabled. This flag is supported for 32-bit applications only.
            /// Windows NT, Windows Me/98:  This flag is supported for 16-bit and 32-bit applications.
            /// Windows 95:  This flag is supported for 16-bit applications only.
            /// </summary>
            SPI_GETLOWPOWERACTIVE = 0x0053,

            /// <summary>
            /// Determines whether the power-off phase of screen saving is enabled. The pvParam parameter must point to a BOOL variable
            /// that receives TRUE if enabled, or FALSE if disabled. This flag is supported for 32-bit applications only.
            /// Windows NT, Windows Me/98:  This flag is supported for 16-bit and 32-bit applications.
            /// Windows 95:  This flag is supported for 16-bit applications only.
            /// </summary>
            SPI_GETPOWEROFFACTIVE = 0x0054,

            /// <summary>
            /// Activates or deactivates the low-power phase of screen saving. Set uiParam to 1 to activate, or zero to deactivate.
            /// The pvParam parameter must be null. This flag is supported for 32-bit applications only.
            /// Windows NT, Windows Me/98:  This flag is supported for 16-bit and 32-bit applications.
            /// Windows 95:  This flag is supported for 16-bit applications only.
            /// </summary>
            SPI_SETLOWPOWERACTIVE = 0x0055,

            /// <summary>
            /// Activates or deactivates the power-off phase of screen saving. Set uiParam to 1 to activate, or zero to deactivate.
            /// The pvParam parameter must be null. This flag is supported for 32-bit applications only.
            /// Windows NT, Windows Me/98:  This flag is supported for 16-bit and 32-bit applications.
            /// Windows 95:  This flag is supported for 16-bit applications only.
            /// </summary>
            SPI_SETPOWEROFFACTIVE = 0x0056,

            /// <summary>
            /// Reloads the system cursors. Set the uiParam parameter to zero and the pvParam parameter to null.
            /// </summary>
            SPI_SETCURSORS = 0x0057,

            /// <summary>
            /// Reloads the system icons. Set the uiParam parameter to zero and the pvParam parameter to null.
            /// </summary>
            SPI_SETICONS = 0x0058,

            /// <summary>
            /// Retrieves the input locale identifier for the system default input language. The pvParam parameter must point
            /// to an HKL variable that receives this value. For more information, see Languages, Locales, and Keyboard Layouts on MSDN.
            /// </summary>
            SPI_GETDEFAULTINPUTLANG = 0x0059,

            /// <summary>
            /// Sets the default input language for the system shell and applications. The specified language must be displayable
            /// using the current system character set. The pvParam parameter must point to an HKL variable that contains
            /// the input locale identifier for the default language. For more information, see Languages, Locales, and Keyboard Layouts on MSDN.
            /// </summary>
            SPI_SETDEFAULTINPUTLANG = 0x005A,

            /// <summary>
            /// Sets the hot key set for switching between input languages. The uiParam and pvParam parameters are not used.
            /// The value sets the shortcut keys in the keyboard property sheets by reading the registry again. The registry must be set before this flag is used. the path in the registry is \HKEY_CURRENT_USER\keyboard layout\toggle. Valid values are "1" = ALT+SHIFT, "2" = CTRL+SHIFT, and "3" = none.
            /// </summary>
            SPI_SETLANGTOGGLE = 0x005B,

            /// <summary>
            /// Windows 95:  Determines whether the Windows extension, Windows Plus!, is installed. Set the uiParam parameter to 1.
            /// The pvParam parameter is not used. The function returns TRUE if the extension is installed, or FALSE if it is not.
            /// </summary>
            SPI_GETWINDOWSEXTENSION = 0x005C,

            /// <summary>
            /// Enables or disables the Mouse Trails feature, which improves the visibility of mouse cursor movements by briefly showing
            /// a trail of cursors and quickly erasing them.
            /// To disable the feature, set the uiParam parameter to zero or 1. To enable the feature, set uiParam to a value greater than 1
            /// to indicate the number of cursors drawn in the trail.
            /// Windows 2000/NT:  This value is not supported.
            /// </summary>
            SPI_SETMOUSETRAILS = 0x005D,

            /// <summary>
            /// Determines whether the Mouse Trails feature is enabled. This feature improves the visibility of mouse cursor movements
            /// by briefly showing a trail of cursors and quickly erasing them.
            /// The pvParam parameter must point to an integer variable that receives a value. If the value is zero or 1, the feature is disabled.
            /// If the value is greater than 1, the feature is enabled and the value indicates the number of cursors drawn in the trail.
            /// The uiParam parameter is not used.
            /// Windows 2000/NT:  This value is not supported.
            /// </summary>
            SPI_GETMOUSETRAILS = 0x005E,

            /// <summary>
            /// Windows Me/98:  Used internally; applications should not use this flag.
            /// </summary>
            SPI_SETSCREENSAVERRUNNING = 0x0061,

            /// <summary>
            /// Same as SPI_SETSCREENSAVERRUNNING.
            /// </summary>
            SPI_SCREENSAVERRUNNING = SPI_SETSCREENSAVERRUNNING,
            //#endif /* WINVER >= 0x0400 */

            /// <summary>
            /// Retrieves information about the FilterKeys accessibility feature. The pvParam parameter must point to a FILTERKEYS structure
            /// that receives the information. Set the cbSize member of this structure and the uiParam parameter to sizeof(FILTERKEYS).
            /// </summary>
            SPI_GETFILTERKEYS = 0x0032,

            /// <summary>
            /// Sets the parameters of the FilterKeys accessibility feature. The pvParam parameter must point to a FILTERKEYS structure
            /// that contains the new parameters. Set the cbSize member of this structure and the uiParam parameter to sizeof(FILTERKEYS).
            /// </summary>
            SPI_SETFILTERKEYS = 0x0033,

            /// <summary>
            /// Retrieves information about the ToggleKeys accessibility feature. The pvParam parameter must point to a TOGGLEKEYS structure
            /// that receives the information. Set the cbSize member of this structure and the uiParam parameter to sizeof(TOGGLEKEYS).
            /// </summary>
            SPI_GETTOGGLEKEYS = 0x0034,

            /// <summary>
            /// Sets the parameters of the ToggleKeys accessibility feature. The pvParam parameter must point to a TOGGLEKEYS structure
            /// that contains the new parameters. Set the cbSize member of this structure and the uiParam parameter to sizeof(TOGGLEKEYS).
            /// </summary>
            SPI_SETTOGGLEKEYS = 0x0035,

            /// <summary>
            /// Retrieves information about the MouseKeys accessibility feature. The pvParam parameter must point to a MOUSEKEYS structure
            /// that receives the information. Set the cbSize member of this structure and the uiParam parameter to sizeof(MOUSEKEYS).
            /// </summary>
            SPI_GETMOUSEKEYS = 0x0036,

            /// <summary>
            /// Sets the parameters of the MouseKeys accessibility feature. The pvParam parameter must point to a MOUSEKEYS structure
            /// that contains the new parameters. Set the cbSize member of this structure and the uiParam parameter to sizeof(MOUSEKEYS).
            /// </summary>
            SPI_SETMOUSEKEYS = 0x0037,

            /// <summary>
            /// Determines whether the Show Sounds accessibility flag is on or off. If it is on, the user requires an application
            /// to present information visually in situations where it would otherwise present the information only in audible form.
            /// The pvParam parameter must point to a BOOL variable that receives TRUE if the feature is on, or FALSE if it is off.
            /// Using this value is equivalent to calling GetSystemMetrics (SM_SHOWSOUNDS). That is the recommended call.
            /// </summary>
            SPI_GETSHOWSOUNDS = 0x0038,

            /// <summary>
            /// Sets the parameters of the SoundSentry accessibility feature. The pvParam parameter must point to a SOUNDSENTRY structure
            /// that contains the new parameters. Set the cbSize member of this structure and the uiParam parameter to sizeof(SOUNDSENTRY).
            /// </summary>
            SPI_SETSHOWSOUNDS = 0x0039,

            /// <summary>
            /// Retrieves information about the StickyKeys accessibility feature. The pvParam parameter must point to a STICKYKEYS structure
            /// that receives the information. Set the cbSize member of this structure and the uiParam parameter to sizeof(STICKYKEYS).
            /// </summary>
            SPI_GETSTICKYKEYS = 0x003A,

            /// <summary>
            /// Sets the parameters of the StickyKeys accessibility feature. The pvParam parameter must point to a STICKYKEYS structure
            /// that contains the new parameters. Set the cbSize member of this structure and the uiParam parameter to sizeof(STICKYKEYS).
            /// </summary>
            SPI_SETSTICKYKEYS = 0x003B,

            /// <summary>
            /// Retrieves information about the time-out period associated with the accessibility features. The pvParam parameter must point
            /// to an ACCESSTIMEOUT structure that receives the information. Set the cbSize member of this structure and the uiParam parameter
            /// to sizeof(ACCESSTIMEOUT).
            /// </summary>
            SPI_GETACCESSTIMEOUT = 0x003C,

            /// <summary>
            /// Sets the time-out period associated with the accessibility features. The pvParam parameter must point to an ACCESSTIMEOUT
            /// structure that contains the new parameters. Set the cbSize member of this structure and the uiParam parameter to sizeof(ACCESSTIMEOUT).
            /// </summary>
            SPI_SETACCESSTIMEOUT = 0x003D,

            //#if(WINVER >= 0x0400)
            /// <summary>
            /// Windows Me/98/95:  Retrieves information about the SerialKeys accessibility feature. The pvParam parameter must point
            /// to a SERIALKEYS structure that receives the information. Set the cbSize member of this structure and the uiParam parameter
            /// to sizeof(SERIALKEYS).
            /// Windows Server 2003, Windows XP/2000/NT:  Not supported. The user controls this feature through the control panel.
            /// </summary>
            SPI_GETSERIALKEYS = 0x003E,

            /// <summary>
            /// Windows Me/98/95:  Sets the parameters of the SerialKeys accessibility feature. The pvParam parameter must point
            /// to a SERIALKEYS structure that contains the new parameters. Set the cbSize member of this structure and the uiParam parameter
            /// to sizeof(SERIALKEYS).
            /// Windows Server 2003, Windows XP/2000/NT:  Not supported. The user controls this feature through the control panel.
            /// </summary>
            SPI_SETSERIALKEYS = 0x003F,
            //#endif /* WINVER >= 0x0400 */

            /// <summary>
            /// Retrieves information about the SoundSentry accessibility feature. The pvParam parameter must point to a SOUNDSENTRY structure
            /// that receives the information. Set the cbSize member of this structure and the uiParam parameter to sizeof(SOUNDSENTRY).
            /// </summary>
            SPI_GETSOUNDSENTRY = 0x0040,

            /// <summary>
            /// Sets the parameters of the SoundSentry accessibility feature. The pvParam parameter must point to a SOUNDSENTRY structure
            /// that contains the new parameters. Set the cbSize member of this structure and the uiParam parameter to sizeof(SOUNDSENTRY).
            /// </summary>
            SPI_SETSOUNDSENTRY = 0x0041,

            //#if(_WIN32_WINNT >= 0x0400)
            /// <summary>
            /// Determines whether the snap-to-default-button feature is enabled. If enabled, the mouse cursor automatically moves
            /// to the default button, such as OK or Apply, of a dialog box. The pvParam parameter must point to a BOOL variable
            /// that receives TRUE if the feature is on, or FALSE if it is off.
            /// Windows 95:  Not supported.
            /// </summary>
            SPI_GETSNAPTODEFBUTTON = 0x005F,

            /// <summary>
            /// Enables or disables the snap-to-default-button feature. If enabled, the mouse cursor automatically moves to the default button,
            /// such as OK or Apply, of a dialog box. Set the uiParam parameter to TRUE to enable the feature, or FALSE to disable it.
            /// Applications should use the ShowWindow function when displaying a dialog box so the dialog manager can position the mouse cursor.
            /// Windows 95:  Not supported.
            /// </summary>
            SPI_SETSNAPTODEFBUTTON = 0x0060,
            //#endif /* _WIN32_WINNT >= 0x0400 */

            //#if (_WIN32_WINNT >= 0x0400) || (_WIN32_WINDOWS > 0x0400)
            /// <summary>
            /// Retrieves the width, in pixels, of the rectangle within which the mouse pointer has to stay for TrackMouseEvent
            /// to generate a WM_MOUSEHOVER message. The pvParam parameter must point to a UINT variable that receives the width.
            /// Windows 95:  Not supported.
            /// </summary>
            SPI_GETMOUSEHOVERWIDTH = 0x0062,

            /// <summary>
            /// Retrieves the width, in pixels, of the rectangle within which the mouse pointer has to stay for TrackMouseEvent
            /// to generate a WM_MOUSEHOVER message. The pvParam parameter must point to a UINT variable that receives the width.
            /// Windows 95:  Not supported.
            /// </summary>
            SPI_SETMOUSEHOVERWIDTH = 0x0063,

            /// <summary>
            /// Retrieves the height, in pixels, of the rectangle within which the mouse pointer has to stay for TrackMouseEvent
            /// to generate a WM_MOUSEHOVER message. The pvParam parameter must point to a UINT variable that receives the height.
            /// Windows 95:  Not supported.
            /// </summary>
            SPI_GETMOUSEHOVERHEIGHT = 0x0064,

            /// <summary>
            /// Sets the height, in pixels, of the rectangle within which the mouse pointer has to stay for TrackMouseEvent
            /// to generate a WM_MOUSEHOVER message. Set the uiParam parameter to the new height.
            /// Windows 95:  Not supported.
            /// </summary>
            SPI_SETMOUSEHOVERHEIGHT = 0x0065,

            /// <summary>
            /// Retrieves the time, in milliseconds, that the mouse pointer has to stay in the hover rectangle for TrackMouseEvent
            /// to generate a WM_MOUSEHOVER message. The pvParam parameter must point to a UINT variable that receives the time.
            /// Windows 95:  Not supported.
            /// </summary>
            SPI_GETMOUSEHOVERTIME = 0x0066,

            /// <summary>
            /// Sets the time, in milliseconds, that the mouse pointer has to stay in the hover rectangle for TrackMouseEvent
            /// to generate a WM_MOUSEHOVER message. This is used only if you pass HOVER_DEFAULT in the dwHoverTime parameter in the call to TrackMouseEvent. Set the uiParam parameter to the new time.
            /// Windows 95:  Not supported.
            /// </summary>
            SPI_SETMOUSEHOVERTIME = 0x0067,

            /// <summary>
            /// Retrieves the number of lines to scroll when the mouse wheel is rotated. The pvParam parameter must point
            /// to a UINT variable that receives the number of lines. The default value is 3.
            /// Windows 95:  Not supported.
            /// </summary>
            SPI_GETWHEELSCROLLLINES = 0x0068,

            /// <summary>
            /// Sets the number of lines to scroll when the mouse wheel is rotated. The number of lines is set from the uiParam parameter.
            /// The number of lines is the suggested number of lines to scroll when the mouse wheel is rolled without using modifier keys.
            /// If the number is 0, then no scrolling should occur. If the number of lines to scroll is greater than the number of lines viewable,
            /// and in particular if it is WHEEL_PAGESCROLL (#defined as UINT_MAX), the scroll operation should be interpreted
            /// as clicking once in the page down or page up regions of the scroll bar.
            /// Windows 95:  Not supported.
            /// </summary>
            SPI_SETWHEELSCROLLLINES = 0x0069,

            /// <summary>
            /// Retrieves the time, in milliseconds, that the system waits before displaying a shortcut menu when the mouse cursor is
            /// over a submenu item. The pvParam parameter must point to a DWORD variable that receives the time of the delay.
            /// Windows 95:  Not supported.
            /// </summary>
            SPI_GETMENUSHOWDELAY = 0x006A,

            /// <summary>
            /// Sets uiParam to the time, in milliseconds, that the system waits before displaying a shortcut menu when the mouse cursor is
            /// over a submenu item.
            /// Windows 95:  Not supported.
            /// </summary>
            SPI_SETMENUSHOWDELAY = 0x006B,

            /// <summary>
            /// Determines whether the IME status window is visible (on a per-user basis). The pvParam parameter must point to a BOOL variable
            /// that receives TRUE if the status window is visible, or FALSE if it is not.
            /// Windows NT, Windows 95:  This value is not supported.
            /// </summary>
            SPI_GETSHOWIMEUI = 0x006E,

            /// <summary>
            /// Sets whether the IME status window is visible or not on a per-user basis. The uiParam parameter specifies TRUE for on or FALSE for off.
            /// Windows NT, Windows 95:  This value is not supported.
            /// </summary>
            SPI_SETSHOWIMEUI = 0x006F,
            //#endif

            //#if(WINVER >= 0x0500)
            /// <summary>
            /// Retrieves the current mouse speed. The mouse speed determines how far the pointer will move based on the distance the mouse moves.
            /// The pvParam parameter must point to an integer that receives a value which ranges between 1 (slowest) and 20 (fastest).
            /// A value of 10 is the default. The value can be set by an end user using the mouse control panel application or
            /// by an application using SPI_SETMOUSESPEED.
            /// Windows NT, Windows 95:  This value is not supported.
            /// </summary>
            SPI_GETMOUSESPEED = 0x0070,

            /// <summary>
            /// Sets the current mouse speed. The pvParam parameter is an integer between 1 (slowest) and 20 (fastest). A value of 10 is the default.
            /// This value is typically set using the mouse control panel application.
            /// Windows NT, Windows 95:  This value is not supported.
            /// </summary>
            SPI_SETMOUSESPEED = 0x0071,

            /// <summary>
            /// Determines whether a screen saver is currently running on the window station of the calling process.
            /// The pvParam parameter must point to a BOOL variable that receives TRUE if a screen saver is currently running, or FALSE otherwise.
            /// Note that only the interactive window station, "WinSta0", can have a screen saver running.
            /// Windows NT, Windows 95:  This value is not supported.
            /// </summary>
            SPI_GETSCREENSAVERRUNNING = 0x0072,

            /// <summary>
            /// Retrieves the full path of the bitmap file for the desktop wallpaper. The pvParam parameter must point to a buffer
            /// that receives a null-terminated path string. Set the uiParam parameter to the size, in characters, of the pvParam buffer. The returned string will not exceed MAX_PATH characters. If there is no desktop wallpaper, the returned string is empty.
            /// Windows NT, Windows Me/98/95:  This value is not supported.
            /// </summary>
            SPI_GETDESKWALLPAPER = 0x0073,
            //#endif /* WINVER >= 0x0500 */

            //#if(WINVER >= 0x0500)
            /// <summary>
            /// Determines whether active window tracking (activating the window the mouse is on) is on or off. The pvParam parameter must point
            /// to a BOOL variable that receives TRUE for on, or FALSE for off.
            /// Windows NT, Windows 95:  This value is not supported.
            /// </summary>
            SPI_GETACTIVEWINDOWTRACKING = 0x1000,

            /// <summary>
            /// Sets active window tracking (activating the window the mouse is on) either on or off. Set pvParam to TRUE for on or FALSE for off.
            /// Windows NT, Windows 95:  This value is not supported.
            /// </summary>
            SPI_SETACTIVEWINDOWTRACKING = 0x1001,

            /// <summary>
            /// Determines whether the menu animation feature is enabled. This master switch must be on to enable menu animation effects.
            /// The pvParam parameter must point to a BOOL variable that receives TRUE if animation is enabled and FALSE if it is disabled.
            /// If animation is enabled, SPI_GETMENUFADE indicates whether menus use fade or slide animation.
            /// Windows NT, Windows 95:  This value is not supported.
            /// </summary>
            SPI_GETMENUANIMATION = 0x1002,

            /// <summary>
            /// Enables or disables menu animation. This master switch must be on for any menu animation to occur.
            /// The pvParam parameter is a BOOL variable; set pvParam to TRUE to enable animation and FALSE to disable animation.
            /// If animation is enabled, SPI_GETMENUFADE indicates whether menus use fade or slide animation.
            /// Windows NT, Windows 95:  This value is not supported.
            /// </summary>
            SPI_SETMENUANIMATION = 0x1003,

            /// <summary>
            /// Determines whether the slide-open effect for combo boxes is enabled. The pvParam parameter must point to a BOOL variable
            /// that receives TRUE for enabled, or FALSE for disabled.
            /// Windows NT, Windows 95:  This value is not supported.
            /// </summary>
            SPI_GETCOMBOBOXANIMATION = 0x1004,

            /// <summary>
            /// Enables or disables the slide-open effect for combo boxes. Set the pvParam parameter to TRUE to enable the gradient effect,
            /// or FALSE to disable it.
            /// Windows NT, Windows 95:  This value is not supported.
            /// </summary>
            SPI_SETCOMBOBOXANIMATION = 0x1005,

            /// <summary>
            /// Determines whether the smooth-scrolling effect for list boxes is enabled. The pvParam parameter must point to a BOOL variable
            /// that receives TRUE for enabled, or FALSE for disabled.
            /// Windows NT, Windows 95:  This value is not supported.
            /// </summary>
            SPI_GETLISTBOXSMOOTHSCROLLING = 0x1006,

            /// <summary>
            /// Enables or disables the smooth-scrolling effect for list boxes. Set the pvParam parameter to TRUE to enable the smooth-scrolling effect,
            /// or FALSE to disable it.
            /// Windows NT, Windows 95:  This value is not supported.
            /// </summary>
            SPI_SETLISTBOXSMOOTHSCROLLING = 0x1007,

            /// <summary>
            /// Determines whether the gradient effect for window title bars is enabled. The pvParam parameter must point to a BOOL variable
            /// that receives TRUE for enabled, or FALSE for disabled. For more information about the gradient effect, see the GetSysColor function.
            /// Windows NT, Windows 95:  This value is not supported.
            /// </summary>
            SPI_GETGRADIENTCAPTIONS = 0x1008,

            /// <summary>
            /// Enables or disables the gradient effect for window title bars. Set the pvParam parameter to TRUE to enable it, or FALSE to disable it.
            /// The gradient effect is possible only if the system has a color depth of more than 256 colors. For more information about
            /// the gradient effect, see the GetSysColor function.
            /// Windows NT, Windows 95:  This value is not supported.
            /// </summary>
            SPI_SETGRADIENTCAPTIONS = 0x1009,

            /// <summary>
            /// Determines whether menu access keys are always underlined. The pvParam parameter must point to a BOOL variable that receives TRUE
            /// if menu access keys are always underlined, and FALSE if they are underlined only when the menu is activated by the keyboard.
            /// Windows NT, Windows 95:  This value is not supported.
            /// </summary>
            SPI_GETKEYBOARDCUES = 0x100A,

            /// <summary>
            /// Sets the underlining of menu access key letters. The pvParam parameter is a BOOL variable. Set pvParam to TRUE to always underline menu
            /// access keys, or FALSE to underline menu access keys only when the menu is activated from the keyboard.
            /// Windows NT, Windows 95:  This value is not supported.
            /// </summary>
            SPI_SETKEYBOARDCUES = 0x100B,

            /// <summary>
            /// Same as SPI_GETKEYBOARDCUES.
            /// </summary>
            SPI_GETMENUUNDERLINES = SPI_GETKEYBOARDCUES,

            /// <summary>
            /// Same as SPI_SETKEYBOARDCUES.
            /// </summary>
            SPI_SETMENUUNDERLINES = SPI_SETKEYBOARDCUES,

            /// <summary>
            /// Determines whether windows activated through active window tracking will be brought to the top. The pvParam parameter must point
            /// to a BOOL variable that receives TRUE for on, or FALSE for off.
            /// Windows NT, Windows 95:  This value is not supported.
            /// </summary>
            SPI_GETACTIVEWNDTRKZORDER = 0x100C,

            /// <summary>
            /// Determines whether or not windows activated through active window tracking should be brought to the top. Set pvParam to TRUE
            /// for on or FALSE for off.
            /// Windows NT, Windows 95:  This value is not supported.
            /// </summary>
            SPI_SETACTIVEWNDTRKZORDER = 0x100D,

            /// <summary>
            /// Determines whether hot tracking of user-interface elements, such as menu names on menu bars, is enabled. The pvParam parameter
            /// must point to a BOOL variable that receives TRUE for enabled, or FALSE for disabled.
            /// Hot tracking means that when the cursor moves over an item, it is highlighted but not selected. You can query this value to decide
            /// whether to use hot tracking in the user interface of your application.
            /// Windows NT, Windows 95:  This value is not supported.
            /// </summary>
            SPI_GETHOTTRACKING = 0x100E,

            /// <summary>
            /// Enables or disables hot tracking of user-interface elements such as menu names on menu bars. Set the pvParam parameter to TRUE
            /// to enable it, or FALSE to disable it.
            /// Hot-tracking means that when the cursor moves over an item, it is highlighted but not selected.
            /// Windows NT, Windows 95:  This value is not supported.
            /// </summary>
            SPI_SETHOTTRACKING = 0x100F,

            /// <summary>
            /// Determines whether menu fade animation is enabled. The pvParam parameter must point to a BOOL variable that receives TRUE
            /// when fade animation is enabled and FALSE when it is disabled. If fade animation is disabled, menus use slide animation.
            /// This flag is ignored unless menu animation is enabled, which you can do using the SPI_SETMENUANIMATION flag.
            /// For more information, see AnimateWindow.
            /// Windows NT, Windows Me/98/95:  This value is not supported.
            /// </summary>
            SPI_GETMENUFADE = 0x1012,

            /// <summary>
            /// Enables or disables menu fade animation. Set pvParam to TRUE to enable the menu fade effect or FALSE to disable it.
            /// If fade animation is disabled, menus use slide animation. he The menu fade effect is possible only if the system
            /// has a color depth of more than 256 colors. This flag is ignored unless SPI_MENUANIMATION is also set. For more information,
            /// see AnimateWindow.
            /// Windows NT, Windows Me/98/95:  This value is not supported.
            /// </summary>
            SPI_SETMENUFADE = 0x1013,

            /// <summary>
            /// Determines whether the selection fade effect is enabled. The pvParam parameter must point to a BOOL variable that receives TRUE
            /// if enabled or FALSE if disabled.
            /// The selection fade effect causes the menu item selected by the user to remain on the screen briefly while fading out
            /// after the menu is dismissed.
            /// Windows NT, Windows Me/98/95:  This value is not supported.
            /// </summary>
            SPI_GETSELECTIONFADE = 0x1014,

            /// <summary>
            /// Set pvParam to TRUE to enable the selection fade effect or FALSE to disable it.
            /// The selection fade effect causes the menu item selected by the user to remain on the screen briefly while fading out
            /// after the menu is dismissed. The selection fade effect is possible only if the system has a color depth of more than 256 colors.
            /// Windows NT, Windows Me/98/95:  This value is not supported.
            /// </summary>
            SPI_SETSELECTIONFADE = 0x1015,

            /// <summary>
            /// Determines whether ToolTip animation is enabled. The pvParam parameter must point to a BOOL variable that receives TRUE
            /// if enabled or FALSE if disabled. If ToolTip animation is enabled, SPI_GETTOOLTIPFADE indicates whether ToolTips use fade or slide animation.
            /// Windows NT, Windows Me/98/95:  This value is not supported.
            /// </summary>
            SPI_GETTOOLTIPANIMATION = 0x1016,

            /// <summary>
            /// Set pvParam to TRUE to enable ToolTip animation or FALSE to disable it. If enabled, you can use SPI_SETTOOLTIPFADE
            /// to specify fade or slide animation.
            /// Windows NT, Windows Me/98/95:  This value is not supported.
            /// </summary>
            SPI_SETTOOLTIPANIMATION = 0x1017,

            /// <summary>
            /// If SPI_SETTOOLTIPANIMATION is enabled, SPI_GETTOOLTIPFADE indicates whether ToolTip animation uses a fade effect or a slide effect.
            ///  The pvParam parameter must point to a BOOL variable that receives TRUE for fade animation or FALSE for slide animation.
            ///  For more information on slide and fade effects, see AnimateWindow.
            /// Windows NT, Windows Me/98/95:  This value is not supported.
            /// </summary>
            SPI_GETTOOLTIPFADE = 0x1018,

            /// <summary>
            /// If the SPI_SETTOOLTIPANIMATION flag is enabled, use SPI_SETTOOLTIPFADE to indicate whether ToolTip animation uses a fade effect
            /// or a slide effect. Set pvParam to TRUE for fade animation or FALSE for slide animation. The tooltip fade effect is possible only
            /// if the system has a color depth of more than 256 colors. For more information on the slide and fade effects,
            /// see the AnimateWindow function.
            /// Windows NT, Windows Me/98/95:  This value is not supported.
            /// </summary>
            SPI_SETTOOLTIPFADE = 0x1019,

            /// <summary>
            /// Determines whether the cursor has a shadow around it. The pvParam parameter must point to a BOOL variable that receives TRUE
            /// if the shadow is enabled, FALSE if it is disabled. This effect appears only if the system has a color depth of more than 256 colors.
            /// Windows NT, Windows Me/98/95:  This value is not supported.
            /// </summary>
            SPI_GETCURSORSHADOW = 0x101A,

            /// <summary>
            /// Enables or disables a shadow around the cursor. The pvParam parameter is a BOOL variable. Set pvParam to TRUE to enable the shadow
            /// or FALSE to disable the shadow. This effect appears only if the system has a color depth of more than 256 colors.
            /// Windows NT, Windows Me/98/95:  This value is not supported.
            /// </summary>
            SPI_SETCURSORSHADOW = 0x101B,

            //#if(_WIN32_WINNT >= 0x0501)
            /// <summary>
            /// Retrieves the state of the Mouse Sonar feature. The pvParam parameter must point to a BOOL variable that receives TRUE
            /// if enabled or FALSE otherwise. For more information, see About Mouse Input on MSDN.
            /// Windows 2000/NT, Windows 98/95:  This value is not supported.
            /// </summary>
            SPI_GETMOUSESONAR = 0x101C,

            /// <summary>
            /// Turns the Sonar accessibility feature on or off. This feature briefly shows several concentric circles around the mouse pointer
            /// when the user presses and releases the CTRL key. The pvParam parameter specifies TRUE for on and FALSE for off. The default is off.
            /// For more information, see About Mouse Input.
            /// Windows 2000/NT, Windows 98/95:  This value is not supported.
            /// </summary>
            SPI_SETMOUSESONAR = 0x101D,

            /// <summary>
            /// Retrieves the state of the Mouse ClickLock feature. The pvParam parameter must point to a BOOL variable that receives TRUE
            /// if enabled, or FALSE otherwise. For more information, see About Mouse Input.
            /// Windows 2000/NT, Windows 98/95:  This value is not supported.
            /// </summary>
            SPI_GETMOUSECLICKLOCK = 0x101E,

            /// <summary>
            /// Turns the Mouse ClickLock accessibility feature on or off. This feature temporarily locks down the primary mouse button
            /// when that button is clicked and held down for the time specified by SPI_SETMOUSECLICKLOCKTIME. The uiParam parameter specifies
            /// TRUE for on,
            /// or FALSE for off. The default is off. For more information, see Remarks and About Mouse Input on MSDN.
            /// Windows 2000/NT, Windows 98/95:  This value is not supported.
            /// </summary>
            SPI_SETMOUSECLICKLOCK = 0x101F,

            /// <summary>
            /// Retrieves the state of the Mouse Vanish feature. The pvParam parameter must point to a BOOL variable that receives TRUE
            /// if enabled or FALSE otherwise. For more information, see About Mouse Input on MSDN.
            /// Windows 2000/NT, Windows 98/95:  This value is not supported.
            /// </summary>
            SPI_GETMOUSEVANISH = 0x1020,

            /// <summary>
            /// Turns the Vanish feature on or off. This feature hides the mouse pointer when the user types; the pointer reappears
            /// when the user moves the mouse. The pvParam parameter specifies TRUE for on and FALSE for off. The default is off.
            /// For more information, see About Mouse Input on MSDN.
            /// Windows 2000/NT, Windows 98/95:  This value is not supported.
            /// </summary>
            SPI_SETMOUSEVANISH = 0x1021,

            /// <summary>
            /// Determines whether native User menus have flat menu appearance. The pvParam parameter must point to a BOOL variable
            /// that returns TRUE if the flat menu appearance is set, or FALSE otherwise.
            /// Windows 2000/NT, Windows Me/98/95:  This value is not supported.
            /// </summary>
            SPI_GETFLATMENU = 0x1022,

            /// <summary>
            /// Enables or disables flat menu appearance for native User menus. Set pvParam to TRUE to enable flat menu appearance
            /// or FALSE to disable it.
            /// When enabled, the menu bar uses COLOR_MENUBAR for the menubar background, COLOR_MENU for the menu-popup background, COLOR_MENUHILIGHT
            /// for the fill of the current menu selection, and COLOR_HILIGHT for the outline of the current menu selection.
            /// If disabled, menus are drawn using the same metrics and colors as in Windows 2000 and earlier.
            /// Windows 2000/NT, Windows Me/98/95:  This value is not supported.
            /// </summary>
            SPI_SETFLATMENU = 0x1023,

            /// <summary>
            /// Determines whether the drop shadow effect is enabled. The pvParam parameter must point to a BOOL variable that returns TRUE
            /// if enabled or FALSE if disabled.
            /// Windows 2000/NT, Windows Me/98/95:  This value is not supported.
            /// </summary>
            SPI_GETDROPSHADOW = 0x1024,

            /// <summary>
            /// Enables or disables the drop shadow effect. Set pvParam to TRUE to enable the drop shadow effect or FALSE to disable it.
            /// You must also have CS_DROPSHADOW in the window class style.
            /// Windows 2000/NT, Windows Me/98/95:  This value is not supported.
            /// </summary>
            SPI_SETDROPSHADOW = 0x1025,

            /// <summary>
            /// Retrieves a BOOL indicating whether an application can reset the screensaver's timer by calling the SendInput function
            /// to simulate keyboard or mouse input. The pvParam parameter must point to a BOOL variable that receives TRUE
            /// if the simulated input will be blocked, or FALSE otherwise.
            /// </summary>
            SPI_GETBLOCKSENDINPUTRESETS = 0x1026,

            /// <summary>
            /// Determines whether an application can reset the screensaver's timer by calling the SendInput function to simulate keyboard
            /// or mouse input. The uiParam parameter specifies TRUE if the screensaver will not be deactivated by simulated input,
            /// or FALSE if the screensaver will be deactivated by simulated input.
            /// </summary>
            SPI_SETBLOCKSENDINPUTRESETS = 0x1027,
            //#endif /* _WIN32_WINNT >= 0x0501 */

            /// <summary>
            /// Determines whether UI effects are enabled or disabled. The pvParam parameter must point to a BOOL variable that receives TRUE
            /// if all UI effects are enabled, or FALSE if they are disabled.
            /// Windows NT, Windows Me/98/95:  This value is not supported.
            /// </summary>
            SPI_GETUIEFFECTS = 0x103E,

            /// <summary>
            /// Enables or disables UI effects. Set the pvParam parameter to TRUE to enable all UI effects or FALSE to disable all UI effects.
            /// Windows NT, Windows Me/98/95:  This value is not supported.
            /// </summary>
            SPI_SETUIEFFECTS = 0x103F,

            /// <summary>
            /// Retrieves the amount of time following user input, in milliseconds, during which the system will not allow applications
            /// to force themselves into the foreground. The pvParam parameter must point to a DWORD variable that receives the time.
            /// Windows NT, Windows 95:  This value is not supported.
            /// </summary>
            SPI_GETFOREGROUNDLOCKTIMEOUT = 0x2000,

            /// <summary>
            /// Sets the amount of time following user input, in milliseconds, during which the system does not allow applications
            /// to force themselves into the foreground. Set pvParam to the new timeout value.
            /// The calling thread must be able to change the foreground window, otherwise the call fails.
            /// Windows NT, Windows 95:  This value is not supported.
            /// </summary>
            SPI_SETFOREGROUNDLOCKTIMEOUT = 0x2001,

            /// <summary>
            /// Retrieves the active window tracking delay, in milliseconds. The pvParam parameter must point to a DWORD variable
            /// that receives the time.
            /// Windows NT, Windows 95:  This value is not supported.
            /// </summary>
            SPI_GETACTIVEWNDTRKTIMEOUT = 0x2002,

            /// <summary>
            /// Sets the active window tracking delay. Set pvParam to the number of milliseconds to delay before activating the window
            /// under the mouse pointer.
            /// Windows NT, Windows 95:  This value is not supported.
            /// </summary>
            SPI_SETACTIVEWNDTRKTIMEOUT = 0x2003,

            /// <summary>
            /// Retrieves the number of times SetForegroundWindow will flash the taskbar button when rejecting a foreground switch request.
            /// The pvParam parameter must point to a DWORD variable that receives the value.
            /// Windows NT, Windows 95:  This value is not supported.
            /// </summary>
            SPI_GETFOREGROUNDFLASHCOUNT = 0x2004,

            /// <summary>
            /// Sets the number of times SetForegroundWindow will flash the taskbar button when rejecting a foreground switch request.
            /// Set pvParam to the number of times to flash.
            /// Windows NT, Windows 95:  This value is not supported.
            /// </summary>
            SPI_SETFOREGROUNDFLASHCOUNT = 0x2005,

            /// <summary>
            /// Retrieves the caret width in edit controls, in pixels. The pvParam parameter must point to a DWORD that receives this value.
            /// Windows NT, Windows Me/98/95:  This value is not supported.
            /// </summary>
            SPI_GETCARETWIDTH = 0x2006,

            /// <summary>
            /// Sets the caret width in edit controls. Set pvParam to the desired width, in pixels. The default and minimum value is 1.
            /// Windows NT, Windows Me/98/95:  This value is not supported.
            /// </summary>
            SPI_SETCARETWIDTH = 0x2007,

            //#if(_WIN32_WINNT >= 0x0501)
            /// <summary>
            /// Retrieves the time delay before the primary mouse button is locked. The pvParam parameter must point to DWORD that receives
            /// the time delay. This is only enabled if SPI_SETMOUSECLICKLOCK is set to TRUE. For more information, see About Mouse Input on MSDN.
            /// Windows 2000/NT, Windows 98/95:  This value is not supported.
            /// </summary>
            SPI_GETMOUSECLICKLOCKTIME = 0x2008,

            /// <summary>
            /// Turns the Mouse ClickLock accessibility feature on or off. This feature temporarily locks down the primary mouse button
            /// when that button is clicked and held down for the time specified by SPI_SETMOUSECLICKLOCKTIME. The uiParam parameter
            /// specifies TRUE for on, or FALSE for off. The default is off. For more information, see Remarks and About Mouse Input on MSDN.
            /// Windows 2000/NT, Windows 98/95:  This value is not supported.
            /// </summary>
            SPI_SETMOUSECLICKLOCKTIME = 0x2009,

            /// <summary>
            /// Retrieves the type of font smoothing. The pvParam parameter must point to a UINT that receives the information.
            /// Windows 2000/NT, Windows Me/98/95:  This value is not supported.
            /// </summary>
            SPI_GETFONTSMOOTHINGTYPE = 0x200A,

            /// <summary>
            /// Sets the font smoothing type. The pvParam parameter points to a UINT that contains either FE_FONTSMOOTHINGSTANDARD,
            /// if standard anti-aliasing is used, or FE_FONTSMOOTHINGCLEARTYPE, if ClearType is used. The default is FE_FONTSMOOTHINGSTANDARD.
            /// When using this option, the fWinIni parameter must be set to SPIF_SENDWININICHANGE | SPIF_UPDATEINIFILE; otherwise,
            /// SystemParametersInfo fails.
            /// </summary>
            SPI_SETFONTSMOOTHINGTYPE = 0x200B,

            /// <summary>
            /// Retrieves a contrast value that is used in ClearType™ smoothing. The pvParam parameter must point to a UINT
            /// that receives the information.
            /// Windows 2000/NT, Windows Me/98/95:  This value is not supported.
            /// </summary>
            SPI_GETFONTSMOOTHINGCONTRAST = 0x200C,

            /// <summary>
            /// Sets the contrast value used in ClearType smoothing. The pvParam parameter points to a UINT that holds the contrast value.
            /// Valid contrast values are from 1000 to 2200. The default value is 1400.
            /// When using this option, the fWinIni parameter must be set to SPIF_SENDWININICHANGE | SPIF_UPDATEINIFILE; otherwise,
            /// SystemParametersInfo fails.
            /// SPI_SETFONTSMOOTHINGTYPE must also be set to FE_FONTSMOOTHINGCLEARTYPE.
            /// Windows 2000/NT, Windows Me/98/95:  This value is not supported.
            /// </summary>
            SPI_SETFONTSMOOTHINGCONTRAST = 0x200D,

            /// <summary>
            /// Retrieves the width, in pixels, of the left and right edges of the focus rectangle drawn with DrawFocusRect.
            /// The pvParam parameter must point to a UINT.
            /// Windows 2000/NT, Windows Me/98/95:  This value is not supported.
            /// </summary>
            SPI_GETFOCUSBORDERWIDTH = 0x200E,

            /// <summary>
            /// Sets the height of the left and right edges of the focus rectangle drawn with DrawFocusRect to the value of the pvParam parameter.
            /// Windows 2000/NT, Windows Me/98/95:  This value is not supported.
            /// </summary>
            SPI_SETFOCUSBORDERWIDTH = 0x200F,

            /// <summary>
            /// Retrieves the height, in pixels, of the top and bottom edges of the focus rectangle drawn with DrawFocusRect.
            /// The pvParam parameter must point to a UINT.
            /// Windows 2000/NT, Windows Me/98/95:  This value is not supported.
            /// </summary>
            SPI_GETFOCUSBORDERHEIGHT = 0x2010,

            /// <summary>
            /// Sets the height of the top and bottom edges of the focus rectangle drawn with DrawFocusRect to the value of the pvParam parameter.
            /// Windows 2000/NT, Windows Me/98/95:  This value is not supported.
            /// </summary>
            SPI_SETFOCUSBORDERHEIGHT = 0x2011,

            /// <summary>
            /// Not implemented.
            /// </summary>
            SPI_GETFONTSMOOTHINGORIENTATION = 0x2012,

            /// <summary>
            /// Not implemented.
            /// </summary>
            SPI_SETFONTSMOOTHINGORIENTATION = 0x2013,
        }

        #endregion // SPI

        [Flags]
        public enum SPIF
        {
            None = 0x00,
            /// <summary>Writes the new system-wide parameter setting to the user profile.</summary>
            SPIF_UPDATEINIFILE = 0x01,
            /// <summary>Broadcasts the WM_SETTINGCHANGE message after updating the user profile.</summary>
            SPIF_SENDCHANGE = 0x02,
            /// <summary>Same as SPIF_SENDCHANGE.</summary>
            SPIF_SENDWININICHANGE = 0x02
        }

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetWindowInfo(IntPtr hwnd, ref WINDOWINFO pwi);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool EnumDisplayDevices(string lpDevice, uint iDevNum, ref DisplayDevice lpDisplayDevice, uint dwFlags);

        [DllImport("user32.dll")]
        public static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, EnumMonitorsDelegate lpfnEnum, IntPtr dwData);

        public delegate bool EnumMonitorsDelegate(IntPtr hMonitor, IntPtr hdcMonitor, ref Rect lprcMonitor, IntPtr dwData);

        [DllImport("user32.dll")]
        public static extern bool GetMonitorInfo(IntPtr hMonitor, ref MonitorInfoEx lpmi);

        [DllImport("user32.dll")]
        public static extern bool GetMonitorInfo(IntPtr hMonitor, ref MonitorInfo lpmi);

        /// <summary>
        ///     Defines a new window message that is guaranteed to be unique throughout the system. The message value can be used
        ///     when sending or posting messages.
        ///     <para>
        ///     Go to https://msdn.microsoft.com/en-us/library/windows/desktop/ms644947%28v=vs.85%29.aspx for more
        ///     information.
        ///     </para>
        /// </summary>
        /// <param name="msg">C++ ( lpString [in]. Type: LPCTSTR )<br /> The message to be registered.</param>
        /// <returns>
        ///     C++ ( Type: UINT )<br /> If the message is successfully registered, the return value is a message identifier in the
        ///     range 0xC000 through 0xFFFF. If the function fails, the return value is zero.<br /><br /> To get extended error
        ///     information, call GetLastError.
        /// </returns>
        /// <remarks>
        ///     The <see cref="RegisterWindowMessage" /> function is typically used to register messages for communicating between
        ///     two cooperating applications. If two different applications register the same message string, the applications
        ///     return the same message value.The message remains registered until the session ends. Only use
        ///     <see cref="RegisterWindowMessage" /> when more than one application must process the same message.For sending
        ///     private messages within a window class, an application can use any integer in the range WM_USER through 0x7FFF.
        ///     <br />(Messages in this range are private to a window class, not to an application.For example, predefined control
        ///     classes such as BUTTON, EDIT, LISTBOX, and COMBOBOX may use values in this range.)
        /// </remarks>
        /// <example>
        ///     <code><![CDATA[
        ///  //provide a private internal message id
        ///  private UInt32 queryCancelAutoPlay = 0;
        ///  
        ///  [DllImport("user32.dll", SetLastError=true, CharSet=CharSet.Auto)]
        /// static extern uint RegisterWindowMessage(string lpString);
        ///  
        ///  /* only needed if your application is using a dialog box and needs to respond to a "QueryCancelAutoPlay" message, it cannot simply return TRUE or FALSE.
        ///      [DllImport("user32.dll")]
        ///      static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        /// */
        ///
        /// protected override void WndProc(ref Message m)
        /// {
        ///      //calling the base first is important, otherwise the values you set later will be lost
        ///      base.WndProc(ref m);
        ///      
        ///      //if the QueryCancelAutoPlay message id has not been registered...
        ///      if (queryCancelAutoPlay == 0)
        ///      queryCancelAutoPlay = RegisterWindowMessage("QueryCancelAutoPlay");
        ///      
        ///      //if the window message id equals the QueryCancelAutoPlay message id
        ///      if ((UInt32)m.Msg == queryCancelAutoPlay)
        ///      {
        ///      /* only needed if your application is using a dialog box and needs to
        ///      * respond to a "QueryCancelAutoPlay" message, it cannot simply return TRUE or FALSE.
        ///      SetWindowLong(this.Handle, 0, 1);
        ///      */
        ///      m.Result = (IntPtr)1;
        ///      }
        /// }
        ///  ]]></code>
        /// </example>
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern uint RegisterWindowMessage(string lpString);

        public const UInt32 QueryCancelAutoPlay = 0;

        [DllImport("user32.dll")]
        public static extern IntPtr WindowFromPoint(System.Drawing.Point p);

        #region cursor API

        [DllImport("user32.dll")]
        public static extern int ShowCursor(bool bShow);

        [DllImport("user32.dll")]
        public static extern IntPtr SetCursor(IntPtr handle);

        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int x, int y);

        /// <summary>
        /// IntPtr hcursor = LoadCursor(IntPtr.Zero, IDC_ARROW);
        /// bool ret_val = SetSystemCursor(hcursor, OCR_NORMAL);
        /// </summary>
        /// <param name="hInstance"></param>
        /// <param name="lpCursorName"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern IntPtr LoadCursor(IntPtr hInstance, int lpCursorName);

        [DllImport("user32.dll")]
        public static extern IntPtr LoadCursorFromFile(string lpFileName);

        [DllImport("user32.dll")]
        public static extern bool SetSystemCursor(IntPtr hcur, OCR_SYSTEM_CURSORS id);

        public static string GetCursorRegistryName(OCR_SYSTEM_CURSORS cursorId)
        {
            switch (cursorId)
            {
                case OCR_SYSTEM_CURSORS.OCR_APPSTARTING:
                    return "AppStarting";
                case OCR_SYSTEM_CURSORS.OCR_CROSS:
                    return "Cross";         // not in aero set?
                case OCR_SYSTEM_CURSORS.OCR_HAND:
                    return "Hand";
                case OCR_SYSTEM_CURSORS.OCR_HELP:
                    return "Help";
                case OCR_SYSTEM_CURSORS.OCR_IBEAM:
                    return "IBeam";          // not in aero set
                case OCR_SYSTEM_CURSORS.OCR_NO:
                    return "No";
                case OCR_SYSTEM_CURSORS.OCR_NORMAL:
                    return "Arrow";
                case OCR_SYSTEM_CURSORS.OCR_SIZEALL:
                    return "SizeAll";
                case OCR_SYSTEM_CURSORS.OCR_SIZENESW:
                    return "SizeNESW";
                case OCR_SYSTEM_CURSORS.OCR_SIZENS:
                    return "SizeNS";
                case OCR_SYSTEM_CURSORS.OCR_SIZENWSE:
                    return "SizeNWSE";
                case OCR_SYSTEM_CURSORS.OCR_SIZEWE:
                    return "SizeWE";
                case OCR_SYSTEM_CURSORS.OCR_UP:
                    return "UpArrow";
                case OCR_SYSTEM_CURSORS.OCR_WAIT:
                    return "Wait";
            }
            return null;
        }

        public static Dictionary<OCR_SYSTEM_CURSORS, string> GetSystemCursors()
        {
            var r = new Dictionary<OCR_SYSTEM_CURSORS, string>();
            foreach (var cs in Enum.GetValues(typeof(OCR_SYSTEM_CURSORS)))
            {
                var cname = GetCursorRegistryName((OCR_SYSTEM_CURSORS)cs);
                if (cname != null)
                {
                    var cspath = (string)Registry.GetValue(
                        @"HKEY_CURRENT_USER\Control Panel\Cursors\",
                        cname,
                        string.Empty);
                    r.Add((OCR_SYSTEM_CURSORS)cs, cspath);
                }
            }
            return r;
        }

        public static void SetSystemCursors(IDictionary<OCR_SYSTEM_CURSORS, string> cursors)
        {
            foreach (var cskv in cursors)
            {
                var cname = GetCursorRegistryName(cskv.Key);
                if (cname != null)
                    SetSystemCursor(cskv.Key, cskv.Value);
            }
        }

        public static void SetSystemCursors(string curFile)
        {
            foreach (var cs in Enum.GetValues(typeof(OCR_SYSTEM_CURSORS)))
            {
                var csName = GetCursorRegistryName((OCR_SYSTEM_CURSORS)cs);
                if (csName != null)
                    SetSystemCursor((OCR_SYSTEM_CURSORS)cs, curFile);
            }
        }

        public static bool SetSystemCursor(
            OCR_SYSTEM_CURSORS cursor,
            string curFile)
        {
            var csName = GetCursorRegistryName(cursor);
            if (csName == null)
                return false;
            Registry.SetValue(
                @"HKEY_CURRENT_USER\Control Panel\Cursors\",
                csName,
                curFile);

            ANIMATIONINFO ai = new ANIMATIONINFO();
            var r = SystemParametersInfo(
                SPI.SPI_SETCURSORS,
                0,
                ref ai,
                SPIF.SPIF_UPDATEINIFILE | SPIF.SPIF_SENDCHANGE);
            return r;
        }

        #endregion

        #region vista api

        [DllImport("dwmapi.dll")]
        public static extern void DwmIsCompositionEnabled(ref bool pfEnabled);

        [DllImport("dwmapi.dll")]
        public static extern int
            DwmExtendFrameIntoClientArea(System.IntPtr hWnd, ref MARGINS pMargins);

        [DllImport("dwmapi.dll")]
        public static extern int DwmGetWindowAttribute(
            IntPtr hwnd, int attr, out RECT ptr, int size);

        public const int DWMWA_CAPTION_BUTTON_BOUNDS = 5;

        [Flags]
        public enum DwmBlurBehindFlags : uint
        {
            DWM_BB_ENABLE = 0x00000001,
            DWM_BB_BLURREGION = 0x00000002,
            DWM_BB_TRANSITIONONMAXIMIZED = 0x00000004
        }

#if true || windows10
        [DllImport("user32.dll")]
        public static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

        public enum WindowCompositionAttribute
        {
            // ...
            WCA_ACCENT_POLICY = 19
            // ...
        }

        public enum AccentState
        {
            ACCENT_DISABLED = 0,
            ACCENT_ENABLE_GRADIENT = 1,
            ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
            ACCENT_ENABLE_BLURBEHIND = 3,

            //ACCENT_INVALID_STATE = 4              // WIN 7/Vista

            ACCENT_ENABLE_ACRYLICBLURBEHIND = 4,    // WIN 10
            ACCENT_INVALID_STATE = 5                // WIN 10
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct AccentPolicy
        {
            public AccentState AccentState;
            public uint AccentFlags;
            public uint GradientColor;
            public uint AnimationId;
        }

        public static void DwmEnableBBW(bool en, IntPtr handle)
        {
            DWM_BLURBEHIND blurBehindParameters = new DWM_BLURBEHIND();
            blurBehindParameters.dwFlags =
                DwmBlurBehindFlags.DWM_BB_ENABLE;
            blurBehindParameters.fEnable = en;
            blurBehindParameters.hRgnBlur = IntPtr.Zero;
            DwmEnableBlurBehindWindow(handle, ref blurBehindParameters);
        }

        public static void DwmDisableBBW(IntPtr handle)
        {
            DWM_BLURBEHIND blurBehindParameters = new DWM_BLURBEHIND();
            blurBehindParameters.dwFlags =
                0;
            blurBehindParameters.fEnable = false;
            blurBehindParameters.hRgnBlur = IntPtr.Zero;
            DwmEnableBlurBehindWindow(handle, ref blurBehindParameters);
        }

        public static void EnableBlur(System.Windows.Window win)
        {
            var windowHelper = new WindowInteropHelper(win);

            var accent = new AccentPolicy();
            var accentStructSize = Marshal.SizeOf(accent);
            //accent.AccentState = AccentState.ACCENT_ENABLE_BLURBEHIND;
            //accent.AccentState = AccentState.ACCENT_DISABLED;
            accent.AccentState = AccentState.ACCENT_ENABLE_TRANSPARENTGRADIENT;

            var accentPtr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accent, accentPtr, false);

            var data = new WindowCompositionAttributeData();
            data.Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY;
            data.SizeOfData = accentStructSize;
            data.Data = accentPtr;

            SetWindowCompositionAttribute(
                windowHelper.Handle,
                ref data);

            Marshal.FreeHGlobal(accentPtr);
        }
#endif

        [StructLayout(LayoutKind.Sequential)]
        public struct DWM_BLURBEHIND
        {
            public DwmBlurBehindFlags dwFlags;
            public bool fEnable;
            public IntPtr hRgnBlur;
            public bool fTansitionOnMaximized;
        }

        [DllImport("dwmapi.dll")]
        public static extern IntPtr DwmEnableBlurBehindWindow(IntPtr hwnd, ref DWM_BLURBEHIND blurBehind);

        public static void WmGetMinMaxInfo(System.IntPtr hwnd, System.IntPtr lParam)
        {
            MINMAXINFO mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));

            // Adjust the maximized size and position to fit the work area of the correct monitor

            int MONITOR_DEFAULTTONEAREST = 0x00000002;
            System.IntPtr monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);
            if (monitor != System.IntPtr.Zero)
            {
                var monitorInfo = new MonitorInfo();
                GetMonitorInfo(monitor, ref monitorInfo);
                Rect rcWorkArea = monitorInfo.WorkArea;
                Rect rcMonitorArea = monitorInfo.Monitor;
                mmi.ptMaxPosition.X = Math.Abs(rcWorkArea.Left - rcMonitorArea.Left);
                mmi.ptMaxPosition.Y = Math.Abs(rcWorkArea.Top - rcMonitorArea.Top);
                mmi.ptMaxSize.X = Math.Abs(rcWorkArea.Right - rcWorkArea.Left);
                mmi.ptMaxSize.Y = Math.Abs(rcWorkArea.Bottom - rcWorkArea.Top);
            }
            Marshal.StructureToPtr(mmi, lParam, true);
        }

        [DllImport("dwmapi.dll")]
        public static extern int DwmRegisterThumbnail(IntPtr dest, IntPtr src, out IntPtr thumb);

        [DllImport("dwmapi.dll")]
        public static extern int DwmUnregisterThumbnail(IntPtr thumb);

        [DllImport("dwmapi.dll")]
        public static extern int DwmQueryThumbnailSourceSize(IntPtr thumb, out PSIZE size);

        [StructLayout(LayoutKind.Sequential)]
        public struct PSIZE
        {
            public int x;
            public int y;
        }

        [DllImport("dwmapi.dll")]
        public static extern int DwmUpdateThumbnailProperties(IntPtr hThumb, ref DWM_THUMBNAIL_PROPERTIES props);

        [StructLayout(LayoutKind.Sequential)]
        public struct DWM_THUMBNAIL_PROPERTIES
        {
            public int dwFlags;
            public Rect rcDestination;
            public Rect rcSource;
            public byte opacity;
            public bool fVisible;
            public bool fSourceClientAreaOnly;
        }

        public const int DWM_TNP_VISIBLE = 0x8;
        public const int DWM_TNP_OPACITY = 0x4;
        public const int DWM_TNP_RECTDESTINATION = 0x1;

        #endregion
    }
}

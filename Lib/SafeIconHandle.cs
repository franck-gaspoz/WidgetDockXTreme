using Microsoft.Win32.SafeHandles;
using System.Security.Permissions;

namespace DesktopPanelTool.Lib
{
    [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
    public class SafeIconHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public SafeIconHandle()
            : base(true)
        {
        }

        override protected bool ReleaseHandle()
        {
            return NativeMethods.DestroyIcon(handle);
        }
    }
}

using System.IO;
using System.Runtime.CompilerServices;

namespace DesktopPanelTool.Lib
{
    public static class Debug
    {
        public static void WriteLine(object s,[CallerFilePath] string filePath = "",[CallerMemberName] string memberName = "")
        {
            var p = Path.GetFileNameWithoutExtension(filePath);
            System.Diagnostics.Debug.WriteLine($"[{p}.{memberName}] {s}");
        }
    }
}

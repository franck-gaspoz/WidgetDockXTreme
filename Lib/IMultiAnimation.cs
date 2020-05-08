using System.Windows;

namespace DesktopPanelTool.Lib
{
    public interface IMultiAnimation
    {
        void Start(FrameworkElement target,string name = null);

        void Stop(string name = null);
    }
}

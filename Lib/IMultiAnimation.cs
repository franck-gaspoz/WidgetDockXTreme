using System.Windows;
using System.Windows.Media.Animation;

namespace DesktopPanelTool.Lib
{
    public interface IMultiAnimation
    {
        void Start(FrameworkElement target,string name = null);

        void Stop(string name = null);
    }
}

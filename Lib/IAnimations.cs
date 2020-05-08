using System;
using System.Windows;

namespace DesktopPanelTool.Lib
{
    public interface IAnimations
    {
        void Start(FrameworkElement target,string name = null, object parameters = null,EventHandler completed = null);

        void Stop(string name = null);
    }
}

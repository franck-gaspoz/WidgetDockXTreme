using DesktopPanelTool.Lib;
using DesktopPanelTool.Models;

namespace DesktopPanelTool.ViewModels
{
    public interface IDesktopPanelBaseViewModel
    {
        DockName Dock { get; }
        ScreenInfo DockScreen { get; }
        bool IsCollapsed { get; }
        bool IsDocked { get; }
        bool IsPined { get; }
        string Title { get; set; }

        void NotifyPropertyUpdated(string propName);
        void AttachToDock(DockName dock, ScreenInfo dockScreen);
        void Collapse();
        void Expand();
        void TogglePin();
        void Pin();
        void UnPin();
    }
}
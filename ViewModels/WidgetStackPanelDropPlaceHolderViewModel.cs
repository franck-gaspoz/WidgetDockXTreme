using DesktopPanelTool.Lib;

namespace DesktopPanelTool.ViewModels
{
    public class WidgetStackPanelDropPlaceHolderViewModel
        : ViewModelBase
    {
        public DesktopPanelBaseViewModel PanelViewModel { get; protected set; }

        public WidgetStackPanelDropPlaceHolderViewModel(DesktopPanelBaseViewModel panelViewModel)
        {
            PanelViewModel = panelViewModel;
        }
    }
}

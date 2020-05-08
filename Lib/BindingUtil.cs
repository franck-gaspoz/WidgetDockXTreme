using DesktopPanelTool.Controls;
using DesktopPanelTool.Views;

namespace DesktopPanelTool.Lib
{
    public static class BindingUtil
    {
        public static void UpdateWidgetViewBindings(WidgetControl o, DesktopPanelBase oldDesktopPanelBase,DesktopPanelBase newDesktopPanelBase)
        {
            var childs = WPFUtil.FindChilds<IconButton>(o);
            foreach ( var c in childs )
            {
                if (c.ClickHandlerCommandParameter is object[] t && t.Length==2 && t[0]==oldDesktopPanelBase)
                {
                    t[0] = newDesktopPanelBase;
                }
            }
        }
    }
}

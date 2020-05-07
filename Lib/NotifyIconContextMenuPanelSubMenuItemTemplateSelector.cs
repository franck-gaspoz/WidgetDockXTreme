using System.Windows;
using System.Windows.Controls;

namespace DesktopPanelTool.Lib
{
    public class NotifyIconContextMenuPanelSubMenuItemTemplateSelector 
        : ItemContainerTemplateSelector
    {
        public override DataTemplate SelectTemplate(
            object item, ItemsControl parentItemsControl)
        {
            var tpl = (DataTemplate)parentItemsControl.FindResource("MenuPanelSubMenuItemTemplate");
            return tpl;
        }
    }
}

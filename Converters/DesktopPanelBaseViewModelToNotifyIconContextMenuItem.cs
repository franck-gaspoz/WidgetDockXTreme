using DesktopPanelTool.ViewModels;
using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace DesktopPanelTool.Converters
{
    [ValueConversion(typeof(DesktopPanelBaseViewModel),typeof(MenuItem))]
    public class DesktopPanelBaseViewModelToNotifyIconContextMenuItemConverter
        : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DesktopPanelBaseViewModel o)
            {
                var menuItem = new MenuItem() { Header = o.Title };
                
                return menuItem;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}

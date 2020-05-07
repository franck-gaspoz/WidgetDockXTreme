using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DesktopPanelTool.Converters
{
    [ValueConversion(typeof(object),typeof(Visibility))]
    public class ObjectNotNullToVisibilityConverter
        : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value==null? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return false;
        }
    }
}

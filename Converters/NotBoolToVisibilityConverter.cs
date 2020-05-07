using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DesktopPanelTool.Converters
{
    [ValueConversion(typeof(bool),typeof(Visibility))]
    public class NotBoolToVisibilityConverter
        : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
                return !b ? Visibility.Visible : Visibility.Collapsed;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility v)
                return v == Visibility.Visible ? false : true;
            return false;
        }
    }
}

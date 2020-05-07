using System;
using System.Globalization;
using System.Windows.Data;

namespace DesktopPanelTool.Converters
{
    [ValueConversion(typeof(bool),typeof(bool))]
    public class NotBoolConverter
        : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
                return !b;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
                return !b;
            return false;
        }
    }
}

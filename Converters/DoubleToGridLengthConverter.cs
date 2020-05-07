using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DesktopPanelTool.Converters
{
    [ValueConversion (typeof(double),typeof(GridLength))]
    public class DoubleToGridLengthConverter
        : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is Double d)
                return new GridLength(d);
            return new GridLength(0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is GridLength gl)
                return gl.Value;
            return 0d;
        }
    }
}

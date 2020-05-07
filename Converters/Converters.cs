using System.Windows.Data;

namespace DesktopPanelTool.Converters
{
    public static class Converters
    {
        public static IValueConverter BoolToVisibility => new BoolToVisibilityConverter();

        public static IValueConverter NotBoolToVisibility => new NotBoolToVisibilityConverter();

        public static IMultiValueConverter MultiValue => new MultiValueConverter();

        public static IValueConverter Debug => new DebugConverter();

        public static IValueConverter ObjectNotNullToVisibility => new ObjectNotNullToVisibilityConverter();

        public static IValueConverter NotBool => new NotBoolConverter();

    }
}

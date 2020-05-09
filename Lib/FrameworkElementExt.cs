using System.Windows;

namespace DesktopPanelTool.Lib
{
    internal static class FrameworkElementExt
    {
        internal static void MinimizeSize(this FrameworkElement element)
        {
            var size = element.DesiredSize;
            element.MaxWidth = size.Width;
            element.MaxHeight = size.Height;
        }

        internal static void MaximizeSize(this FrameworkElement element)
        {
            element.SetValue(FrameworkElement.MaxWidthProperty, DependencyProperty.UnsetValue);
            element.SetValue(FrameworkElement.MaxHeightProperty, DependencyProperty.UnsetValue);
        }
    }
}

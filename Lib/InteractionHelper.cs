using System.Linq;
using System.Windows;
using Microsoft.Xaml.Behaviors;

namespace DesktopPanelTool.Lib
{
    internal static class InteractionHelper
    {
        public static T GetBehavior<T>(DependencyObject obj)
            where T : Behavior
        {
            return Interaction.GetBehaviors(obj)
                .OfType<T>()
                .FirstOrDefault();
        }
    }
}

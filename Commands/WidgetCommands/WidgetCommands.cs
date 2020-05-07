using System.Windows.Input;

namespace DesktopPanelTool.Commands.WidgetCommands
{
    public static class WidgetCommands
    {
        public static ICommand Close { get; } = new CloseCommand();

    }
}

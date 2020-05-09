using System.Windows.Input;

namespace DesktopPanelTool.Commands.WidgetCommands
{
    public static class WidgetCommands
    {
        public static ICommand Close { get; } = new CloseCommand();
        public static ICommand DropWidget { get; } = new DropWidgetCommand();
        public static ICommand DropWidgetOnDesktop { get; } = new DropWidgetOnDesktopCommand();

    }
}

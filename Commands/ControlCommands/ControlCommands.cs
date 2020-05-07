using System.Windows.Input;

namespace DesktopPanelTool.Commands.ControlCommands
{
    public static class ControlCommands
    {
        public static ICommand OpenContextMenu { get; } = new OpenContextMenuCommand();

        public static ICommand HideWindow { get; } = new HideWindowCommand();

        public static ICommand OpenWindow { get; } = new OpenWindowCommand();

        public static ICommand CloseWindow { get; } = new CloseWindowCommand();

    }
}

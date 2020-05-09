using System.Windows.Input;

namespace DesktopPanelTool.Commands.ControlCommands
{
    public static class ControlCommands
    {
        public static ICommand OpenContextMenu { get; } = new OpenContextMenuCommand();

        public static ICommand HideWindow { get; } = new HideWindowCommand();

        public static ICommand OpenWindow { get; } = new OpenWindowCommand();

        public static ICommand CloseWindow { get; } = new CloseWindowCommand();

        public static ICommand SetIsEnabledBehavior { get; } = new SetIsEnabledBehaviorCommand();

        public static ICommand EnableClickOutsideHideOrCloseWindowBehavior { get; } = new EnableClickOutsideHideOrCloseWindowBehaviorCommand();

        public static ICommand DisableClickOutsideHideOrCloseWindowBehavior { get; } = new DisableClickOutsideHideOrCloseWindowBehaviorCommand();

    }
}

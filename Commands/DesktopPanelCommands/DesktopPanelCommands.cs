using System.Windows.Input;

namespace DesktopPanelTool.Commands.DesktopPanelCommands
{
    public static class DesktopPanelCommands
    {
        public static ICommand Close { get; } = new CloseCommand();

        public static ICommand AddSelectedWidget { get; } = new AddSelectedWidgetCommand();

        public static ICommand Show { get; } = new ShowCommand();

        public static ICommand Reset { get; } = new ResetCommand();

    }
}

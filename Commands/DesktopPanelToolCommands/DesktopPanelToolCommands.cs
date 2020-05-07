using System.Windows.Input;

namespace DesktopPanelTool.Commands.DesktopPanelToolCommands
{
    public static class DesktopPanelToolCommands
    {
        public static ICommand AddPanel { get; } = new AddPanelCommand();

        public static ICommand Settings { get; } = new SettingsCommand();

        public static ICommand Exit { get; } = new ExitCommand();

        public static ICommand OpenWebPage { get; } = new OpenWebPageCommand();
        
        public static ICommand CheckUpdates { get; } = new CheckUpdatesCommand();

        public static ICommand SaveSettings { get; } = new SaveSettingsCommand();

        public static ICommand SaveAsLayoutSettings { get; } = new SaveAsLayoutSettingsCommand();

        public static ICommand LoadLayoutSettings { get; } = new LoadLayoutSettingsCommand();

    }
}

using System;
using System.Reflection;

namespace DesktopPanelTool.Models
{
    [Serializable]
    public sealed class AppSettings
    {
        static AppSettings _instance;
        public static AppSettings Instance
        {
            get
            {
                if (_instance == null) _instance = new AppSettings();
                return _instance;
            }
        }

        public static void SetInstance(AppSettings appSettings) => _instance = appSettings;

        public static string AppTitle { get; } = "Widget Dock XTreme";
        public static string AppVersionExternal
        {
            get
            {
                var ver = Assembly.GetExecutingAssembly().GetName().Version;
                return $"{ver.Major}.{ver.MajorRevision}";
            }
        }
        public static string AppVersionpublic
        {
            get
            {
                var ver = Assembly.GetExecutingAssembly().GetName().Version;
                return $"{ver.Major}.{ver.MajorRevision}.{ver.Minor}";
            }
        }

        public static string AppIconPath { get; } = "/Images/Icons/add_hardware.ico";

        public static readonly double NotifyIconContextMenuDx = -12;
        public static readonly double NotifyIconContextMenuDy = 12;

        public static readonly double MinimumHorizontalDragDistance = 4;
        public static readonly double MinimumVerticalDragDistance = 4;

        public static string GetWidgetWebPageUri { get; private set; } = "http://franckgaspoz.fr";

        public static bool EnableNotifications { get; set; } = true;

        public static bool EnableWindowGradientAnimation = false;

        public static string SettingsSaveLastPath { get; set; }
        public static readonly string UserSettingsFileName = "settings.dat";
        public static readonly string AppSettingsFileName = "appSettings.dat";

        // TODO: use this to init behaviors
        public static int MouseWatcherScrutationDelay = 100;

        public readonly static string AppLocalDataFolderName = "Widget Dock XTreme";
        internal static readonly string SettingsFilesExt = ".dat";
        internal static readonly string SettingsFileDialogFilter = "dat files (*.dat)|*.dat|All files (*.*)|*.*";
    }
}

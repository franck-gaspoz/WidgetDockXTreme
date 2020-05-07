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

        //public static string AppIconPath { get; } = "/Images/Icons/Papirus-Team-Papirus-Apps-Chromium-app-list.ico";
        public static string AppIconPath { get; } = "/Images/Icons/add_hardware.ico";

        public static readonly double NotifyIconContextMenuDx = -12;
        public static readonly double NotifyIconContextMenuDy = 12;

        public static string GetWidgetWebPageUri { get; private set; } = "http://franckgaspoz.fr";

        // TODO: use this to init behaviors
        public static int MouseWatcherScrutationDelay = 100;
    }
}

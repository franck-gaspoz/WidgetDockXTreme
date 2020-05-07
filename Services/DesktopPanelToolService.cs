using DesktopPanelTool.Behaviors.WindowBehaviors;
using DesktopPanelTool.Controls;
using DesktopPanelTool.Lib;
using DesktopPanelTool.Models;
using DesktopPanelTool.ViewModels;
using DesktopPanelTool.Views;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Input;

namespace DesktopPanelTool.Services
{
    public static class DesktopPanelToolService
    {
        public static DesktopPanelToolViewModel DesktopPanelToolViewModel { get; private set; }
            = new DesktopPanelToolViewModel();
        
        internal static void Initialize()
        {
            var cursorDragging = Application.Current.Resources["CursorDragging"];
            var cursorDefault = Application.Current.Resources["CursorDefault"];
            if (cursorDragging != null)
                MovableTransparentWindowBehavior.CursorDragging = (Cursor)cursorDragging;
            if (cursorDefault != null)
                MovableTransparentWindowBehavior.CursorDefault = (Cursor)cursorDefault;
        }

        public static DesktopPanelBase AddDesktopPanel(
            DesktopPanelBase panel = null,
            DockName dock = DockName.Left,
            ScreenInfo screen = null,
            bool isPined = false,
            bool isCollapsed = false)
        {
            if (panel==null) panel = new DesktopPanelBase();
            DesktopPanelToolViewModel
                .PanelsViewModels
                .Add(panel.ViewModel);
            if (dock != DockName.None)
            {
                void attachMethod(object o, DependencyPropertyChangedEventArgs e)
                {
                    panel.IsVisibleChanged -= attachMethod;
                    panel.ViewModel.AttachToDock(
                        dock, 
                        screen);
                    if (isPined)
                        panel.ViewModel.Pin();
                    if (isCollapsed)
                        panel.ViewModel.Collapse();
                }
                panel.IsVisibleChanged += attachMethod;
            }
            return panel;
        }

        public static void ResetDockPanel(DesktopPanelBase panel)
        {

        }

        public static void ShowDockPanel(DesktopPanelBase panel)
        {
            if (panel.ViewModel.IsCollapsed)
                panel.ViewModel.Expand();
        }

        public static void CloseDockPanel(DesktopPanelBase panel)
        {
            DesktopPanelToolViewModel
                .PanelsViewModels
                .Remove(panel.ViewModel);
            DesktopPanelToolViewModel
                .RecentPanelsViewModels
                .Add(panel.ViewModel);
            panel.Close();
        }
        static string UserSettingsFullPath => Path.Combine(Environment.CurrentDirectory, AppSettings.UserSettingsFileName);
        static string AppSettingsFullPath => Path.Combine(Environment.CurrentDirectory, AppSettings.AppSettingsFileName);

        static internal bool UserSettingsFileExists =>
             File.Exists(UserSettingsFullPath);

        static internal bool AppSettingsFileExists =>
             File.Exists(AppSettingsFullPath);

        internal static void BootstrapUI()
        {
            foreach (var panel in DesktopPanelToolViewModel.PanelsViewModels)
                panel.View.Show();
        }

        internal static void SaveAsLayoutSettings()
        {
        }

        internal static void LoadLayoutSettings()
        {

        }

        internal static void SaveSettings()
        {
            SaveSettingsAs(UserSettingsFullPath);
        }
        
        internal static void SaveSettingsAs(string path)
        {
            DesktopPanelToolViewModel
               .RecentPanelsViewModels
               .Clear();
            var formatter = new BinaryFormatter();
            using FileStream fs = File.Create(path);
            formatter.Serialize(fs, DesktopPanelToolViewModel);
            SaveAppSettings();
        }

        internal static void SaveAppSettings()
        {
            var formatter = new BinaryFormatter();
            using FileStream fs2 = File.Create(AppSettingsFullPath);
            formatter.Serialize(fs2, AppSettings.Instance);
        }

        internal static void LoadAppSettings()
        {
            var formatter = new BinaryFormatter();
            using FileStream fs = new FileStream(AppSettingsFullPath, FileMode.Open);
            var appSettings = (AppSettings)formatter.Deserialize(fs);
            AppSettings.SetInstance(appSettings);
        }

        internal static void LoadSettings()
        {
            var formatter = new BinaryFormatter();
            object viewModel = null;
            using FileStream fs = new FileStream(UserSettingsFullPath, FileMode.Open);
            viewModel = formatter.Deserialize(fs);
            if (viewModel != null)
                DesktopPanelToolViewModel = (DesktopPanelToolViewModel)viewModel;
            LoadAppSettings();
        }

        /// <summary>
        /// first run new settings
        /// </summary>
        internal static void InitializeDefaultSettings()
        {
            var panel = AddDesktopPanel(null,DockName.None);
            panel.ViewModel.Title = "dock 1";

            var widget1 = new WidgetControl();
            widget1.ViewModel.Title = "widget 1";
            panel.ViewModel.AddWidget(widget1);

            var widget2 = new WidgetControl();
            widget2.ViewModel.Title = "widget 2";
            widget2.ViewModel.HasSettings = false;
            panel.ViewModel.AddWidget(widget2);

            var widget3 = new WidgetControl();
            widget3.ViewModel.Title = "widget 3";
            panel.ViewModel.AddWidget(widget3);

        }
    }
}

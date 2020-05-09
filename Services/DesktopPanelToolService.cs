//#define dbg

using DesktopPanelTool.Behaviors.FrameworkElementBehaviors;
using DesktopPanelTool.Behaviors.WindowBehaviors;
using DesktopPanelTool.Controls;
using DesktopPanelTool.Lib;
using DesktopPanelTool.Models;
using DesktopPanelTool.ViewModels;
using DesktopPanelTool.Views;
using Microsoft.Xaml.Behaviors;
using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static DesktopPanelTool.Models.NativeTypes;
using static DesktopPanelTool.Lib.NativeMethods;

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
            DockName dock = DockName.None,
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
            else
                panel.Activate();
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

        internal static string GetNewPanelDefaultTitle()
        {
            return $"dock {DesktopPanelToolViewModel.PanelsViewModels.Count + DesktopPanelToolViewModel.RecentPanelsViewModels.Count}";
        }

        internal static void DropWidgetOnDesktop(WidgetControl widget, FrameworkElement target, DragEventArgs e)
        {
            var oldpanel = WPFHelper.FindLogicalParent<DesktopPanelBase>(widget);
            widget.ViewModel.PanelViewModel.CloseWidget(widget);
            var panel = AddDesktopPanel();
            panel.ViewModel.Title = GetNewPanelDefaultTitle();
            var p = new POINT();
            GetCursorPos(ref p);
            var gap = (Thickness)widget.FindResource("WindowShadowAreaSize");
            var mind = (double)widget.FindResource("DropWidgetOnDesktopPanelMouseRelativeMinPosition");
            var maxd = (double)widget.FindResource("DropWidgetOnDesktopPanelMouseRelativeMaxPosition");            
            panel.Left = p.X - gap.Left -maxd - widget.WidthBackup/2d;
            panel.Top = p.Y - gap.Top -mind - widget.HeightBackup/2d;
            panel.ViewModel.AddWidget(widget);
            widget.UpdateWidgetViewBindings(oldpanel, panel);
            panel.Show();
        }

        internal static void DropWidget(WidgetControl widget,FrameworkElement target,DragEventArgs e)
        {
            var targetPanel = WPFHelper.FindLogicalParent<DesktopPanelBase>(target);
            var sourcePanel = WPFHelper.FindLogicalParent<DesktopPanelBase>(widget);
            var targetStack = WPFHelper.FindLogicalParent<StackPanel>(target);
            var sourceStack = WPFHelper.FindLogicalParent<StackPanel>(widget);
            if (targetPanel!=null && sourcePanel!=null && targetStack!=null && sourceStack!=null)
            {
                var dropAreaTarget = WPFHelper.FindLogicalParent<WidgetStackPanelDropPlaceHolder>(target);
                var idxSourceStack = sourceStack.Children.IndexOf(widget);
                var idxTargetStack = targetStack.Children.IndexOf(dropAreaTarget);
                var targetIsLargeDropArea = dropAreaTarget.Name == "PermanentWidgetDropPlaceHolder";
#if dbg
                DesktopPanelTool.Lib.Debug.WriteLine($"--------------- drop: {widget.ViewModel.Title} ------------ before:");
                DesktopPanelTool.Lib.Debug.WriteLine($"idxTargetStack={idxTargetStack} idxSourceStack={idxSourceStack}");
                DesktopPanelTool.Lib.Debug.WriteLine($"sourcePanel==targetPanel:{sourcePanel==targetPanel} targetIsLargeDropArea={targetIsLargeDropArea} idxSourceStack==sourceStack.Children.Count-1:{idxSourceStack == sourceStack.Children.Count - 1}");
#endif
                if (!((!targetIsLargeDropArea && sourcePanel == targetPanel && Math.Abs(idxTargetStack - idxSourceStack) == 1)
                    || (targetIsLargeDropArea && sourcePanel==targetPanel && idxSourceStack==sourceStack.Children.Count-1) 
                    ))
                {
                    widget.ViewModel.PanelViewModel.CloseWidget(widget);
                    if (targetIsLargeDropArea)
                        targetPanel.ViewModel.AddWidget(widget);
                    else
                    {
                        if (idxTargetStack > idxSourceStack)
                            idxTargetStack -= 2;
                        targetPanel.ViewModel.AddWidget(widget, idxTargetStack);
                    }
                    widget.UpdateWidgetViewBindings(sourcePanel, targetPanel);                    
                }

#if false && dbg
                DesktopPanelTool.Lib.Debug.WriteLine($"--------------- after:");
                widget.ViewModel.PanelViewModel.DumpWidgetsPanelChildren();
#endif
            }
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
            widget2.ViewModel.AutoSizeToFitPanelSize = true;
            panel.ViewModel.AddWidget(widget2);

            var widget3 = new WidgetControl();
            widget3.ViewModel.Title = "widget 3";
            panel.ViewModel.AddWidget(widget3);
            
            var scr = DisplayDevices.GetCurrentScreenInfo();
            var x = (scr.WorkArea.Width - panel.Width)/ 2d;
            var y = (scr.WorkArea.Height - panel.Height)/ 2d;
            panel.Left = x;
            panel.Top = y;
        }
    }
}

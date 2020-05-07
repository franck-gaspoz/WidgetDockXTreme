using DesktopPanelTool.Services;
using DesktopPanelTool.Views;
using System;
using System.Windows;

namespace DesktopPanelTool
{
    public partial class App : Application
    {
        DesktopPanelToolWindow DesktopPanelToolWindow;

        public App()
        {
        }

        void InitializeServices()
        {
            DesktopPanelToolService.Initialize();
            NotificationBarService.Initialize(DesktopPanelToolService.DesktopPanelToolViewModel);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            DesktopPanelToolWindow = new DesktopPanelToolWindow();

            bool settingsStorageMissing = false;
            bool reinitializeSettings = false;
            try
            {
                if (!DesktopPanelToolService.AppSettingsFileExists)
                    DesktopPanelToolService.SaveAppSettings();
                if (DesktopPanelToolService.UserSettingsFileExists)
                {
                    DesktopPanelToolService.LoadSettings();
                    reinitializeSettings = DesktopPanelToolService
                        .DesktopPanelToolViewModel
                        .PanelsViewModels.Count == 0;
                }
                else
                {
                    settingsStorageMissing = true;
                    reinitializeSettings = true;
                }                

            } catch (Exception ex)
            {

#if DEBUG
                DesktopPanelTool.Lib.Debug.WriteLine($"{ex}");
#endif

                reinitializeSettings = true;
            }

            if (reinitializeSettings)
                DesktopPanelToolService.InitializeDefaultSettings();

            InitializeServices();

            DesktopPanelToolService.BootstrapUI();

            //DesktopPanelToolWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            try
            {
                base.OnExit(e);
                DesktopPanelToolService.SaveSettings();
            }
#if DEBUG
            catch (Exception ex)
            {
                DesktopPanelTool.Lib.Debug.WriteLine($"{ex}");
#else
            catch (Exception) {
#endif
            }
            NotificationBarService.HideNotifyIcon();
            // fix visual studio designer exception (bug)
            Environment.Exit(0);
        }
    }
}

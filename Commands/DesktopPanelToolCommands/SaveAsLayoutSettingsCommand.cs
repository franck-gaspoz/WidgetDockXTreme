using DesktopPanelTool.Models;
using DesktopPanelTool.Services;
using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using w=System.Windows;

namespace DesktopPanelTool.Commands.DesktopPanelToolCommands
{
    public class SaveAsLayoutSettingsCommand
        : ICommand
    {
        public bool CanExecute(object parameter)
        {
            var p = GetParameter(parameter);
            return p.HasValue;
        }

        bool? GetParameter(object parameter)
        {
            if (parameter is bool)
                return (bool)parameter;
            else
                return false;
        }

        public void Execute(object parameter)
        {
            var p = GetParameter(parameter).Value;
            var path = AppSettings.SettingsSaveLastPath;
            if (path==null)
            {
                path = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                        , AppSettings.AppLocalDataFolderName);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            }

            var title = "Save layout settings";
            var dial = new OpenFileDialog()
            {
                InitialDirectory = path,
                DefaultExt = AppSettings.SettingsFilesExt,
                Filter = AppSettings.SettingsFileDialogFilter,
                CheckFileExists = false,
                CheckPathExists = true,
                Title= title
            };
            if (dial.ShowDialog() == DialogResult.OK)
            {
                var fn = dial.FileName;
                if (Path.GetExtension(fn) == string.Empty)
                    fn += AppSettings.SettingsFilesExt;
                if (!File.Exists(fn)
                    || w.MessageBox.Show(
                        $"File {fn} already exists. Replace it ?", 
                        title, 
                        MessageBoxButton.OKCancel,
                        MessageBoxImage.Question,
                        MessageBoxResult.OK) == MessageBoxResult.OK)
                {
                    DesktopPanelToolService.SaveSettingsAs(fn);
                    if (p && AppSettings.EnableNotifications)
                        NotificationBarService.Notify("settings have been saved");
                }
            }
        }

        public virtual event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}

using DesktopPanelTool.Lib;
using DesktopPanelTool.Models;
using System;
using System.Collections.ObjectModel;

namespace DesktopPanelTool.ViewModels
{
    [Serializable]
    public class DesktopPanelToolViewModel
        : ViewModelBase
    {
        public AppSettings AppSettings => AppSettings.Instance;

        public ObservableCollection<DesktopPanelBaseViewModel> PanelsViewModels { get; protected set; }
            = new ObservableCollection<DesktopPanelBaseViewModel>();

        public ObservableCollection<DesktopPanelBaseViewModel> RecentPanelsViewModels { get; protected set; }
            = new ObservableCollection<DesktopPanelBaseViewModel>();

        bool _showSplashScreenOnStartup = true;

        public bool ShowSplashScreenOnStartup
        {
            get
            {
                return _showSplashScreenOnStartup;
            }
            set
            {
                _showSplashScreenOnStartup = value;
                NotifyPropertyChanged();
            }
        }


    }
}

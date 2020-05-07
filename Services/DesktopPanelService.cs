using DesktopPanelTool.Controls;
using DesktopPanelTool.Lib;
using DesktopPanelTool.Model;
using DesktopPanelTool.ViewModels;
using DesktopPanelTool.Views;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace DesktopPanelTool.Services
{
    public static class DesktopPanelService
    {
        /*static ObservableCollection<DesktopPanelBaseViewModel> PanelsViewModels
            = new ObservableCollection<DesktopPanelBaseViewModel>();
            */

        public static DesktopPanelBase AddDesktopPanel(
            DockName dock=DockName.Left,
            ScreenInfo screen=null)
        {
            var panel = new DesktopPanelBase();
            PanelsViewModels.Add(panel.ViewModel);
            if (dock != DockName.None)
            {
                void attachMethod(object o, DependencyPropertyChangedEventArgs e)
                {
                    panel.IsVisibleChanged -= attachMethod;
                    panel.ViewModel.AttachToDock(dock, screen);
                }
                panel.IsVisibleChanged += attachMethod;
            }
            return panel;
        }

        public static void CloseDockPanel(DesktopPanelBase panel=null)
        {
            PanelsViewModels.Remove(panel.ViewModel);
            panel.Close();
        }
    }
}

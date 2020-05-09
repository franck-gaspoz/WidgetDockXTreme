using DesktopPanelTool.Lib;
using DesktopPanelTool.ViewModels;
using DesktopPanelTool.Views;
using System.Windows.Controls;

namespace DesktopPanelTool.Controls
{
    /// <summary>
    /// Logique d'interaction pour WidgetControl.xaml
    /// </summary>
    public partial class WidgetControl : UserControl, IAnimatableElement
    {
        public WidgetBaseViewModel ViewModel { get; set; }

        public double WidthBackup { get; set; }
        public double HeightBackup { get; set; }

        public WidgetControl()
        {
            ViewModel = new WidgetBaseViewModel(this);
            InitializeComponent();
            DataContext = ViewModel;
        }

        public WidgetControl(WidgetBaseViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            DataContext = ViewModel;
        }

        public void UpdateWidgetViewBindings(DesktopPanelBase oldDesktopPanelBase, DesktopPanelBase newDesktopPanelBase)
        {
            var childs = WPFHelper.FindChilds<IconButton>(this);
            foreach (var c in childs)
            {
                if (c.ClickHandlerCommandParameter is object[] t && t.Length == 2 && t[0] == oldDesktopPanelBase)
                {
                    t[0] = newDesktopPanelBase;
                }
            }
        }
    }
}

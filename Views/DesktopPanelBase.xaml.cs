using DesktopPanelTool.ViewModels;
using System.Windows;

namespace DesktopPanelTool.Views
{
    public partial class DesktopPanelBase : Window
    {
        public DesktopPanelBaseViewModel ViewModel { get; protected set; }

        public DesktopPanelBase()
        {
            ViewModel = new DesktopPanelBaseViewModel(this);
            InitializeComponent();
            DataContext = ViewModel;
        }

        public DesktopPanelBase(DesktopPanelBaseViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            DataContext = ViewModel;
        }
    }
}

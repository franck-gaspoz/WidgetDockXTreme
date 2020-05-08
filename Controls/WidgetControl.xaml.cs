using DesktopPanelTool.ViewModels;
using System.Windows.Controls;

namespace DesktopPanelTool.Controls
{
    /// <summary>
    /// Logique d'interaction pour WidgetControl.xaml
    /// </summary>
    public partial class WidgetControl : UserControl
    {
        public WidgetBaseViewModel ViewModel { get; set; }

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
    }
}

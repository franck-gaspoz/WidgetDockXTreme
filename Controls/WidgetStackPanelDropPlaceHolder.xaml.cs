using DesktopPanelTool.ViewModels;
using System.Windows.Controls;

namespace DesktopPanelTool.Controls
{
    /// <summary>
    /// Logique d'interaction pour WidgetStackPanelDropPlaceHolder.xaml
    /// </summary>
    public partial class WidgetStackPanelDropPlaceHolder : UserControl
    {
        public WidgetStackPanelDropPlaceHolderViewModel ViewModel { get; set; }

        public WidgetStackPanelDropPlaceHolder()
        {
            InitializeComponent();
        }

        public WidgetStackPanelDropPlaceHolder(WidgetStackPanelDropPlaceHolderViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
        }
    }
}
